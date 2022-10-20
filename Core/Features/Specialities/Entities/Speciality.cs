using Core.Features.LearningTopics.Entities;
using Core.Features.Interns.Entities;
using Core.Features.Persons.Entities;

namespace Core.Features.Specialties.Entities
{
    public class Speciality
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IEnumerable<InternCampaign> InternCampaigns { get; set; } = null!;
        public ICollection<Person> Persons { get; set; } = null!;
        public ICollection<LearningTopic> LearningTopics { get; set; } = null!;
    }
}
