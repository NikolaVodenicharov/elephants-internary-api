using Core.Features.Campaigns.ResponseModels;
using Core.Features.Specialities.ResponseModels;

namespace Core.Features.Mentors.ResponseModels
{
    public record MentorPaginationResponse(
        Guid Id,
        string DisplayName,
        string WorkEmail,
        IEnumerable<SpecialitySummaryResponse> Specialities,
        IEnumerable<CampaignSummaryResponse> Campaigns);
}
