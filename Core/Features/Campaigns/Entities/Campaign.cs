using Core.Features.Mentors.Entities;

using Core.Features.Interns.Entities;

namespace Core.Features.Campaigns.Entities
{
    public class Campaign
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Mentor> Mentors { get; set; }

        public IEnumerable<InternCampaign> InternCampaigns { get; set; }
    }
}
