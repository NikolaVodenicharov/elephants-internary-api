using Core.Common;
using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.ResponseModels;

namespace Core.Features.Campaigns.Interfaces
{
    public interface ICampaignsRepository : IRepositoryBase
    {
        Task<Campaign> AddAsync(Campaign model);

        Task<bool> ExistsByNameAsync(string name);

        Task<Campaign?> GetByIdAsync(Guid campaignId);

        Task<IEnumerable<Campaign>> GetAllAsync(PaginationRequest? filter = null);

        Task<int> GetCountAsync();
    }
}
