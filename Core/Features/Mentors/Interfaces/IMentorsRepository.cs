using Core.Common.Pagination;
using Core.Features.Mentors.Entities;

namespace Core.Features.Mentors.Interfaces
{
    public interface IMentorsRepository
    {
        public Task<Mentor> AddAsync(Mentor mentor);

        public Task<Mentor> UpdateAsync(Mentor mentor);

        public Task<Mentor?> GetByIdAsync(Guid id);

        public Task<IEnumerable<Mentor>> GetAllAsync(PaginationFilterRequest filter);

        public Task<IEnumerable<Mentor>> GetMentorsByCampaignIdAsync(Guid campaignId, PaginationFilterRequest filter);

        public Task<int> GetCountByCampaignIdAsync(Guid campaignId);

        public Task<int> GetCountAsync();

        public Task<bool> IsEmailUsed(string email);
    }
}
