using Core.Features.Specialities.ResponseModels;
using Core.Features.Campaigns.ResponseModels;

namespace Core.Features.Mentors.ResponseModels
{
    public record MentorDetailsResponse(
        Guid Id,
        string DisplayName,
        string WorkEmail,
        ICollection<CampaignSummaryResponse> Campaigns,
        ICollection<SpecialitySummaryResponse> Specialities);
}
