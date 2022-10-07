using Core.Features.Identity.Interfaces;
using Core.Features.Identity.ResponseModels;
using Core.Features.Users;
using Core.Features.Users.Entities;
using Core.Features.Users.Interfaces;
using Core.Features.Users.RequestModels;
using Core.Features.Users.Support;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Features.Users
{
    public class UsersServiceTests
    {
        private readonly Mock<IUsersRepository> usersRepositoryMock;
        private readonly Mock<IIdentityRepository> identityRepositoryMock;
        private readonly UsersService usersService;
        private readonly string userEmail = "user.example@test.com";
        private readonly Guid mentorId = Guid.NewGuid();
        private readonly RoleEnum mentorUserRole = RoleEnum.Mentor;
        private readonly string displayName = "User Example";
        private readonly string applicationUrl = "https://test.com";
        
        public static IEnumerable<object[]> invalidEmails = new List<object[]>
        {
            new object[] { "k.c.a" },
            new object[] { ".invalid@example.c" },
            new object[] { "invalid@example..com" },
            new object[] { "invalid@example.com." },
            new object[] { "invalidexample" },
            new object[] { "invalidexample.com" },
            new object[] { "invalidexample.co.uk." },
            new object[] { "invalidexample.co_uk" },
            new object[] { "invalidexample.co_ukkkk" },
        };

        public UsersServiceTests()
        {
            usersRepositoryMock = new Mock<IUsersRepository>();
            identityRepositoryMock = new Mock<IIdentityRepository>();

            var createUserRequestValidator = new CreateUserRequestValidator();

            usersService = new UsersService(
                usersRepositoryMock.Object,
                identityRepositoryMock.Object,
                createUserRequestValidator
            );
        }

        [Fact]
        public async Task CreateAsync_WhenRequestDataIsValid_ShouldReturnCorrectObject()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest(userEmail, mentorUserRole, mentorId);

            var user = new User()
            {
                Id = Guid.NewGuid(), 
                Email = userEmail, 
                RoleId = mentorUserRole, 
                MentorId = mentorId
            };

            usersRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(user);
            
            // Act
            var userSummary = await usersService.CreateAsync(createUserRequest);

            // Assert
            Assert.Equal(user.Id, userSummary.Id);
            Assert.Equal(user.Email, userSummary.Email);
            Assert.Equal(user.RoleId, userSummary.Role);
        }

        [Theory]
        [MemberData(nameof(invalidEmails))]
        public async Task CreateAsync_WhenEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {
            // Arrange
            var createUserRequest = new CreateUserRequest(invalidEmail, mentorUserRole, mentorId);
            
            // Act
            var action = async () => await usersService.CreateAsync(createUserRequest);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]   
        public async Task CreateAsync_WhenMentorIdIsNotProvided_ShouldThrowException()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest(userEmail, mentorUserRole, Guid.Empty);
            
            // Act
            var action = async () => await usersService.CreateAsync(createUserRequest);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task SendInvitationByEmailAsync_WhenEmailExists_ShouldReturnCorrectObject()
        {
            // Arrange
            var identitySummary = new IdentitySummaryResponse(userEmail, displayName);

            identityRepositoryMock
                .Setup(x => x.SendUserInviteAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(identitySummary);

            // Act
            var identityResponse = await usersService.SendInvitationByEmailAsync(userEmail, applicationUrl);

            // Assert
            Assert.Equal(identitySummary.Email, identityResponse.Email);
            Assert.Equal(identitySummary.DisplayName, identityResponse.DisplayName);
        }

        [Fact]
        public async Task ExistsByEmailAsync_WhenEmailDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            usersRepositoryMock
                .Setup(x => x.ExistsByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            // Act
            var exists = await usersService.ExistsByEmailAsync(userEmail);
            
            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ExistsByEmailAsync_WhenEmailExists_ShouldReturnTrue()
        {
            // Arrange
            usersRepositoryMock
                .Setup(x => x.ExistsByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
          
            // Act
            var exists = await usersService.ExistsByEmailAsync(userEmail);

            // Assert
            Assert.True(exists);
        }
    }
}