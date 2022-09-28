namespace Core.Features.Interns.RequestModels
{
    public record CreateInternRequest (
        string FirstName, 
        string LastName, 
        string Email, 
        Guid CampaignId, 
        Guid SpecialityId,
        string Justification);
}
