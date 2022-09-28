using Core.Features.Specialities.ResponseModels;

namespace Core.Features.Interns.ResponseModels
{
    public record InternByCampaignSummaryResponse(
    InternSummaryResponse InternSummaryResponse,
    SpecialitySummaryResponse SpecialitySummaryResponse,
    StateResponse StateResponse);
}
