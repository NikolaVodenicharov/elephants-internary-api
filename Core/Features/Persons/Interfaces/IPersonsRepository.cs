using Core.Features.Persons.ResponseModels;

namespace Core.Features.Persons.Interfaces
{
    public interface IPersonsRepository
    {
        Task<PersonRolesSummaryResponse?> GetPersonRolesByEmailAsync(string email);
    }
}