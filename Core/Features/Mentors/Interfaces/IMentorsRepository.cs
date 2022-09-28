using Core.Common;
using Core.Common.Pagination;
using Core.Features.Mentors.Entities;
using Core.Features.Mentors.ResponseModels;

namespace Core.Features.Mentors.Interfaces
{
    public interface IMentorsRepository : IRepositoryBase
    {
        public Task<Mentor> AddAsync(Mentor mentor);

        public Task<Mentor?> GetByIdAsync(Guid id);

        public Task<IEnumerable<Mentor>> GetAllAsync(PaginationRequest filter, Guid? campaignId = null);

        public Task<int> GetCountByCampaignIdAsync(Guid campaignId);

        public Task<int> GetCountAsync();

        public Task<bool> IsEmailUsed(string email);
    }
}
