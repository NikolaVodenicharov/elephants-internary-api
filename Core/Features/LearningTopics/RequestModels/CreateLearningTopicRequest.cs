namespace Core.Features.LearningTopics.RequestModels
{
    public record CreateLearningTopicRequest (string Name, ICollection<Guid> SpecialityIds);
}