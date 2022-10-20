using Core.Features.Campaigns.Entities;

namespace Core.Features.Mentors.RequestModels
{
    public record AddMentorToCampaignRepoRequest(Guid MentorId, Campaign Campaign);
}
