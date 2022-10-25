using Core.Common.Pagination;
using Core.Features.Admins.RequestModels;
using Core.Features.Admins.ResponseModels;
using Core.Features.Mentors.ResponseModels;

namespace Core.Features.Admins.Interfaces
{
    public interface IAdminsService
    {
        Task<AdminSummaryResponse> CreateAsync(CreateAdminRequest request);

        Task<AdminSummaryResponse?> GetByIdAsync(Guid id);

        Task<PaginationResponse<AdminListingResponse>> GetAllAsync(PaginationRequest filter);

        Task<MentorSummaryResponse> AddAsMentorAsync(AddMentorRoleRequest addMentorRoleRequest);
    }
}