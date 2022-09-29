using Core.Common.Pagination;
using Core.Features.LearningTopics.RequestModels;
using Core.Features.LearningTopics.ResponseModels;
namespace Core.Features.LearningTopics.Interfaces
{
    public interface ILearningTopicsService
    {
        Task<LearningTopicSummaryResponse> CreateAsync(CreateLearningTopicRequest createLearningTopic);

        Task<LearningTopicSummaryResponse> UpdateAsync(UpdateLearningTopicRequest updateLearningTopic);

        Task<LearningTopicSummaryResponse> GetByIdAsync(Guid id);

        Task<IEnumerable<LearningTopicSummaryResponse>> GetAllAsync();

        Task<PaginationResponse<LearningTopicSummaryResponse>> GetPaginationAsync(PaginationRequest filter);
    }
}