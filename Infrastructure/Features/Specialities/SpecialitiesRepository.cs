using Core.Common.Pagination;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialities.Support;
using Core.Features.Specialties.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Specialities
{
    internal static class Counter
    {
        public static int specialitiesCount = -1;
    }

    public class SpecialitiesRepository : ISpecialitiesRepository
    {
        private readonly InternaryContext context;

        public SpecialitiesRepository(InternaryContext context)
        {
            this.context = context;
        }

        public async Task<SpecialitySummaryResponse> AddAsync(Speciality speciality)
        {
            await context.Specialties.AddAsync(speciality);

            await context.SaveChangesAsync();

            return speciality.ToSpecialitySummaryResponse();
        }

        public async Task SaveTrackingChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            var exist = await context
                .Specialties
                .AsNoTracking()
                .AnyAsync(s => s.Name == name);

            return exist;
        }

        public async Task<IEnumerable<SpecialitySummaryResponse>> GetAllAsync(PaginationRequest? filter = null)
        {
            if (Counter.specialitiesCount == -1 || filter?.PageNum == 1)
            {
               await GetCountAsync();
            }

            var skip = filter != null ? (filter.PageNum.Value - 1) * filter.PageSize.Value : 0;
            var take = filter != null ? filter.PageSize.Value : await GetCountAsync();

            var specialities = await context
                .Specialties
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return specialities.ToSpecialitySummaryResponses();
        }

        public async Task<Speciality?> GetByIdAsync(Guid id)
        {
            var speciality = await context.Specialties
                .FirstOrDefaultAsync(s => s.Id == id);

            return speciality;
        }

        public async Task<bool> IsNameTakenByOtherAsync(string name, Guid updatedCampaignId)
        {
            var isNameTakenByOther = await context
                .Specialties
                .AnyAsync(s => s.Name == name && s.Id != updatedCampaignId);

            return isNameTakenByOther;
        }

        public async Task<ICollection<Speciality>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            var specialities = await context.Specialties
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();

            return specialities;
        }

        public async Task<int> GetCountAsync()
        {
            var count = await context.Specialties.CountAsync();

            Counter.specialitiesCount = count;

            return count;
        }
    }
}
