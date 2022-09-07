﻿using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.RequestModels;

namespace Core.Features.Campaigns.Interfaces
{
    public interface ICampaignsRepository
    {
        Task<Campaign> AddAsync(Campaign model);

        Task<bool> ExistsByNameAsync(string name);

        Task<Campaign> UpdateAsync(Campaign model);

        Task<Campaign?> GetByIdAsync(Guid campaignId);

        Task<IEnumerable<Campaign>> GetAllAsync(PaginationFilterRequest filter);

        Task<int> GetCountAsync();
    }
}