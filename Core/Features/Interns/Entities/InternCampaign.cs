using Core.Features.Campaigns.Entities;
using Core.Features.Persons.Entities;
using Core.Features.Specialties.Entities;

namespace Core.Features.Interns.Entities
{
    public class InternCampaign
    {
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!;
        public Guid CampaignId { get; set; }
        public Campaign Campaign { get; set; } = null!;
        public Guid SpecialityId { get; set; }
        public Speciality Speciality { get; set; } = null!;
        public ICollection<State> States { get; set; } = null!;
    }
}
