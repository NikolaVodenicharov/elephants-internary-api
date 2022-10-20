using Core.Features.Interns.Entities;
using Core.Features.Persons.Entities;

namespace Core.Features.Campaigns.Entities
{
    public class Campaign
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Person> Persons { get; set; } = null!;

        public IEnumerable<InternCampaign> InternCampaigns { get; set; } = null!;
    }
}
