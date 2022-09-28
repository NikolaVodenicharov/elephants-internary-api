using Core.Common.Pagination;
using Core.Features.Mentors.Entities;
using Core.Features.Mentors.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Mentors
{
    public class MentorsRepository : IMentorsRepository
    {
        private readonly InternaryContext context;

        public MentorsRepository(InternaryContext context)
        {
            this.context = context;
        }

        public async Task<Mentor> AddAsync(Mentor mentor)
        {
            await context.Mentors.AddAsync(mentor);

            await context.SaveChangesAsync();

            return mentor;
        }

        public async Task<Mentor?> GetByIdAsync(Guid id)
        {
            var mentor = await context.Mentors
                .Include(m => m.Specialities)
                .FirstOrDefaultAsync(m => m.Id == id);

            return mentor;
        }

        public async Task<int> GetCountAsync()
        {
            var mentorCount = await context.Mentors.CountAsync();

            return mentorCount;
        }

        public async Task<bool> IsEmailUsed(string email)
        {
            return await context.Mentors.AnyAsync(mentor => mentor.Email.Equals(email));
        }

        public async Task SaveTrackingChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Mentor>> GetAllAsync(PaginationRequest filter, Guid? campaignId)
        {
            var skip = (filter.PageNum.Value - 1) * filter.PageSize.Value;

            var mentors = await context.Mentors
                .AsNoTracking()
                .Where(m => campaignId != null ? m.Campaigns.Any(c => c.Id == campaignId) : true)
                .Include(m => m.Specialities)
                .OrderBy(m => m.Id)
                .Skip(skip)
                .Take(filter.PageSize.Value)
                .ToListAsync();

            return mentors;
        }

        public async Task<int> GetCountByCampaignIdAsync(Guid campaignId)
        {
            var count = await context.Mentors
                .Where(m => m.Campaigns.Any(c => c.Id == campaignId))
                .CountAsync();

            return count;
        }
    }
}
