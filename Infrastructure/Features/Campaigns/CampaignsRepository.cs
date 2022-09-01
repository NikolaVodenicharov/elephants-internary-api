using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
