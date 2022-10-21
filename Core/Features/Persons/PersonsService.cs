using Core.Common;
using Core.Features.Admins.Interfaces;
using Core.Features.Admins.RequestModels;
using Core.Features.Persons.Interfaces;
using Core.Features.Persons.ResponseModels;
using Microsoft.Extensions.Logging;

namespace Core.Features.Persons
{
    public class PersonsService : IPersonsService
    {
        private readonly IPersonsRepository personsRepository;
        private readonly IAdminsRepository adminsRepository;
        private readonly ILogger<PersonsService> personsServiceLogger;

        public PersonsService(
            IPersonsRepository personsRepository,
            IAdminsRepository adminsRepository,
            ILogger<PersonsService> personsServiceLogger)
        {
            this.personsRepository = personsRepository;
            this.adminsRepository = adminsRepository;
            this.personsServiceLogger = personsServiceLogger;
        }

        public async Task<PersonRolesSummaryResponse?> GetUserRolesByEmailAsync(string email)
        {
            var user = await personsRepository.GetPersonRolesByEmailAsync(email);

            personsServiceLogger.LogInformationMethod(nameof(PersonsService), nameof(GetUserRolesByEmailAsync), true);

            return user;
        }

        public async Task<PersonRolesSummaryResponse?> CreatePersonAsAdminAsync(string email, string displayName)
        {
            var createAdminRequest = new CreateAdminRepoRequest(displayName, email);

            await adminsRepository.CreateAsync(createAdminRequest);

            var personRolesSummary = await personsRepository.GetPersonRolesByEmailAsync(email);

            personsServiceLogger.LogInformationMethod(nameof(PersonsService), nameof(CreatePersonAsAdminAsync), true);

            return personRolesSummary;
        }
    }
}