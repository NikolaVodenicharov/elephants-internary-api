namespace Core.Features.Interns.RequestModels
{
    public record AddInternCampaignRequest(
        Guid InternId, 
        Guid CampaignId, 
        Guid SpecialityId, 
        string Justification);
}
