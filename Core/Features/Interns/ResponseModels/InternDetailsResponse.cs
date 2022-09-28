namespace Core.Features.Interns.ResponseModels
{
    public record InternDetailsResponse(
        Guid Id, 
        string FirstName, 
        string LastName, 
        string Email, 
        IEnumerable<InternCampaignSummaryResponse> InternCampaignResponses);
}
