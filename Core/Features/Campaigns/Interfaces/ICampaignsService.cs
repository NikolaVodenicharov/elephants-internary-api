using Core.Common.Pagination;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;

namespace Core.Features.Campaigns.Interfaces
{
    public interface ICampaignsService
    {
        Task<CampaignSummaryResponse> CreateAsync(CreateCampaignRequest model);

        Task<CampaignSummaryResponse> UpdateAsync(UpdateCampaignRequest model);

        Task<CampaignSummaryResponse?> GetByIdAsync(Guid campaignId);

        Task<PaginationResponse<CampaignSummaryResponse>> GetAllAsync(PaginationRequest filter);

        Task<int> GetCountAsync();
    }
}
