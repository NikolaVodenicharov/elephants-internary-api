using Core.Common.Pagination;
using Core.Features.Interns.Entities;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;

namespace Core.Features.Interns.Interfaces
{
    public interface IInternsService
    {
        Task<InternSummaryResponse> CreateAsync(CreateInternRequest createInternRequest);

        Task<InternSummaryResponse> InviteAsync(InviteInternRequest inviteInternRequest);

        Task<InternSummaryResponse> UpdateAsync(UpdateInternRequest updateInternRequest);

        Task<IEnumerable<InternListingResponse>> GetAllAsync();

        Task<PaginationResponse<InternListingResponse>> GetPaginationAsync(PaginationRequest paginationRequest);

        Task<PaginationResponse<InternSummaryResponse>> GetAllByCampaignIdAsync(PaginationRequest paginationRequest, Guid campaignId);

        Task<InternDetailsResponse> GetDetailsByIdAsync(Guid id);  
    }
}
