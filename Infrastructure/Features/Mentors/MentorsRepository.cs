﻿using Core.Common.Pagination;
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

        public async Task<IEnumerable<Mentor>> GetAllAsync(PaginationFilterRequest filter)
        {
            var mentors = await context.Mentors
                .Include(m => m.Specialities)
                .OrderBy(m => m.Id)
                .Skip(filter.Skip)
                .Take(filter.Take)
                .ToListAsync();

            return mentors;
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

        public async Task<Mentor> UpdateAsync(Mentor mentor)
        {
            await context.SaveChangesAsync();

            return mentor;
        }

        public async Task<IEnumerable<Mentor>> GetMentorsByCampaignIdAsync(Guid campaignId, PaginationFilterRequest filter)
        {
            var mentors = await context.Mentors
                .Where(m => m.Campaigns.Any(c => c.Id == campaignId))
                .Include(m => m.Specialities)
                .OrderBy(m => m.Id)
                .Skip(filter.Skip)
                .Take(filter.Take)
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