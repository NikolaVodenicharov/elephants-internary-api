using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
using Core.Features.Campaigns.Support;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Core.Features.Campaigns
{
    internal static class Counter
    {
        public static int campaignCount = -1;
    }

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
                var message = $"Campaign with name {model.Name} already exist.";

                campaignsServiceLogger.LogError($"[CampaignsService] {message}");

                throw new CoreException(message, HttpStatusCode.BadRequest);
            }

            var campaign = model.ToCampaign();

            var createdCampaign = await campaignsRepository.AddAsync(campaign);

            campaignsServiceLogger.LogInformation($"[CampaignsService] Create new campaign with Id {createdCampaign.Id} complete.");

            return createdCampaign.ToCampaignSummary();
        }

        public async Task<CampaignSummaryResponse> UpdateAsync(UpdateCampaignRequest model)
        {
            campaignsServiceLogger.LogInformation($"[CampaignsService] Executing update of campaign with Id {model.Id}.");

            await updateCampaignValidator.ValidateAndThrowAsync(model);

            var existingCampaign = await campaignsRepository.GetByIdAsync(model.Id);

            if (existingCampaign is null)
            {
                campaignsServiceLogger.LogError($"[CampaignsService] Campaign with Id {model.Id} was not found.");

                throw new CoreException("Requested campaign couldn't be found.", HttpStatusCode.NotFound);
            }

            var isCampaignCompleted = IsCampaignCompleted(existingCampaign.IsActive, existingCampaign.EndDate);
            
            if (isCampaignCompleted)
            {
                campaignsServiceLogger.LogError($"[CampaignsService] Campaign with Id {model.Id} is completed " +
                    $"and can't be updated.");

                throw new CoreException("Completed campaigns can't be updated.", HttpStatusCode.BadRequest);
            }

            var hasNameChange = !existingCampaign.Name.Equals(model.Name);
            
            if (hasNameChange)
            {
                var existsByName = await campaignsRepository.ExistsByNameAsync(model.Name);

                if(existsByName)
                {
                    var message = $"Campaign with name {model.Name} already exists.";

                    campaignsServiceLogger.LogError($"[CampaignsService] {message}");

                    throw new CoreException(message, HttpStatusCode.BadRequest);
                }
            }

            existingCampaign.Name = model.Name;
            existingCampaign.StartDate = model.StartDate;
            existingCampaign.EndDate = model.EndDate;
            existingCampaign.IsActive = model.IsActive;

            await campaignsRepository.SaveTrackingChangesAsync();
            
            campaignsServiceLogger.LogInformation($"[CampaignsService] Update of campaign with {model.Id} complete.");

            return existingCampaign.ToCampaignSummary();
        }

        public async Task<PaginationResponse<CampaignSummaryResponse>> GetAllAsync(PaginationRequest filter)
        {
            await paginationFilterRequestValidator.ValidateAndThrowAsync(filter);

            if (Counter.campaignCount == -1 || filter.PageNum == 1)
            {
                Counter.campaignCount = await GetCountAsync();
            }

            if (Counter.campaignCount == 0)
            {
                if (filter.PageNum > PaginationConstants.DefaultPageCount)
                {
                    LogErrorAndThrowExceptionPageCount(PaginationConstants.DefaultPageCount, filter.PageNum.Value);
                }

                var emptyPaginationResponse = new PaginationResponse<CampaignSummaryResponse>(
                    new List<CampaignSummaryResponse>(), filter.PageNum.Value, PaginationConstants.DefaultPageCount);

                return emptyPaginationResponse;
            }

            var totalPages = (Counter.campaignCount + filter.PageSize.Value - 1) / filter.PageSize.Value;

            if (filter.PageNum > totalPages)
            {
                LogErrorAndThrowExceptionPageCount(totalPages, filter.PageNum.Value);
            }

            var campaigns = await campaignsRepository.GetAllAsync(filter);

            var paginationResponse = new PaginationResponse<CampaignSummaryResponse>(
                campaigns.ToCampaignSummaries(), filter.PageNum.Value, totalPages);

            return paginationResponse;
        }

        public async Task<CampaignSummaryResponse?> GetByIdAsync(Guid campaignId)
        {
            var campaign = await campaignsRepository.GetByIdAsync(campaignId);

            if (campaign is null)
            {
                campaignsServiceLogger.LogError($"[CampaignsService] Campaign with Id {campaignId} was not found.");

                throw new CoreException("Requested campaign couldn't be found.", HttpStatusCode.NotFound);
            }

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

        private void LogErrorAndThrowExceptionPageCount(int totalPages, int pageNum)
        {
            var message = $"Total number of pages is {totalPages} and requested page number is {pageNum}";

            campaignsServiceLogger.LogError($"[{nameof(CampaignsService)}] {message}");

            throw new CoreException(message, HttpStatusCode.BadRequest);
        }
    }
}
