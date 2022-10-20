using Core.Common;
using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;
using Core.Features.Persons.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Core.Features.Interns
{
    public class InternsService : IInternsService
    {
        private readonly IInternsRepository internsRepository;
        private readonly IInternCampaignsService internCampaignsService;
        private readonly ICampaignsService campaignsService;
        private readonly ILogger<InternsService> internsServiceLogger;
        private readonly IValidator<CreateInternRequest> createInternRequestValidator;
        private readonly IValidator<UpdateInternRequest> updateInternRequestValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;

        public InternsService(
            IInternsRepository internsRepository,
            IInternCampaignsService internCampaignsService,
            ICampaignsService campaignsService,
            ILogger<InternsService> internsServiceLogger,
            IValidator<CreateInternRequest> createInternRequestValidator,
            IValidator<UpdateInternRequest> updateInternRequestValidator,
            IValidator<PaginationRequest> paginationRequestValidator)
        {
            this.internsRepository = internsRepository;
            this.internCampaignsService = internCampaignsService;
            this.campaignsService = campaignsService;
            this.internsServiceLogger = internsServiceLogger;
            this.createInternRequestValidator = createInternRequestValidator;
            this.updateInternRequestValidator = updateInternRequestValidator;
            this.paginationRequestValidator = paginationRequestValidator;
        }

        public async Task<InternSummaryResponse> CreateAsync(CreateInternRequest createInternRequest)
        {
            await createInternRequestValidator.ValidateAndThrowAsync(createInternRequest);

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

        public async Task<PaginationResponse<InternSummaryResponse>> GetAllAsync(PaginationRequest paginationRequest)
        {
            await paginationRequestValidator.ValidateAndThrowAsync(paginationRequest);

            var internPaginationResponse = await internsRepository.GetAllAsync(paginationRequest);

            PaginationResponseValidation(internPaginationResponse);

            internsServiceLogger.LogInformationMethod(nameof(InternsService), nameof(GetAllAsync), true);

            return internPaginationResponse;
        }

        public async Task<PaginationResponse<InternByCampaignSummaryResponse>> GetAllByCampaignIdAsync(PaginationRequest paginationRequest, Guid campaignId)
        {
            await paginationRequestValidator.ValidateAndThrowAsync(paginationRequest);

            await EnsureCampaignExist(campaignId);

            var internsByCampaignPaginationResponse = await internsRepository.GetAllByCampaignIdAsync(paginationRequest, campaignId);

            PaginationResponseValidation(internsByCampaignPaginationResponse);

            internsServiceLogger.LogInformationMethod(nameof(InternsService), nameof(GetAllByCampaignIdAsync), true);

            return internsByCampaignPaginationResponse;
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
            await updateInternRequestValidator.ValidateAndThrowAsync(updateInternRequest);

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
