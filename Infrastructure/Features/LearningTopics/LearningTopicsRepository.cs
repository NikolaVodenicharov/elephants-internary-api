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

        public async Task<IEnumerable<LearningTopic>> GetAllAsync()
        {
            var learningTopics = await context.LearningTopics
                .Include(t => t.Specialities)
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
    }
}