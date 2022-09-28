namespace Core.Features.Interns.Entities
{
    public class State
    {
        public Guid Id { get; set; }
        public string Justification { get; set; }
        public DateTime Created { get; set; }
        public StatusEnum StatusId { get; set; }
        public Status Status { get; set; }
        public InternCampaign InternCampaign { get; set; }
        public Guid InternId { get ; set; }
        public Guid CampaignId { get; set; }
    }
}
