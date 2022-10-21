using Core.Common.Pagination;
using Core.Features.Admins.RequestModels;
using Core.Features.Admins.ResponseModels;

namespace Core.Features.Admins.Interfaces
{
    public interface IAdminsRepository
    {
        Task<AdminSummaryResponse> CreateAsync(CreateAdminRepoRequest createAdminRequest);

        Task<AdminSummaryResponse?> GetByIdAsync(Guid id);

        Task<IEnumerable<AdminSummaryResponse>> GetAllAsync(PaginationRequest filter);

        Task<int> GetCountAsync();

        Task<bool> ExistsByEmailAsync(string email);
    }
}