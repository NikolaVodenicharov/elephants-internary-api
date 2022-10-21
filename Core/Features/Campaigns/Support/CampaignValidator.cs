using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using FluentValidation;

namespace Core.Features.Campaigns.Support
{
    public class CampaignValidator : ICampaignValidator
    {
        private readonly IValidator<CreateCampaignRequest> createCampaignValidator;
        private readonly IValidator<UpdateCampaignRequest> updateCampaignValidator;

        public CampaignValidator(
            IValidator<CreateCampaignRequest> createCampaignValidator, 
            IValidator<UpdateCampaignRequest> updateCampaignValidator)
        {
            this.createCampaignValidator = createCampaignValidator;
            this.updateCampaignValidator = updateCampaignValidator;
        }

        public async Task ValidateAndThrowAsync(CreateCampaignRequest request)
        {
            await createCampaignValidator.ValidateAndThrowAsync(request);
        }

        public async Task ValidateAndThrowAsync(UpdateCampaignRequest request)
        {
            await updateCampaignValidator.ValidateAndThrowAsync(request);
        }
    }
}
