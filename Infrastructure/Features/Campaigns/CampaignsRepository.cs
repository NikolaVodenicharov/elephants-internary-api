using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Microsoft.EntityFrameworkCore;
using System.Text;

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

        public async Task<IEnumerable<Campaign>> GetAllAsync(PaginationFilterRequest filter)
        {
            var sqlQuery =
                $"SELECT [Id], [Name], [StartDate], [EndDate], [IsActive] " +
                $"FROM [Campaigns] " +
                $"ORDER BY [IsActive] DESC, [EndDate], [Id] " +
                $"OFFSET {filter.Skip} ROWS FETCH NEXT {filter.Take} ROWS ONLY";

            var campaigns = await context.Campaigns
                .FromSqlRaw(sqlQuery)
                .ToListAsync();

            return campaigns;
        }

        public async Task<Campaign?> GetByIdAsync(Guid campaignId)
        {
            var campaign = await context.Campaigns.FirstOrDefaultAsync(c => c.Id == campaignId);

            return campaign;
        }

        public async Task<int> GetCountAsync()
        {
            var campaignCount = await context.Campaigns.CountAsync();

            return campaignCount;
        }
    }
}
