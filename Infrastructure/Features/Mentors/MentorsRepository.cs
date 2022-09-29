using Core.Common.Pagination;
using Core.Features.Mentors.Entities;
using Core.Features.Mentors.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Mentors
{
    internal static class Counter
    {
        public static int mentorsCount = -1;
    }

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
                .Include(m => m.Campaigns)
                .Include(m => m.Specialities)
                .FirstOrDefaultAsync(m => m.Id == id);

            return mentor;
        }

        public async Task<int> GetCountAsync()
        {
            var mentorCount = await context.Mentors.CountAsync();

            Counter.mentorsCount = mentorCount;

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

        public async Task<IEnumerable<Mentor>> GetAllAsync(PaginationRequest? filter = null, Guid? campaignId = null)
        {
            if (Counter.mentorsCount == -1 || filter?.PageNum == 1)
            {
                await GetCountAsync();
            }

            var skip = filter != null ? (filter.PageNum.Value - 1) * filter.PageSize.Value : 0;
            var take = filter != null ? filter.PageSize.Value : await GetCountAsync();

            var mentors = await context.Mentors
                .AsNoTracking()
                .Where(m => campaignId != null ? m.Campaigns.Any(c => c.Id == campaignId) : true)
                .Include(m => m.Specialities)
                .OrderBy(m => m.Id)
                .Skip(skip)
                .Take(take)
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
