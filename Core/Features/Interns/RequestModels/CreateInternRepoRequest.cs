using Core.Features.Interns.Entities;

namespace Core.Features.Interns.RequestModels
{
    public record CreateInternRepoRequest(
        string FirstName,
        string LastName,
        string Email,
        InternCampaign InternCampaign);
}
