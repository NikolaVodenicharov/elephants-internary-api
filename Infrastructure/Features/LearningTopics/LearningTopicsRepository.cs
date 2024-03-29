using Core.Common.Pagination;
using Core.Features.LearningTopics.Entities;
using Core.Features.LearningTopics.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.LearningTopics
{
    public class LearningTopicsRepository : ILearningTopicsRepository
    {
        private readonly InternaryContext context;

        public LearningTopicsRepository(InternaryContext context)
        {
            this.context = context;
        }

        public async Task<LearningTopic> AddAsync(LearningTopic learningTopic)
        {
            await context.LearningTopics.AddAsync(learningTopic);

            await context.SaveChangesAsync();

            return learningTopic;
        }

        public async Task<LearningTopic?> GetByIdAsync(Guid id)
        {
            var learningTopic = await context.LearningTopics
                .Include(t => t.Specialities)
                .FirstOrDefaultAsync(t => t.Id == id);

            return learningTopic;
        }

        public async Task<IEnumerable<LearningTopic>> GetAllAsync(PaginationRequest? filter = null)
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

            var learningTopics = await context
                .LearningTopics
                .AsNoTracking()
                .Include(t => t.Specialities)
                .OrderByDescending(s => EF.Property<DateTime>(s, "UpdatedDate"))
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return learningTopics;
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            var existsByName = await context.LearningTopics.AnyAsync(t => t.Name.Equals(name));

            return existsByName;
        }

        public async Task SaveTrackingChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync()
        {
            var count = await context.LearningTopics.CountAsync();

            return count;
        }
    }
}