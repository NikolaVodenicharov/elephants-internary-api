using Core.Common;
using Core.Common.Pagination;
using Core.Features.Interns.Entities;
using Core.Features.Interns.ResponseModels;

namespace Core.Features.Interns.Interfaces
{
    public interface IInternsRepository : IRepositoryBase
    {
        Task<InternSummaryResponse> AddAsync(Intern intern);

        Task<bool> ExistsByEmailAsync(string email);

        Task<Intern?> GetByIdAsync(Guid Id);

        Task<InternDetailsResponse?> GetDetailsByIdAsync(Guid id);
        
        Task<PaginationResponse<InternSummaryResponse>> GetAllAsync(PaginationRequest paginationRequest);

        Task<PaginationResponse<InternByCampaignSummaryResponse>> GetAllByCampaignIdAsync(PaginationRequest paginationRequest, Guid campaignId);

        Task<InternCampaign?> GetInternCampaignByIdsAsync(Guid internId, Guid campaignId);

        Task<IEnumerable<StatusResponse>> GetAllStatusAsync();
    }
}
