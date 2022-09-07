using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static CampaignSummaryResponse ToCampaignSummary(this Campaign entity)
        {
            var summary = new CampaignSummaryResponse(
                entity.Id,
                entity.Name,
                entity.StartDate,
                entity.EndDate,
                entity.IsActive);

            return summary;
        }

        public static Campaign ToCampaign(this UpdateCampaign model)
        {
            var campaign = new Campaign()
            {
                Id = model.Id,
                Name = model.Name,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive
            };

            return campaign;
        }

        public static IEnumerable<CampaignSummaryResponse> ToCampaignSummaries(this IEnumerable<Campaign> campaigns)
        {
            return campaigns.Select(c => c.ToCampaignSummary());
        }
    }
}
