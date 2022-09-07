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

        public CampaignsService(
            ICampaignsRepository campaignsRepository, 
            ILogger<CampaignsService> campaignsServiceLogger)
        {
            this.campaignsRepository = campaignsRepository;
            this.campaignsServiceLogger = campaignsServiceLogger;
        }

        public async Task<CampaignSummary> CreateAsync(CreateCampaign model)
        {
            campaignsServiceLogger.LogInformation($"[CampaignService] Executing create of campaign.");

            var validator = new CreateCampaignValidator();

            await validator.ValidateAndThrowAsync(model);

            var isExist = await campaignsRepository.ExistsByNameAsync(model.Name);

            if (isExist)
            {
                campaignsServiceLogger.LogError($"Campaign with name {model.Name} already exists!");

                throw new CoreException($"Campaign with name {model.Name} already exist.", HttpStatusCode.BadRequest);
            }

            var campaign = model.ToCampaign();

            var createdCampaign = await campaignsRepository.AddAsync(campaign);

            campaignsServiceLogger.LogInformation($"[CampaignService] Create new campaign with Id {createdCampaign.Id} complete.");

            return createdCampaign.ToCampaignSummary();
        }

        public async Task<CampaignSummary> UpdateAsync(UpdateCampaign model)
        {
            campaignsServiceLogger.LogInformation($"[CampaignService] Executing update of campaign with {model.Id}");

            var validator = new UpdateCampaignValidator();
            
            await validator.ValidateAndThrowAsync(model);

            var existingCampaign = await campaignsRepository.GetByIdAsync(model.Id);

            if (existingCampaign is null)
            {
                campaignsServiceLogger.LogError($"Campaign with Id {model.Id} was not found!");

                throw new CoreException($"Campaign with Id {model.Id} was not found!", HttpStatusCode.NotFound);
            }

            if (existingCampaign.EndDate < DateTime.UtcNow)
            {
                campaignsServiceLogger.LogError($"Cannot update Campaign {existingCampaign.Id} that has already ended!");

                throw new CoreException("Cannot update Campaign that has already ended!", HttpStatusCode.BadRequest);
            }

            var hasNameChange = !existingCampaign.Name.Equals(model.Name);
            
            if (hasNameChange)
            {
                var existsByName = await campaignsRepository.ExistsByNameAsync(model.Name);

                if(existsByName)
                {
                    campaignsServiceLogger.LogError($"Campaign with name {model.Name} already exists!");

                    throw new CoreException($"Campaign with name {model.Name} already exists!", HttpStatusCode.BadRequest);
                }
            }

            existingCampaign.Name = model.Name;
            existingCampaign.StartDate = model.StartDate;
            existingCampaign.EndDate = model.EndDate;
            existingCampaign.IsActive = model.IsActive;

            var result = await campaignsRepository.UpdateAsync(existingCampaign);
            
            campaignsServiceLogger.LogInformation($"[CampaignService] Update of campaign with {model.Id} complete.");

            return result.ToCampaignSummary();
        }
    }
}
