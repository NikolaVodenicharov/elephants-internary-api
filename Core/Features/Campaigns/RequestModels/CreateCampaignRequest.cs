namespace Core.Features.Campaigns.RequestModels
{
    public class CreateCampaignRequest
    {
        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public bool IsActive { get; private set; }

        public CreateCampaignRequest(string name, DateTime startDate, DateTime endDate, bool isActive)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = isActive;
        }
    }
}
