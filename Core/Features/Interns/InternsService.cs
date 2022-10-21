using Core.Common;
using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;
using Core.Features.Persons.Entities;
using Core.Features.Persons.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Core.Features.Interns
{
    public class InternsService : IInternsService
    {
        private readonly IInternsRepository internsRepository;
        private readonly IIdentityRepository identityRepository;
        private readonly IInternCampaignsService internCampaignsService;
        private readonly ICampaignsService campaignsService;
        private readonly ILogger<InternsService> internsServiceLogger;
        private readonly IInternValidator internValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;
        private readonly IValidator<InviteInternRequest> inviteInternRequestValidator;

        public InternsService(
            IInternsRepository internsRepository,
            IIdentityRepository identityRepository,
            IInternCampaignsService internCampaignsService,
            ICampaignsService campaignsService,
            ILogger<InternsService> internsServiceLogger,
            IInternValidator internValidator,
            IValidator<PaginationRequest> paginationRequestValidator,
            IValidator<InviteInternRequest> inviteInternRequestValidator)
        {
            this.internsRepository = internsRepository;
            this.identityRepository = identityRepository;
            this.internCampaignsService = internCampaignsService;
            this.campaignsService = campaignsService;
            this.internsServiceLogger = internsServiceLogger;
            this.internValidator = internValidator;
            this.paginationRequestValidator = paginationRequestValidator;
            this.inviteInternRequestValidator = inviteInternRequestValidator;
        }

        public async Task<InternSummaryResponse> CreateAsync(CreateInternRequest createInternRequest)
        {
            await internValidator.ValidateAndThrowAsync(createInternRequest);

            await ValidateNoEmailDuplicationAsync(createInternRequest.Email);

            var createInternRepoRequest = await CreateInternRepoRequestGenerator(createInternRequest);

            var internSummaryResponse = await internsRepository.CreateAsync(createInternRepoRequest);

            internsServiceLogger.LogInformationMethod(nameof(InternsService), nameof(CreateAsync), true);

            return internSummaryResponse;
        }

        public async Task<InternSummaryResponse> UpdateAsync(UpdateInternRequest updateInternRequest)
        {
            await UpdateAsyncValidations(updateInternRequest);

            var updatedInternSumamryResponse = await internsRepository.UpdateAsync(updateInternRequest);

            Guard.EnsureNotNull(updatedInternSumamryResponse, internsServiceLogger, nameof(InternsService), nameof(Person), updateInternRequest.Id);

            return updatedInternSumamryResponse;
        }

        public async Task<IEnumerable<InternListingResponse>> GetAllAsync()
        {
            var interns = await internsRepository.GetAllAsync();

            internsServiceLogger.LogInformationMethod(nameof(InternsService), nameof(GetAllAsync), true);

            return interns;
        }

        public async Task<PaginationResponse<InternListingResponse>> GetPaginationAsync(PaginationRequest paginationRequest)
        {
            await paginationRequestValidator.ValidateAndThrowAsync(paginationRequest);

            var internPaginationResponse = await internsRepository.GetPaginationAsync(paginationRequest);

            PaginationResponseValidation(internPaginationResponse);

            internsServiceLogger.LogInformationMethod(nameof(InternsService), nameof(GetPaginationAsync), true);

            return internPaginationResponse;
        }

        public async Task<PaginationResponse<InternSummaryResponse>> GetAllByCampaignIdAsync(PaginationRequest paginationRequest, Guid campaignId)
        {
            await paginationRequestValidator.ValidateAndThrowAsync(paginationRequest);

            await EnsureCampaignExist(campaignId);

            var internSummaryResponseCollection = await internsRepository.GetPaginationByCampaignIdAsync(paginationRequest, campaignId);

            PaginationResponseValidation(internSummaryResponseCollection);

            internsServiceLogger.LogInformationMethod(nameof(InternsService), nameof(GetAllByCampaignIdAsync), true);

            return internSummaryResponseCollection;
        }

        public async Task<InternDetailsResponse> GetDetailsByIdAsync(Guid id)
        {
            var internDetailsResponse = await internsRepository.GetDetailsByIdAsync(id);

            Guard.EnsureNotNull(internDetailsResponse, internsServiceLogger,
                nameof(InternsService), nameof(Person), id);

            internsServiceLogger.LogInformationMethod(nameof(InternsService), nameof(GetDetailsByIdAsync),
                nameof(Person), id, true);

            return internDetailsResponse;
        }

        public async Task<InternSummaryResponse> InviteAsync(InviteInternRequest inviteInternRequest)
        {
            await inviteInternRequestValidator.ValidateAndThrowAsync(inviteInternRequest);

            var internDetailsResponse = await internsRepository.GetDetailsByIdAsync(inviteInternRequest.Id);

            Guard.EnsureNotNull(internDetailsResponse, internsServiceLogger, nameof(InternsService), nameof(Person), inviteInternRequest.Id);

            var isRequestEmailAlreadyAdded = internDetailsResponse.WorkEmail == inviteInternRequest.WorkEmail;

            if (isRequestEmailAlreadyAdded)
            {
                var InternSummaryResponse = new InternSummaryResponse(
                    internDetailsResponse.Id,
                    internDetailsResponse.DisplayName,
                    internDetailsResponse.WorkEmail);

                return InternSummaryResponse;
            }

            await ValidateNoEmailDuplicationAsync(inviteInternRequest.WorkEmail);

            var internSummaryResponse = await AddIdentityAsync(inviteInternRequest);

            return internSummaryResponse;
        }

        private async Task<InternSummaryResponse> AddIdentityAsync(InviteInternRequest inviteInternRequest)
        {
            var identitySummaryResponse = await identityRepository.SendUserInviteAsync(
                inviteInternRequest.WorkEmail,
                inviteInternRequest.ApplicationUrl);

            var addInternIdentityRepoRequest = new AddInternIdentityRepoRequest(
                inviteInternRequest.Id,
                identitySummaryResponse.Email,
                identitySummaryResponse.DisplayName);

            var internSummaryResponse = await internsRepository.AddIdentityAsync(addInternIdentityRepoRequest);

            Guard.EnsureNotNull(
                internSummaryResponse,
                internsServiceLogger,
                nameof(InternsService),
                nameof(Person),
                inviteInternRequest.Id);

            return internSummaryResponse;
        }

        private async Task ValidateNoEmailDuplicationAsync(string email)
        {
            var emailExist = await internsRepository.ExistsByPersonalEmailAsync(email);

            if (emailExist)
            {
                internsServiceLogger.LogErrorAndThrowExceptionValueTaken(nameof(InternsService), nameof(Person), nameof(email), email);
            }
        }

        private async Task<CreateInternRepoRequest> CreateInternRepoRequestGenerator(CreateInternRequest createInternRequest)
        {
            var internCampaign = await internCampaignsService.CreateInternCampaignAsync(
                createInternRequest.CampaignId,
                createInternRequest.SpecialityId,
                createInternRequest.Justification);

            var createInternRepoRequest = new CreateInternRepoRequest
            (
                createInternRequest.FirstName,
                createInternRequest.LastName,
                createInternRequest.Email,
                internCampaign);

            return createInternRepoRequest;
        }

        private async Task UpdateAsyncValidations(UpdateInternRequest updateInternRequest)
        {
            await internValidator.ValidateAndThrowAsync(updateInternRequest);

            var internSummaryResponse = await internsRepository.GetByIdAsync(updateInternRequest.Id);

            Guard.EnsureNotNull(internSummaryResponse, internsServiceLogger, nameof(InternsService), nameof(Person), updateInternRequest.Id);

            await ValidateNoEmailDuplicationAsync(updateInternRequest.Email);
        }

        private void PaginationResponseValidation<T> (PaginationResponse<T> internPaginationResponse)
        {
            var isPageNumBiggerThanTotalPages = internPaginationResponse.PageNum > internPaginationResponse.TotalPages;

            if (isPageNumBiggerThanTotalPages)
            {
                internsServiceLogger.LogErrorAndThrowExceptionPageCount(
                    nameof(InternsService),
                    internPaginationResponse.TotalPages,
                    internPaginationResponse.PageNum);
            }
        }

        private async Task EnsureCampaignExist(Guid campaignId)
        {
            var campaign = await campaignsService.GetByIdAsync(campaignId);

            Guard.EnsureNotNull(campaign, internsServiceLogger, nameof(InternsService), nameof(Campaign), campaignId);
        }
    }
}
