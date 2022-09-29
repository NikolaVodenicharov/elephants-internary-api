using Core.Common;
using Core.Common.Pagination;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;

namespace Core.Features.Mentors.Interfaces
{
    public interface IMentorsService
    {
        public Task<MentorSummaryResponse> CreateAsync(CreateMentorRequest request);

        public Task<MentorSummaryResponse> UpdateAsync(UpdateMentorRequest request);

        public Task<MentorSummaryResponse> GetByIdAsync(Guid id);

        public Task<PaginationResponse<MentorSummaryResponse>> GetPaginationAsync(PaginationRequest filter, Guid? campaignId = null);

        public Task<IEnumerable<MentorSummaryResponse>> GetAllAsync();

        public Task<int> GetCountByCampaignIdAsync(Guid campaignId);

        public Task<int> GetCountAsync();

        public Task AddToCampaignAsync(AddToCampaignRequest request);
    }
}
