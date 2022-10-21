using Core.Features.Persons.ResponseModels;

namespace Core.Features.Persons.Interfaces
{
    public interface IPersonsService
    {
        Task<PersonRolesSummaryResponse?> GetUserRolesByEmailAsync(string email);

        Task<PersonRolesSummaryResponse?> CreatePersonAsAdminAsync(string email, string displayName);
    }
}