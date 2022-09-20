using Core.Features.LearningTopics.Entities;
using Core.Features.LearningTopics.RequestModels;
using Core.Features.LearningTopics.ResponseModels;
using Core.Features.Specialities.Support;

namespace Core.Features.LearningTopics.Support
{
    public static class LearningTopicsMappingExtensions
    {
        public static LearningTopic ToLearningTopic(this CreateLearningTopicRequest model)
        {
            var learningTopic = new LearningTopic() 
            {
                Name = model.Name
            };

            return learningTopic;
        }

        public static LearningTopicSummaryResponse ToLearningTopicSummary(this LearningTopic entity)
        {
            var summary = new LearningTopicSummaryResponse(
                entity.Id,
                entity.Name,
                entity.Specialities.ToSpecialitySummaryResponses().ToList()
            );

            return summary;
        }

        public static IEnumerable<LearningTopicSummaryResponse> ToLearningTopicSummaries(this IEnumerable<LearningTopic> entities)
        {
            var summaries = entities.Select(e => e.ToLearningTopicSummary());

            return summaries;
        }
    }
}