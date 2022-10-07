using Core.Features.Users.RequestModels;
using Core.Features.Users.ResponseModels;
using Core.Features.Identity.ResponseModels;

namespace Core.Features.Users.Interfaces
{
    public interface IUsersService
    {
        Task<UserSummaryResponse> CreateAsync(CreateUserRequest request);
        
        Task<bool> ExistsByEmailAsync(string email);

        Task<IdentitySummaryResponse> SendInvitationByEmailAsync(string email, string applicationUrl);
    }
}