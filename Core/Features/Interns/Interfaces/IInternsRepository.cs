using Core.Common;
using Core.Common.Pagination;
using Core.Features.Interns.Entities;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;

namespace Core.Features.Interns.Interfaces
{
    public interface IInternsRepository
    {
        Task<InternSummaryResponse> CreateAsync(CreateInternRepoRequest createInternWithDetailsRequest);

        Task<InternCampaignSummaryResponse?> AddInternCampaignAsync(AddInternCampaignRepoRequest addInternCampaignRepoRequest);

        Task<InternSummaryResponse?> AddIdentityAsync(AddInternIdentityRepoRequest addInternIdentityRepoRequest);

        Task<InternSummaryResponse?> UpdateAsync(UpdateInternRequest updateInternRequest);

        Task<InternSummaryResponse?> GetByIdAsync(Guid Id);

        Task<bool> ExistsByPersonalEmailAsync(string personalEmail);     

        Task<InternDetailsResponse?> GetDetailsByIdAsync(Guid id);

        Task<IEnumerable<InternListingResponse>> GetAllAsync();

        Task<PaginationResponse<InternListingResponse>> GetPaginationAsync(PaginationRequest paginationRequest);

        Task<PaginationResponse<InternSummaryResponse>> GetPaginationByCampaignIdAsync(PaginationRequest paginationRequest, Guid campaignId);

        Task<InternCampaign?> GetInternCampaignByIdsAsync(Guid internId, Guid campaignId);

        Task<IEnumerable<StatusResponse>> GetAllStatusAsync();

        Task<InternCampaignSummaryResponse> UpdateInternCampaignAsync(InternCampaign internCampaign);
    }
}
