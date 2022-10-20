using Core.Features.Campaigns.Entities;
using Core.Features.Interns.Entities;
using Core.Features.Specialties.Entities;

namespace Core.Features.Persons.Entities
{
    public class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
        public string PersonalEmail { get; set; } = string.Empty;
        public ICollection<PersonRole> PersonRoles { get; set; } = null!;
        public ICollection<Campaign> Campaigns { get; set; } = null!;
        public ICollection<Speciality> Specialities { get; set; } = null!;
        public ICollection<InternCampaign> InternCampaigns { get; set; } = null!;
    }
}
