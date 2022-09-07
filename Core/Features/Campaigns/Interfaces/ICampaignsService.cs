using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Features.Campaigns.Interfaces
{
    public interface ICampaignsService
    {
        Task<CampaignSummary> CreateAsync(CreateCampaign model);

        Task<CampaignSummary> UpdateAsync(UpdateCampaign model);
    }
}
