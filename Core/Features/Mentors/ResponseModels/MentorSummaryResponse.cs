using Core.Features.Specialities.ResponseModels;
namespace Core.Features.Mentors.ResponseModels
{
    public record MentorSummaryResponse(
        Guid Id, 
        string DisplayName, 
        string WorkEmail,
        IEnumerable<SpecialitySummaryResponse> Specialities);
}
