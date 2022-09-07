using Core.Features.Campaigns.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Features.Campaigns.Interfaces
{
    public interface ICampaignsRepository
    {
        Task<Campaign> AddAsync(Campaign model);

        Task<bool> ExistsByNameAsync(string name);

        Task<Campaign> UpdateAsync(Campaign model);

        Task<Campaign?> GetByIdAsync(Guid campaignId);
    }
}
