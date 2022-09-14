namespace Core.Features.Campaigns.RequestModels
{
    public class UpdateCampaignRequest
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public bool IsActive { get; private set; }

        public UpdateCampaignRequest(Guid id, string name, DateTime startDate, DateTime endDate, bool isActive)
        {
            Id = id;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = isActive;
        }
    }

}