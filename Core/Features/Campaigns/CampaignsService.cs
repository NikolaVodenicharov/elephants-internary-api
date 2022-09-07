using Core.Common.Exceptions;
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
        private readonly IValidator<CreateCampaign> createCampaignValidator;
        private readonly IValidator<UpdateCampaign> updateCampaignValidator;
        private readonly IValidator<PaginationFilterRequest> paginationFilterRequestValidator;

        public CampaignsService(ICampaignsRepository campaignsRepository, 
            ILogger<CampaignsService> campaignsServiceLogger,
            IValidator<CreateCampaign> createCampaignValidator, 
            IValidator<UpdateCampaign> updateCampaignValidator,
            IValidator<PaginationFilterRequest> paginationFilterRequestValidator)
        {
            this.campaignsRepository = campaignsRepository;
            this.campaignsServiceLogger = campaignsServiceLogger;
            this.createCampaignValidator = createCampaignValidator;
            this.updateCampaignValidator = updateCampaignValidator;
            this.paginationFilterRequestValidator = paginationFilterRequestValidator;
        }

        public async Task<CampaignSummaryResponse> CreateAsync(CreateCampaign model)
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

        public async Task<CampaignSummaryResponse> UpdateAsync(UpdateCampaign model)
        {
            campaignsServiceLogger.LogInformation($"[CampaignsService] Executing update of campaign with Id {model.Id}.");

            await updateCampaignValidator.ValidateAndThrowAsync(model);

            var existingCampaign = await campaignsRepository.GetByIdAsync(model.Id);

            if (existingCampaign is null)
            {
                campaignsServiceLogger.LogError($"[CampaignsService] Campaign with Id {model.Id} was not found.");

                throw new CoreException("Requested campaign couldn't be found.", HttpStatusCode.NotFound);
            }

            if (existingCampaign.EndDate < DateTime.UtcNow)
            {
                campaignsServiceLogger.LogError($"[CampaignsService] Cannot update Campaign with Id {existingCampaign.Id} " +
                    $"because it has already ended.");

                throw new CoreException("Cannot update Campaign that has already ended.", HttpStatusCode.BadRequest);
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

            var result = await campaignsRepository.UpdateAsync(existingCampaign);
            
            campaignsServiceLogger.LogInformation($"[CampaignsService] Update of campaign with {model.Id} complete.");

            return result.ToCampaignSummary();
        }

        public async Task<IEnumerable<CampaignSummaryResponse>> GetAllAsync(PaginationFilterRequest filter)
        {
            await paginationFilterRequestValidator.ValidateAndThrowAsync(filter);

            var campaigns = await campaignsRepository.GetAllAsync(filter);

            return campaigns.ToCampaignSummaries();
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
    }
}
