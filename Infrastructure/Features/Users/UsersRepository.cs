using Core.Features.Users.Interfaces;
using Core.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Users
{
    public class UsersRepository : IUsersRepository
    {
        private readonly InternaryContext context;

        public UsersRepository(InternaryContext context)
        {
            this.context = context;
        }

        public async Task<User> AddAsync(User user)
        {
            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var userExists = await context.Users.AnyAsync(u => u.Email == email);

            return userExists;
        }

        public async Task SaveTrackingChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}