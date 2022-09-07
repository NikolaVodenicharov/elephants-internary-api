using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Campaigns
{
    public class CampaignsRepository : ICampaignsRepository
    {
        private readonly InternaryContext context;

        public CampaignsRepository(InternaryContext context)
        {
            this.context = context;
        }

        public async Task<Campaign> AddAsync(Campaign model)
        {
            await context.Campaigns.AddAsync(model);

            await context.SaveChangesAsync();

            return model;
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await context.Campaigns.AnyAsync(campaign => campaign.Name.Equals(name));
        }
        
        public async Task<Campaign> UpdateAsync(Campaign model)
        {
            context.Campaigns.Update(model);

            await context.SaveChangesAsync();

            return model;
        }

        public async Task<Campaign?> GetByIdAsync(Guid campaignId)
        {
            var campaign = await context.Campaigns.FirstOrDefaultAsync(c => c.Id == campaignId);

            return campaign;
        }
    }
}
