using Core.Common.Pagination;
using Core.Features.LearningTopics.Entities;
namespace Core.Features.LearningTopics.Interfaces
{
    public interface ILearningTopicsRepository
    {
        Task<LearningTopic> AddAsync(LearningTopic learningTopic);

        Task<LearningTopic> UpdateAsync(LearningTopic learningTopic);

        Task<LearningTopic?> GetByIdAsync(Guid Id);

        Task<IEnumerable<LearningTopic>> GetAllAsync();

        Task<bool> ExistsByNameAsync(string name);
    }
}