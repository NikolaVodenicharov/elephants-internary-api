using Core.Common;
using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Persons.Entities;
using Core.Features.Persons.Interfaces;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialties.Entities;
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
        private readonly IIdentityRepository identityRepository;
        private readonly ILogger<MentorsService> mentorsServiceLogger;
        private readonly IMentorValidator mentorValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;

        public MentorsService(
            IMentorsRepository mentorsRepository,
            ICampaignsRepository campaignsRepository,
            ISpecialitiesRepository specialitiesRepository,
            IIdentityRepository identityRepository,
            ILogger<MentorsService> mentorsServiceLogger,
            IMentorValidator mentorValidator,
            IValidator<PaginationRequest> paginationRequestValidator)
        {
            this.mentorsRepository = mentorsRepository;
            this.campaignsRepository = campaignsRepository;
            this.specialitiesRepository = specialitiesRepository;
            this.identityRepository = identityRepository;
            this.mentorsServiceLogger = mentorsServiceLogger;
            this.mentorValidator = mentorValidator;
            this.paginationRequestValidator = paginationRequestValidator;
        }

        public async Task<MentorSummaryResponse> CreateAsync(CreateMentorRequest createMentorRequest)
        {
            await mentorValidator.ValidateAndThrowAsync(createMentorRequest);

            await ValidateNoEmailDuplication(createMentorRequest.Email);

            var mentorSpecialities = await GetValidSpecialties(createMentorRequest.SpecialityIds.Distinct());

            var identitySummaryResponse = await identityRepository.SendUserInviteAsync(createMentorRequest.Email, createMentorRequest.ApplicationUrl);

            var createMentorRepoRequest = new CreateMentorRepoRequest(
                identitySummaryResponse.DisplayName,
                identitySummaryResponse.Email,
                mentorSpecialities);

            var mentorSummaryResponse = await mentorsRepository.CreateAsync(createMentorRepoRequest);

            return mentorSummaryResponse;
        }

        public async Task<PaginationResponse<MentorPaginationResponse>> GetPaginationAsync(PaginationRequest filter, Guid? campaignId = null)
        {
            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            Guard.EnsureNotNullPagination(filter.PageNum, filter.PageSize, mentorsServiceLogger,
                nameof(MentorsService));

            var mentorCount = campaignId != null ?
                await GetCountByCampaignIdAsync(campaignId.Value) :
                await GetCountAsync();

            var totalPages = PaginationMethods.CalculateTotalPages(mentorCount, filter.PageSize.Value);

            if (filter.PageNum > totalPages)
            {
                mentorsServiceLogger.LogErrorAndThrowExceptionPageCount(nameof(MentorsService), totalPages,
                    filter.PageNum.Value);
            }

            var mentorsPaginationRespose = mentorCount > 0 ?
                await mentorsRepository.GetAllAsync(filter, campaignId) :
                new List<MentorPaginationResponse>();

            var paginationResponse = new PaginationResponse<MentorPaginationResponse>(
                mentorsPaginationRespose, filter.PageNum.Value, totalPages);

            mentorsServiceLogger.LogInformationMethod(nameof(MentorsService), nameof(GetPaginationAsync), true);

            return paginationResponse;
        }

        public async Task<IEnumerable<MentorPaginationResponse>> GetAllAsync()
        {
            var mentors = await mentorsRepository.GetAllAsync();

            mentorsServiceLogger.LogInformationMethod(nameof(MentorsService), nameof(GetAllAsync), true);

            return mentors;
        }

        public async Task<MentorDetailsResponse> GetByIdAsync(Guid id)
        {
            var mentorDetailsResponse = await mentorsRepository.GetByIdAsync(id);

            Guard.EnsureNotNull(mentorDetailsResponse, mentorsServiceLogger, nameof(MentorsService), nameof(Person), id);

            mentorsServiceLogger.LogInformationMethod(nameof(MentorsService), nameof(GetByIdAsync), true);

            return mentorDetailsResponse;
        }

        public async Task<int> GetCountAsync()
        {
            var mentorCount = await mentorsRepository.GetCountAsync();

            return mentorCount;
        }

        public async Task<MentorDetailsResponse> UpdateAsync(UpdateMentorRequest updateMentorRequest)
        {
            await mentorValidator.ValidateAndThrowAsync(updateMentorRequest);

            var mentorSpecialities = await GetValidSpecialties(updateMentorRequest.SpecialityIds.Distinct());

            var updateMentorRepoRequest = new UpdateMentorRepoRequest(
                updateMentorRequest.Id,
                mentorSpecialities);

            var mentorDetailsReponse = await mentorsRepository.UpdateAsync(updateMentorRepoRequest);

            Guard.EnsureNotNull(mentorDetailsReponse, mentorsServiceLogger, nameof(MentorsService), nameof(Person));

            mentorsServiceLogger.LogInformationMethod(nameof(MentorsService), nameof(UpdateAsync), true);

            return mentorDetailsReponse;
        }

        public async Task<int> GetCountByCampaignIdAsync(Guid campaignId)
        {
            var count = await mentorsRepository.GetCountByCampaignIdAsync(campaignId);

            return count;
        }

        public async Task<bool> AddToCampaignAsync(AddMentorToCampaignRequest addMentorToCampaignRequest)
        {
            var campaign = await GetValidCampaign(addMentorToCampaignRequest.CampaignId);

            await ValidateMentorIsNotInCampaign(campaign, addMentorToCampaignRequest.MentorId);

            var addMentorToCampaignRepoRequest = new AddMentorToCampaignRepoRequest(addMentorToCampaignRequest.MentorId, campaign);

            var isMentorAdded = await mentorsRepository.AddToCampaignAsync(addMentorToCampaignRepoRequest);

            mentorsServiceLogger.LogInformationMethod(nameof(MentorsService), nameof(AddToCampaignAsync), true);

            return isMentorAdded;
        }

        private async Task ValidateMentorIsNotInCampaign(Campaign campaign, Guid mentorId)
        {
            var mentorDetailsResponse = await mentorsRepository.GetByIdAsync(mentorId);

            Guard.EnsureNotNull(
                mentorDetailsResponse,
                mentorsServiceLogger,
                nameof(MentorsService),
                nameof(Person),
                mentorId);

            var isMentorInCampaign = mentorDetailsResponse
                .Campaigns
                .Any(c => c.Id == campaign.Id);

            if (isMentorInCampaign)
            {
                mentorsServiceLogger.LogError("[{ServiceName}] Mentor with Id {MentorId} is already assigned to" +
                    " campaign with Id {CampaignId}", nameof(MentorsService), mentorId, campaign.Id);

                throw new CoreException($"Mentor {mentorDetailsResponse.DisplayName} ({mentorDetailsResponse.WorkEmail}) " +
                    $"is already assigned to campaign '{campaign.Name}'.", HttpStatusCode.BadRequest);
            }
        }

        private async Task<Campaign> GetValidCampaign(Guid id)
        {
            var campaign = await campaignsRepository.GetByIdAsync(id);

            Guard.EnsureNotNull(campaign, mentorsServiceLogger, nameof(MentorsService), nameof(Campaign), id);

            return campaign;
        }

        private async Task ValidateNoEmailDuplication(string email)
        {
            var isEmailUsed = await mentorsRepository.IsEmailUsed(email);

            if (isEmailUsed)
            {
                mentorsServiceLogger.LogErrorAndThrowExceptionValueTaken(nameof(MentorsService), nameof(Person), "email", email);
            }
        }

        private async Task<ICollection<Speciality>> GetValidSpecialties(IEnumerable<Guid> distinctSpecialtyIds)
        {
            var mentorSpecialities = await specialitiesRepository.GetByIdsAsync(distinctSpecialtyIds);

            if (mentorSpecialities.Count != distinctSpecialtyIds.Count())
            {
                mentorsServiceLogger.LogErrorAndThrowExceptionNotAllFound(nameof(MentorsService), nameof(Speciality), distinctSpecialtyIds);
            }

            return mentorSpecialities;
        }

        public async Task<bool> RemoveFromCampaignAsync(Guid campaignId, Guid mentorId)
        {
            var campaign = await GetValidCampaign(campaignId);

            var mentor = await mentorsRepository.GetByIdAsync(mentorId);

            Guard.EnsureNotNull(mentor, mentorsServiceLogger, nameof(MentorsService),
                nameof(Person), mentorId);

            if (!mentor.Campaigns.Any(c => c.Id == campaign.Id))
            {
                mentorsServiceLogger.LogError("[{ServiceName}] Mentor with Id {MentorId} is not assigned to " +
                    "campaign with Id {CampaignId}", nameof(MentorsService), mentorId, campaignId);

                throw new CoreException($"{mentor.DisplayName} ({mentor.WorkEmail}) is not assigned to " +
                    $"campaign '{campaign.Name}'", HttpStatusCode.BadRequest);
            }

            var isRemoved = await mentorsRepository.RemoveFromCampaignAsync(mentorId, campaign);

            mentorsServiceLogger.LogInformationMethod(nameof(MentorsService), nameof(RemoveFromCampaignAsync), true);

            return isRemoved;
        }
    }
}
