using Core.Common;
using Core.Common.Pagination;
using Core.Features.LearningTopics.Entities;
namespace Core.Features.LearningTopics.Interfaces
{
    public interface ILearningTopicsRepository : IRepositoryBase
    {
        Task<LearningTopic> AddAsync(LearningTopic learningTopic);

        Task<LearningTopic?> GetByIdAsync(Guid id);

        Task<IEnumerable<LearningTopic>> GetAllAsync(PaginationRequest? filter = null);

        Task<bool> ExistsByNameAsync(string name);

        Task<int> GetCountAsync();
    }
}