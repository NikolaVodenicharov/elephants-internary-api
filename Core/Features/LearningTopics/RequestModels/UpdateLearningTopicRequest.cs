namespace Core.Features.LearningTopics.RequestModels
{
    public record UpdateLearningTopicRequest(Guid Id, string Name, ICollection<Guid> SpecialityIds);
}