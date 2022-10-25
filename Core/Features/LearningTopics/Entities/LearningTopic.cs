using Core.Features.Specialties.Entities;

namespace Core.Features.LearningTopics.Entities
{
    public class LearningTopic
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;

        public ICollection<Speciality> Specialities { get; set; } = null!;
    }
}