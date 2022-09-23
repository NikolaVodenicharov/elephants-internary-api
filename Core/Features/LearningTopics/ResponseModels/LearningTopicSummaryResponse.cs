using Core.Features.Specialities.ResponseModels;

namespace Core.Features.LearningTopics.ResponseModels
{
    public record LearningTopicSummaryResponse(Guid Id, string Name, ICollection<SpecialitySummaryResponse> Specialities);
}