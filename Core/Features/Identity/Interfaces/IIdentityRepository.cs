using Core.Features.Identity.ResponseModels;

namespace Core.Features.Identity.Interfaces
{
    public interface IIdentityRepository
    {
        Task<IdentitySummaryResponse> SendUserInviteAsync(string userEmail, string applicationUrl);
    }
}