using Core.Common;
using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;

namespace Core.Features.Mentors.Interfaces
{
    public interface IMentorsRepository
    {
        public Task<MentorSummaryResponse> CreateAsync(CreateMentorRepoRequest createMentorRepoRequest);

        public Task<bool> AddToCampaignAsync(AddMentorToCampaignRepoRequest addMentorToCampaignRepoRequest);

        public Task<MentorDetailsResponse?> GetByIdAsync(Guid id);

        public Task<MentorDetailsResponse?> UpdateAsync(UpdateMentorRepoRequest updateMentorRepoRequest);

        public Task<IEnumerable<MentorPaginationResponse>> GetAllAsync(PaginationRequest? filter = null, Guid? campaignId = null);

        public Task<int> GetCountByCampaignIdAsync(Guid campaignId);

        public Task<int> GetCountAsync();

        public Task<bool> IsEmailUsed(string email);

        public Task<bool> RemoveFromCampaignAsync(Guid mentorId, Campaign campaign);
    }
}
