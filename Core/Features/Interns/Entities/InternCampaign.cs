using Core.Features.Campaigns.Entities;
using Core.Features.Specialties.Entities;

namespace Core.Features.Interns.Entities
{
    public class InternCampaign
    {
        public Guid InternId { get; set; }
        public Intern Intern { get; set; } 
        public Guid CampaignId { get; set; }
        public Campaign Campaign { get; set; }
        public Guid SpecialityId { get; set; }
        public Speciality Speciality { get; set; }
        public ICollection<State> States { get; set; }
    }
}
