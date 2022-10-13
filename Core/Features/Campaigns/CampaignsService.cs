using Core.Common;
using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
using Core.Features.Campaigns.Support;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Core.Features.Campaigns
{
    public class CampaignsService : ICampaignsService
    {
        private readonly ICampaignsRepository campaignsRepository;
        private readonly ILogger<CampaignsService> campaignsServiceLogger;
        private readonly IValidator<CreateCampaignRequest> createCampaignValidator;
        private readonly IValidator<UpdateCampaignRequest> updateCampaignValidator;
        private readonly IValidator<PaginationRequest> paginationFilterRequestValidator;

        public CampaignsService(ICampaignsRepository campaignsRepository, 
            ILogger<CampaignsService> campaignsServiceLogger,
            IValidator<CreateCampaignRequest> createCampaignValidator, 
            IValidator<UpdateCampaignRequest> updateCampaignValidator,
            IValidator<PaginationRequest> paginationFilterRequestValidator)
        {
            this.campaignsRepository = campaignsRepository;
            this.campaignsServiceLogger = campaignsServiceLogger;
            this.createCampaignValidator = createCampaignValidator;
            this.updateCampaignValidator = updateCampaignValidator;
            this.paginationFilterRequestValidator = paginationFilterRequestValidator;
        }

        public async Task<CampaignSummaryResponse> CreateAsync(CreateCampaignRequest model)
        {
            await createCampaignValidator.ValidateAndThrowAsync(model);

            var isExist = await campaignsRepository.ExistsByNameAsync(model.Name);

            if (isExist)
            {
                campaignsServiceLogger.LogErrorAndThrowExceptionValueTaken(nameof(CampaignsService), nameof(Campaign),
                    nameof(Campaign.Name), model.Name);
            }

            var campaign = model.ToCampaign();

            var createdCampaign = await campaignsRepository.AddAsync(campaign);

            campaignsServiceLogger.LogInformationMethod(nameof(CampaignsService), nameof(CreateAsync), true);

            return createdCampaign.ToCampaignSummary();
        }

        public async Task<CampaignSummaryResponse> UpdateAsync(UpdateCampaignRequest model)
        {
            await updateCampaignValidator.ValidateAndThrowAsync(model);

            var existingCampaign = await campaignsRepository.GetByIdAsync(model.Id);

            Guard.EnsureNotNull(existingCampaign, campaignsServiceLogger,
                nameof(CampaignsService), nameof(Campaign), model.Id);

            var isCampaignCompleted = IsCampaignCompleted(existingCampaign.IsActive, existingCampaign.EndDate);
            
            if (isCampaignCompleted)
            {
                campaignsServiceLogger.LogError("[{ServiceName}] Campaign with Id {Id} is completed " +
                    "and can't be updated.", nameof(CampaignsService), model.Id);

                throw new CoreException("Completed campaigns can't be updated.", HttpStatusCode.BadRequest);
            }

            var hasNameChange = !existingCampaign.Name.Equals(model.Name);
            
            if (hasNameChange)
            {
                var existsByName = await campaignsRepository.ExistsByNameAsync(model.Name);

                if(existsByName)
                {
                    campaignsServiceLogger.LogErrorAndThrowExceptionValueTaken(nameof(CampaignsService), nameof(Campaign),
                        nameof(Campaign.Name), model.Name);
                }
            }

            existingCampaign.Name = model.Name;
            existingCampaign.StartDate = model.StartDate;
            existingCampaign.EndDate = model.EndDate;
            existingCampaign.IsActive = model.IsActive;

            await campaignsRepository.SaveTrackingChangesAsync();

            campaignsServiceLogger.LogInformationMethod(nameof(CampaignsService), nameof(UpdateAsync), 
                nameof(Campaign), model.Id, true);

            return existingCampaign.ToCampaignSummary();
        }

        public async Task<PaginationResponse<CampaignSummaryResponse>> GetAllAsync(PaginationRequest filter)
        {
            await paginationFilterRequestValidator.ValidateAndThrowAsync(filter);

            Guard.EnsureNotNullPagination(filter.PageNum, filter.PageSize, campaignsServiceLogger,
                nameof(CampaignsService));

            var campaignCount = await GetCountAsync();

            var totalPages = PaginationMethods.CalculateTotalPages(campaignCount, filter.PageSize.Value);

            if (filter.PageNum > totalPages)
            {
                campaignsServiceLogger.LogErrorAndThrowExceptionPageCount(nameof(CampaignsService), 
                    totalPages, filter.PageNum.Value);
            }

            var campaigns = campaignCount > 0 ?
                await campaignsRepository.GetAllAsync(filter) :
                new List<Campaign>();

            var paginationResponse = new PaginationResponse<CampaignSummaryResponse>(
                campaigns.ToCampaignSummaries(), filter.PageNum.Value, totalPages);

            campaignsServiceLogger.LogInformationMethod(nameof(CampaignsService), nameof(GetAllAsync), true);

            return paginationResponse;
        }

        public async Task<CampaignSummaryResponse?> GetByIdAsync(Guid campaignId)
        {
            var campaign = await campaignsRepository.GetByIdAsync(campaignId);

            Guard.EnsureNotNull(campaign, campaignsServiceLogger, nameof(CampaignsService), nameof(Campaign),
                campaignId);

            campaignsServiceLogger.LogInformationMethod(nameof(CampaignsService), nameof(GetByIdAsync),
                nameof(Campaign), campaignId, true);

            return campaign.ToCampaignSummary();
        }

        public async Task<int> GetCountAsync()
        {
            return await campaignsRepository.GetCountAsync();
        }

        private static bool IsCampaignCompleted(bool isActive, DateTime endDate)
        { 
            return !isActive && DateTime.Compare(DateTime.Today, endDate) > 0;
        }
    }
}
