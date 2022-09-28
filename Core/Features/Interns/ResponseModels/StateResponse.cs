using Core.Features.Interns.Entities;

namespace Core.Features.Interns.ResponseModels
{
    public record StateResponse(string Status, string Justification, DateTime Created);
}
