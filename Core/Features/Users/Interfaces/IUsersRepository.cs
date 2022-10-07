using Core.Common;
using Core.Features.Users.Entities;

namespace Core.Features.Users.Interfaces
{
    public interface IUsersRepository : IRepositoryBase
    {
        public Task<User> AddAsync(User user);

        public Task<bool> ExistsByEmailAsync(string email);
    }
}