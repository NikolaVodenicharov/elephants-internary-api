namespace WebAPI.Features.Interns.ApiRequestModels
{
    public record AddInternCampaignApiRequest(
        Guid SpecialityId, 
        string Justification);
}
