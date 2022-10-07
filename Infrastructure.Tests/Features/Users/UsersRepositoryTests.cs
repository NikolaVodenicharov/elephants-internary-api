using Core.Features.Users.Entities;
using Core.Features.Users.Interfaces;
using Infrastructure.Features.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Features.Users
{
    public class UsersRepositoryTests
    {
        private readonly InternaryContext internaryContext;
        private readonly string email = "john.doe@test.com";
        private readonly Guid mentorId = Guid.NewGuid();
        private readonly IUsersRepository usersRepository;

        public UsersRepositoryTests()
        {
            DbContextOptionsBuilder<InternaryContext>? dbOptions = new DbContextOptionsBuilder<InternaryContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString());
            
            internaryContext = new InternaryContext(dbOptions.Options);

            usersRepository = new UsersRepository(internaryContext);
        }

        [Fact]
        public async Task AddAsync_AddingMentorUserWithId_ShouldBeAddedToDatabase()
        {
            // Arrange
            var user = new User()
            {
                Email = email,
                RoleId = RoleEnum.Mentor,
                MentorId = mentorId
            };

            // Act
            var expectedUser = await usersRepository.AddAsync(user);
            
            // Assert
            Assert.Equal(email, expectedUser.Email);
            Assert.Equal(RoleEnum.Mentor, expectedUser.RoleId);
            Assert.Equal(mentorId, expectedUser.MentorId);
        }

        [Fact]
        public async Task AddAsync_AddingAdminUserWithoutId_ShouldBeAddedToDatabase()
        {
            // Arrange
            var user = new User()
            {
                Email = email,
                RoleId = RoleEnum.Administrator
            };

            // Act
            var expectedUser = await usersRepository.AddAsync(user);
            
            // Assert
            Assert.Equal(email, expectedUser.Email);
            Assert.Equal(RoleEnum.Administrator, expectedUser.RoleId);
        }

        [Fact]
        public async Task ExistsByEmail_WhenEmailExists_ShouldReturnTrue()
        {
            // Arrange
            var user = new User()
            {
                Email = email,
                RoleId = RoleEnum.Mentor,
                MentorId = mentorId
            };

            var existingUser = await usersRepository.AddAsync(user);

            // Act
            var exists = await usersRepository.ExistsByEmailAsync(email);
        
            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsByEmail_WhenEmailDoesNotExist_ShouldReturnFalse()
        {

            // Act
            var exists = await usersRepository.ExistsByEmailAsync(email);

            // Assert
            Assert.False(exists);
        }
    }
}