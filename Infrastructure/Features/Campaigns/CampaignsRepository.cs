using Core.Common.Pagination;
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

        public async Task<IEnumerable<Campaign>> GetAllAsync(PaginationRequest filter)
        {
            var skip = (filter.PageNum.Value - 1) * filter.PageSize.Value;

            var campaigns = await context.Campaigns
                .AsNoTracking()
                .OrderByDescending(c => c.IsActive)
                .ThenBy(c => c.EndDate)
                .ThenBy(c => c.Id)
                .Skip(skip)
                .Take(filter.PageSize.Value)
                .ToListAsync();

            return campaigns;
        }

        public async Task<Campaign?> GetByIdAsync(Guid campaignId)
        {
            var campaign = await context.Campaigns
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == campaignId);

            return campaign;
        }

        public async Task<int> GetCountAsync()
        {
            var campaignCount = await context.Campaigns.CountAsync();

            return campaignCount;
        }

        public async Task SaveTrackingChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
