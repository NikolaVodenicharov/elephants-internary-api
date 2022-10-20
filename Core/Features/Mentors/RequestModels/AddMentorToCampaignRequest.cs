namespace Core.Features.Mentors.RequestModels
{
    public record AddMentorToCampaignRequest(Guid CampaignId, Guid MentorId);
}
