using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialities.Support;
using Core.Features.Specialties.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Specialities
{
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

        public async Task<SpecialitySummaryResponse> UpdateAync(Speciality speciality)
        {
            await context.SaveChangesAsync();

            return speciality.ToSpecialitySummaryResponse();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            var exist = await context
                .Specialties
                .AnyAsync(s => s.Name == name);

            return exist;
        }

        public async Task<IEnumerable<SpecialitySummaryResponse>> GetAllAsync()
        {
            var specialitySummaries = await context
                .Specialties
                .Select(s => s.ToSpecialitySummaryResponse())
                .ToListAsync();

            return specialitySummaries;
        }

        public async Task<Speciality?> GetByIdAsync(Guid id)
        {
            var speciality = await context.Specialties.FirstOrDefaultAsync(s => s.Id == id);

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
    }
}
