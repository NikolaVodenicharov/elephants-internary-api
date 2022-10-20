using Core.Common.Pagination;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;

namespace Core.Features.Mentors.Interfaces
{
    public interface IMentorsService
    {
        public Task<MentorSummaryResponse> CreateAsync(CreateMentorRequest createMentorRequest);

        public Task<MentorDetailsResponse> UpdateAsync(UpdateMentorRequest updateMentorRequest);

        public Task<MentorDetailsResponse> GetByIdAsync(Guid id);

        public Task<PaginationResponse<MentorPaginationResponse>> GetPaginationAsync(PaginationRequest filter, Guid? campaignId = null);
        
        public Task<IEnumerable<MentorPaginationResponse>> GetAllAsync();

        public Task<int> GetCountByCampaignIdAsync(Guid campaignId);

        public Task<int> GetCountAsync();

        public Task<bool> AddToCampaignAsync(AddMentorToCampaignRequest addMentorToCampaignRequest);

        public Task<bool> RemoveFromCampaignAsync(Guid campaignId, Guid mentorId);
    }
}
