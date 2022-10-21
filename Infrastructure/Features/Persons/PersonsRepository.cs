using Core.Features.Persons.Interfaces;
using Core.Features.Persons.ResponseModels;
using Infrastructure.Features.Persons.Support;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Persons
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly InternaryContext context;

        public PersonsRepository(InternaryContext context)
        {
            this.context = context;
        }

        public async Task<PersonRolesSummaryResponse?> GetPersonRolesByEmailAsync(string email)
        {
            var user = await context
                .Persons
                .Include(p => p.PersonRoles)
                .Where(p => p.WorkEmail == email)
                .FirstOrDefaultAsync();
            
            return user != null ? user.ToPersonRolesSummaryResponse() : null;
        }
    }
}