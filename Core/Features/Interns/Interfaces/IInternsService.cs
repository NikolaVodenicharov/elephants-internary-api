using Core.Common.Pagination;
using Core.Features.Interns.Entities;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;

namespace Core.Features.Interns.Interfaces
{
    public interface IInternsService
    {
        Task<InternSummaryResponse> CreateAsync(CreateInternRequest createInternRequest);

        Task<InternSummaryResponse> UpdateAsync(UpdateInternRequest updateInternRequest);

        Task<PaginationResponse<InternSummaryResponse>> GetAllAsync(PaginationRequest paginationRequest);

        Task<PaginationResponse<InternByCampaignSummaryResponse>> GetAllByCampaignIdAsync(PaginationRequest paginationRequest, Guid campaignId);

        Task<InternDetailsResponse> GetDetailsByIdAsync(Guid id);  
    }
}
