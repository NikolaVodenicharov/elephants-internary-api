using Core.Features.Campaigns.RequestModels;

namespace Core.Features.Campaigns.Interfaces
{
    public interface ICampaignValidator
    {
        Task ValidateAndThrowAsync(CreateCampaignRequest request);

        Task ValidateAndThrowAsync(UpdateCampaignRequest request);
    }
}
