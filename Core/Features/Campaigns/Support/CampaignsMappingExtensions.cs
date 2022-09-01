using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;

namespace Core.Features.Campaigns.Support
{
    public static class CampaignsMappingExtensions
    {
        public static Campaign ToCampaign(this CreateCampaign model)
        {
            var campaign = new Campaign()
            {
                Name = model.Name,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive
            };

            return campaign;
        }

        public static CampaignSummary ToCampaignSummary(this Campaign entity)
        {
            var summary = new CampaignSummary(
                entity.Id,
                entity.Name,
                entity.StartDate,
                entity.EndDate,
                entity.IsActive);

            return summary;
        }
    }
}
