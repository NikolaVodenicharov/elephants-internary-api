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
        Task<CampaignSummaryResponse> CreateAsync(CreateCampaign model);

        Task<CampaignSummaryResponse> UpdateAsync(UpdateCampaign model);

        Task<CampaignSummaryResponse?> GetByIdAsync(Guid campaignId);

        Task<IEnumerable<CampaignSummaryResponse>> GetAllAsync(PaginationFilterRequest filter);

        Task<int> GetCountAsync();
    }
}
