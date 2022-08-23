using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;

namespace Core.Features.Campaigns.Interfaces
{
    public interface ICampaignsService
    {
        Task CreateAsync(CreateCampaign model);

        Task UpdateAsync(UpdateCampaign model);

        Task<CampaignSummary> GetAsync(Guid id);

        Task<IEnumerable<CampaignSummary>> GetAsync(GetAllCampaigns filter);
    }
}
