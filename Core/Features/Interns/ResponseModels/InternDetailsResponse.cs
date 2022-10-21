namespace Core.Features.Interns.ResponseModels
{
    public record InternDetailsResponse(
        Guid Id, 
        string DisplayName,
        string FirstName, 
        string LastName,
        string PersonalEmail,
        string WorkEmail,
        IEnumerable<InternCampaignSummaryResponse> InternCampaignResponses);
}
