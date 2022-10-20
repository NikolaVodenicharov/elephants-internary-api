using Core.Features.Interns.Entities;

namespace Core.Features.Interns.RequestModels
{
    public record AddInternCampaignRepoRequest(
        Guid InternId,
        InternCampaign InternCampaign);
}
