using Core.Features.Mentors.Entities;
using Core.Features.LearningTopics.Entities;

namespace Core.Features.Specialties.Entities
{
    public class Speciality
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<Mentor> Mentors { get; set; }
        public ICollection<LearningTopic> LearningTopics { get; set; }
    }
}
