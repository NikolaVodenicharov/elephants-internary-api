﻿using Core.Common.Pagination;
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
            int skip = 0, take = 0;

            if (filter != null)
            {
                skip = (filter.PageNum!.Value - 1) * filter.PageSize!.Value;
                take = filter.PageSize!.Value;
            }
            else
            {
                take = await GetCountAsync();
            }

            var specialities = await context
                .Specialties
                .AsNoTracking()
                .OrderByDescending(s => EF.Property<DateTime>(s, "UpdatedDate"))
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

            return count;
        }
    }
}
