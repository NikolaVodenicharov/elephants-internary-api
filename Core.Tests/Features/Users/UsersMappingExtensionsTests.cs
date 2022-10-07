using Core.Features.Users.Entities;
using Core.Features.Users.RequestModels;
using Core.Features.Users.Support;
using System;
using System.Collections.Generic;
using Xunit;

namespace Core.Tests.Features.Users
{
    public class UsersMappingExtensionsTests
    {
        private readonly string userEmail = "user.example@test.com";
        private readonly Guid userId = Guid.NewGuid();

        public static IEnumerable<object[]> userRolesTestData =>
            new List<object[]>
            {
                new object[] { RoleEnum.Mentor, Guid.NewGuid() },
                new object[] { RoleEnum.Administrator, Guid.Empty },
                new object[] { RoleEnum.Intern, Guid.Empty },
            };

        [Theory]
        [MemberData(nameof(userRolesTestData))]
        public void CreateUserRequest_ToUser_CreateCorrectObject(RoleEnum role, Guid personId)
        {
            // Arrange
            var createUserRequest = new CreateUserRequest(userEmail, role, personId);
            
            // Act
            var user = createUserRequest.ToUser();

            // Assert
            Assert.Equal(userEmail, user.Email);
            Assert.Equal(role, user.RoleId);
            Assert.Equal(personId, user.MentorId);
        }
        
        [Theory]
        [MemberData(nameof(userRolesTestData))]
        public void User_ToUserSummary_CreateCorrectObject(RoleEnum role, Guid personId)
        {
            // Arrange
            var user = new User()
            {
                Id = userId, 
                Email = userEmail, 
                RoleId = role, 
                MentorId = personId
            };
            
            // Act
            var userSummary = user.ToUserSummary();

            // Assert
            Assert.Equal(userId, userSummary.Id);
            Assert.Equal(userEmail, userSummary.Email);
            Assert.Equal(role, userSummary.Role);
        }
    }
}