using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
using Core.Support.Exceptions;

namespace Core.Features.Campaigns
{
    public class CampaignsService : ICampaignsService
    {
        private readonly ICampaignsRepository repository;

        public CampaignsService(ICampaignsRepository repository)
        {
            this.repository = repository ?? throw new CoreNotImplementedException($"{nameof(ICampaignsRepository)} is not implemented.");
        }

        public Task CreateAsync(CreateCampaign model)
        {
            throw new NotImplementedException();
        }

        public Task<CampaignSummary> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignSummary>> GetAsync(GetAllCampaigns filter)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UpdateCampaign model)
        {
            throw new NotImplementedException();
        }
    }
}
