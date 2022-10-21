using Core.Features.LearningTopics.RequestModels;

namespace Core.Features.LearningTopics.Interfaces
{
    public interface ILearningTopicValidator
    {
        Task ValidateAndThrowAsync(CreateLearningTopicRequest request);
        Task ValidateAndThrowAsync(UpdateLearningTopicRequest request);
    }
}
