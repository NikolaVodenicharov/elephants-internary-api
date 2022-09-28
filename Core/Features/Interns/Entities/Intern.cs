namespace Core.Features.Interns.Entities
{
    public class Intern
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PersonalEmail { get; set; } = string.Empty;
        public ICollection<InternCampaign>? InternCampaigns { get; set; }
    }
}
