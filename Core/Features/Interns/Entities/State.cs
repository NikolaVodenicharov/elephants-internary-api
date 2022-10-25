namespace Core.Features.Interns.Entities
{
    public class State
    {
        public Guid Id { get; set; }
        public string Justification { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public StatusId StatusId { get; set; }
        public Status Status { get; set; } = null!;
        public InternCampaign InternCampaign { get; set; } = null!;
        public Guid InternId { get ; set; }
        public Guid CampaignId { get; set; }
    }
}
