using Core.Common.Pagination;
using Core.Features.Admins.Interfaces;
using Core.Features.Admins.RequestModels;
using Core.Features.Admins.ResponseModels;
using Core.Features.Persons.Entities;
using Infrastructure.Features.Persons.Support;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Admins
{
    public class AdminsRepository : IAdminsRepository
    {
        private readonly InternaryContext context;

        public AdminsRepository(InternaryContext context)
        {
            this.context = context;
        }

        public async Task<AdminSummaryResponse> CreateAsync(CreateAdminRepoRequest createAdminRequest)
        {
            var person = new Person()
            {
                DisplayName = createAdminRequest.DisplayName,
                WorkEmail = createAdminRequest.WorkEmail
            };

            var personRole = new PersonRole()
            {
                RoleId = RoleId.Administrator,
                Person = person
            };

            await context
                .PersonRoles
                .AddAsync(personRole);
            
            await context.SaveChangesAsync();

            return person.ToAdminSummaryResponse();
        }

        public async Task<AdminSummaryResponse?> GetByIdAsync(Guid id)
        {
            var admin = await context
                .Persons
                .Where(p => 
                    p.Id == id && 
                    p.PersonRoles.Any(r => r.RoleId == RoleId.Administrator))
                .FirstOrDefaultAsync();
            
            return admin?.ToAdminSummaryResponse();
        }

        public async Task<IEnumerable<AdminSummaryResponse>> GetAllAsync(PaginationRequest filter)
        {
            var skip = (filter.PageNum!.Value - 1) * filter.PageSize!.Value;
            var take = filter.PageSize.Value;

            var admins = await context
                .Persons
                .AsNoTracking()
                .Where(p => p.PersonRoles.Any(r => r.RoleId == RoleId.Administrator))
                .OrderByDescending(s => EF.Property<DateTime>(s, "CreatedDate"))
                .Select(p => p.ToAdminSummaryResponse())
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            
            return admins;
        }

        public async Task<int> GetCountAsync()
        {
            var adminCount = await context
                .Persons
                .Where(p => p.PersonRoles.Any(r => r.RoleId == RoleId.Administrator))
                .CountAsync();
            
            return adminCount;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await context
                .Persons
                .AnyAsync(p => p.WorkEmail == email);
        }
    }
}