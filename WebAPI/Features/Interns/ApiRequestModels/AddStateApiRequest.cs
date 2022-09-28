using Core.Features.Interns.Entities;

namespace WebAPI.Features.Interns.ApiRequestModels
{
    public record AddStateApiRequest(
        StatusEnum StatusId,
        string Justification);
}
