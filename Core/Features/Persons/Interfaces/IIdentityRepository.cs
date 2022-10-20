using Core.Features.Persons.ResponseModels;

namespace Core.Features.Persons.Interfaces
{
    public interface IIdentityRepository
    {
        Task<IdentitySummaryResponse> SendUserInviteAsync(string userEmail, string applicationUrl);

    }
}