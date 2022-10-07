using Core.Common;
using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns.Interfaces;
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
        private readonly ICampaignsRepository campaignsRepository;
        private readonly ISpecialitiesRepository specialitiesRepository;
        private readonly ILogger<MentorsService> mentorsServiceLogger;
        private readonly IValidator<CreateMentorRequest> createMentorRequestValidator;
        private readonly IValidator<UpdateMentorRequest> updateMentorRequestValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;

        public MentorsService(
            IMentorsRepository mentorsRepository,
            ICampaignsRepository campaignsRepository,
            ISpecialitiesRepository specialitiesRepository,
            ILogger<MentorsService> mentorsServiceLogger,
            IValidator<CreateMentorRequest> createMentorRequestValidator,
            IValidator<UpdateMentorRequest> updateMentorRequestValidator,
            IValidator<PaginationRequest> paginationRequestValidator)
        {
            this.mentorsRepository = mentorsRepository;
            this.campaignsRepository = campaignsRepository;
            this.specialitiesRepository = specialitiesRepository;
            this.mentorsServiceLogger = mentorsServiceLogger;
            this.createMentorRequestValidator = createMentorRequestValidator;
            this.updateMentorRequestValidator = updateMentorRequestValidator;
            this.paginationRequestValidator = paginationRequestValidator;
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

        public async Task<PaginationResponse<MentorSummaryResponse>> GetPaginationAsync(PaginationRequest filter, Guid? campaignId = null)
        {
            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            int mentorCount;

            if (campaignId != null)
            {
                mentorCount = await GetCountByCampaignIdAsync(campaignId.Value);
                
                if (mentorCount == 0)
                {
                    if (filter.PageNum > PaginationConstants.DefaultPageCount)
                    {
                        LogErrorAndThrowExceptionPageCount(PaginationConstants.DefaultPageCount, filter.PageNum.Value);
                    }

                    var emptyPaginationResponse = new PaginationResponse<MentorSummaryResponse>(
                        new List<MentorSummaryResponse>(), filter.PageNum.Value, PaginationConstants.DefaultPageCount);

                    return emptyPaginationResponse;
                }
            }
            else
            {
                mentorCount = await GetCountAsync();

                if (mentorCount == 0)
                {
                    if (filter.PageNum > PaginationConstants.DefaultPageCount)
                    {
                        LogErrorAndThrowExceptionPageCount(PaginationConstants.DefaultPageCount, filter.PageNum.Value);
                    }

                    var emptyPaginationResponse = new PaginationResponse<MentorSummaryResponse>(
                        new List<MentorSummaryResponse>(), filter.PageNum.Value, PaginationConstants.DefaultPageCount);

                    return emptyPaginationResponse;
                }
            }

            var totalPages = (mentorCount + filter.PageSize.Value - 1) / filter.PageSize.Value;

            if (filter.PageNum > totalPages)
            {
                LogErrorAndThrowExceptionPageCount(totalPages, filter.PageNum.Value);
            }

            var mentors = campaignId != null ? 
                await mentorsRepository.GetAllAsync(filter, campaignId) : 
                await mentorsRepository.GetAllAsync(filter);

            var paginationResponse = new PaginationResponse<MentorSummaryResponse>(
                mentors.ToMentorSummaryResponses(), filter.PageNum.Value, totalPages);

            return paginationResponse;
        }

        public async Task<IEnumerable<MentorSummaryResponse>> GetAllAsync()
        {
            var mentors = await mentorsRepository.GetAllAsync();

            return mentors.ToMentorSummaryResponses();
        }

        public async Task<MentorSummaryResponse> GetByIdAsync(Guid id)
        {
            var mentor = await mentorsRepository.GetByIdAsync(id);

            if (mentor is null)
            {
                LogErrorAndThrowExceptionEntityNotFound("Mentor", id);
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
                LogErrorAndThrowExceptionEntityNotFound("Mentor", request.Id);
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

        public async Task<int> GetCountByCampaignIdAsync(Guid campaignId)
        {
            var count = await mentorsRepository.GetCountByCampaignIdAsync(campaignId);

            return count;
        }

        public async Task AddToCampaignAsync(AddToCampaignRequest request)
        {
            var campaign = await campaignsRepository.GetByIdAsync(request.CampaignId);

            if (campaign is null)
            {
                LogErrorAndThrowExceptionEntityNotFound("Campaign", request.CampaignId);
            }

            var mentor = await mentorsRepository.GetByIdAsync(request.PersonId);

            if (mentor is null)
            {
                LogErrorAndThrowExceptionEntityNotFound("Mentor", request.PersonId);
            }

            if (mentor.Campaigns.Contains(campaign))
            {
                mentorsServiceLogger.LogError($"[MentorsService] Mentor with Id {request.PersonId} is already assigned to" +
                    $" campaign with Id {request.CampaignId}");

                throw new CoreException($"Mentor {mentor.FirstName} {mentor.LastName} ({mentor.Email}) " +
                    $"is already assigned to campaign '{campaign.Name}'.", HttpStatusCode.BadRequest);
            }

            mentor.Campaigns.Add(campaign);

            await mentorsRepository.SaveTrackingChangesAsync();

            mentorsServiceLogger.LogInformation($"[MentorsService] Mentor with Id {request.PersonId} successfully assigned " +
                $"to campaign with Id {request.CampaignId}");
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

        private void LogErrorAndThrowExceptionPageCount(int totalPages, int pageNum)
        {
            var message = $"Total number of pages is {totalPages} and requested page number is {pageNum}";

            mentorsServiceLogger.LogError($"[{nameof(MentorsService)}] {message}");

            throw new CoreException(message, HttpStatusCode.BadRequest);
        }

        private void LogErrorAndThrowExceptionEntityNotFound(string entity, Guid id)
        {
            mentorsServiceLogger.LogError($"[{nameof(MentorsService)}] {entity} with Id {id} " +
                    $"was not found");

            throw new CoreException($"Requested {entity} was not found.", HttpStatusCode.NotFound);
        }
    }
}
