using Core.Features.Interns.Entities;

namespace WebAPI.Features.Interns.ApiRequestModels
{
    public record AddStateApiRequest(
        StatusId StatusId,
        string Justification,
        string? WorkEmail = null);
}
