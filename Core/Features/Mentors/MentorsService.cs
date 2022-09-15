using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Mentors.Support;
using Core.Features.Specialities.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Core.Features.Mentors
{
    public class MentorsService : IMentorsService
    {
        private readonly IMentorsRepository mentorsRepository;
        private readonly ISpecialitiesRepository specialitiesRepository;
        private readonly ILogger<MentorsService> mentorsServiceLogger;
        private readonly IValidator<CreateMentorRequest> createMentorRequestValidator;
        private readonly IValidator<UpdateMentorRequest> updateMentorRequestValidator;
        private readonly IValidator<PaginationFilterRequest> paginationFilterRequestValidator;

        public MentorsService(
            IMentorsRepository mentorsRepository,
            ISpecialitiesRepository specialitiesRepository,
            ILogger<MentorsService> mentorsServiceLogger,
            IValidator<CreateMentorRequest> createMentorRequestValidator,
            IValidator<UpdateMentorRequest> updateMentorRequestValidator,
            IValidator<PaginationFilterRequest> paginationFilterRequestValidator)
        {
            this.mentorsRepository = mentorsRepository;
            this.specialitiesRepository = specialitiesRepository;
            this.mentorsServiceLogger = mentorsServiceLogger;
            this.createMentorRequestValidator = createMentorRequestValidator;
            this.updateMentorRequestValidator = updateMentorRequestValidator;
            this.paginationFilterRequestValidator = paginationFilterRequestValidator;
        }

        public async Task<MentorSummaryResponse> CreateAsync(CreateMentorRequest request)
        {
            await createMentorRequestValidator.ValidateAndThrowAsync(request);

            await CheckEmailAsync(request.Email);

            if (request.SpecialityIds.Count() != request.SpecialityIds.Distinct().Count())
            {
                mentorsServiceLogger.LogError($"[MentorsService] Mentor can't have duplicate specialties: " +
                    $"[{String.Join(",", request.SpecialityIds)}]");

                throw new CoreException("Mentor can't have duplicate specialties.", HttpStatusCode.BadRequest);
            }

            var mentorSpecialities = await specialitiesRepository.GetByIdsAsync(request.SpecialityIds);

            if (mentorSpecialities.Count != request.SpecialityIds.Count())
            {
                mentorsServiceLogger.LogError($"[MentorsService] Not all specialities are found: " +
                    $"[{String.Join(", ", request.SpecialityIds)}]");

                throw new CoreException("Not all specialties were found.", HttpStatusCode.BadRequest);
            }

            var mentor = request.ToMentor();
            mentor.Specialities = mentorSpecialities;

            var createdMentor = await mentorsRepository.AddAsync(mentor);

            return createdMentor.ToMentorSummaryResponse();
        }

        public async Task<IEnumerable<MentorSummaryResponse>> GetAllAsync(PaginationFilterRequest filter)
        {
            await paginationFilterRequestValidator.ValidateAndThrowAsync(filter);

            var mentors = await mentorsRepository.GetAllAsync(filter);

            return mentors.ToMentorSummaryResponses();
        }

        public async Task<MentorSummaryResponse> GetByIdAsync(Guid id)
        {
            var mentor = await mentorsRepository.GetByIdAsync(id);

            if (mentor is null)
            {
                mentorsServiceLogger.LogError($"[MentorsService] Mentor with Id {id} was not found.");

                throw new CoreException("Requested mentor was not found.", HttpStatusCode.NotFound);
            }

            return mentor.ToMentorSummaryResponse();
        }

        public async Task<int> GetCountAsync()
        {
            var mentorCount = await mentorsRepository.GetCountAsync();

            return mentorCount;
        }

        public async Task<MentorSummaryResponse> UpdateAsync(UpdateMentorRequest request)
        {
            await updateMentorRequestValidator.ValidateAndThrowAsync(request);

            mentorsServiceLogger.LogInformation($"[MentorsService] Update mentor with Id {request.Id}");

            var existingMentor = await mentorsRepository.GetByIdAsync(request.Id);

            if (existingMentor is null)
            {
                mentorsServiceLogger.LogError($"[MentorsService] Mentor with Id {request.Id} wasn't found");

                throw new CoreException("Requested mentor wasn't found.", HttpStatusCode.NotFound);
            }

            var isEmailChanged = !existingMentor.Email.Equals(request.Email);

            if (isEmailChanged)
            {
                await CheckEmailAsync(request.Email);
            }

            if (request.SpecialityIds.Count() != request.SpecialityIds.Distinct().Count())
            {
                mentorsServiceLogger.LogError($"[MentorsService] Mentor can't have duplicate specialties: " +
                    $"[{String.Join(",", request.SpecialityIds)}]");

                throw new CoreException("Mentor can't have duplicate specialties.", HttpStatusCode.BadRequest);
            }

            var mentorSpecialities = await specialitiesRepository.GetByIdsAsync(request.SpecialityIds);

            if (mentorSpecialities.Count != request.SpecialityIds.Count())
            {
                mentorsServiceLogger.LogError($"[MentorsService] Not all specialties were found: " +
                    $"[{String.Join(", ", request.SpecialityIds)}]");

                throw new CoreException("Not all specialties were found.", HttpStatusCode.BadRequest);
            }

            existingMentor.FirstName = request.FirstName;
            existingMentor.LastName = request.LastName;
            existingMentor.Email = request.Email;
            existingMentor.Specialities = mentorSpecialities;

            await mentorsRepository.SaveTrackingChangesAsync();

            mentorsServiceLogger.LogInformation($"[MentorsService] Mentor with Id {request.Id} updated successfully");

            return existingMentor.ToMentorSummaryResponse();
        }

        public async Task<IEnumerable<MentorSummaryResponse>> GetMentorsByCampaignIdAsync(Guid campaignId, PaginationFilterRequest filter)
        {
            await paginationFilterRequestValidator.ValidateAndThrowAsync(filter);

            var mentors = await mentorsRepository.GetMentorsByCampaignIdAsync(campaignId, filter);

            return mentors.ToMentorSummaryResponses();
        }

        public async Task<int> GetCountByCampaignIdAsync(Guid campaignId)
        {
            var count = await mentorsRepository.GetCountByCampaignIdAsync(campaignId);

            return count;
        }

        private async Task CheckEmailAsync(string email)
        {
            var isEmailUsed = await mentorsRepository.IsEmailUsed(email);

            if (isEmailUsed)
            {
                var message = $"Email '{email}' is already used";

                mentorsServiceLogger.LogError($"[MentorsService] {message}");

                throw new CoreException($"{message}. Please choose a new one.", HttpStatusCode.BadRequest);
            }
        }
    }
}
