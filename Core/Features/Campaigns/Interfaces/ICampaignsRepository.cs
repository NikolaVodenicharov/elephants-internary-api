using Core.Common;
using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;

namespace Core.Features.Campaigns.Interfaces
{
    public interface ICampaignsRepository : IRepositoryBase
    {
        Task<Campaign> AddAsync(Campaign model);

        Task<bool> ExistsByNameAsync(string name);

        Task<Campaign?> GetByIdAsync(Guid campaignId);

        Task<IEnumerable<Campaign>> GetAllAsync(PaginationFilterRequest filter);

        Task<int> GetCountAsync();
    }
}
