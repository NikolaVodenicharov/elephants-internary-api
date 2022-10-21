using Core.Features.Admins.Interfaces;
using Core.Features.Admins.RequestModels;
using Core.Features.Admins.ResponseModels;
using Core.Features.Persons;
using Core.Features.Persons.Entities;
using Core.Features.Persons.Interfaces;
using Core.Features.Persons.ResponseModels;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Features.Persons
{
    public class PersonsServiceTests
    {
        private readonly Mock<IPersonsRepository> personsRepositoryMock;
        private readonly Mock<IAdminsRepository> adminRepositoryMock;
        private readonly PersonsService personsService;
        private readonly Guid personId = Guid.NewGuid();
        private readonly string personEmail = "john.doe@test.com";
        private readonly string personDisplayName = "John Doe";

        public PersonsServiceTests()
        {
            var personsServiceLoggerMock = new Mock<ILogger<PersonsService>>();
            
            personsRepositoryMock = new Mock<IPersonsRepository>();
            adminRepositoryMock = new Mock<IAdminsRepository>();

            personsService = new PersonsService(
                personsRepositoryMock.Object,
                adminRepositoryMock.Object,
                personsServiceLoggerMock.Object
            );
        }

        [Fact]
        public async Task GetUserRolesByEmailAsync_WhenDataIsValid_ShouldReturnCorrectObject()
        {
            // Arrange
            var personWithMultipleRoles = new PersonRolesSummaryResponse(
                personId,
                new List<RoleId>() { RoleId.Administrator, RoleId.Mentor }
            );

            personsRepositoryMock
                .Setup(p => p.GetPersonRolesByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(personWithMultipleRoles);

            // Act
            var personRolesSummary = await personsService.GetUserRolesByEmailAsync(personEmail);

            // Assert
            Assert.NotNull(personRolesSummary);
            Assert.Equal(personWithMultipleRoles.Id, personRolesSummary?.Id);
            Assert.Equal(personWithMultipleRoles.Roles.Count, personRolesSummary?.Roles.Count);
        }

        [Fact]
        public async Task GetUserRolesByEmailAsync_WhenUserNotFound_ShouldReturnEmptyObject()
        {
            // Act
            var personRolesSummary = await personsService.GetUserRolesByEmailAsync(personEmail);

            // Assert
            Assert.Null(personRolesSummary);
        }

        [Fact]
        public async Task GetUserRolesByEmailAsync_WhenUserDoesNotHaveRoles_ShouldReturnEmptyList()
        {
            // Arrange
            var personWithNoRoles = new PersonRolesSummaryResponse(
                personId,
                new List<RoleId>()
            );

            personsRepositoryMock
                .Setup(p => p.GetPersonRolesByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(personWithNoRoles);

            // Act
            var personRolesSummary = await personsService.GetUserRolesByEmailAsync(personEmail);

            // Assert
            Assert.NotNull(personRolesSummary);
            Assert.Equal(personWithNoRoles.Id, personRolesSummary!.Id);
            Assert.Equal(personWithNoRoles.Roles.Count, personRolesSummary!.Roles.Count);
        }

        [Fact]
        public async Task CreatePersonAsAdminAsync_WhenDataIsValid_ShouldReturnCorrectObject()
        {
            // Arrange
            var createdAdmin = new AdminSummaryResponse(personId, personDisplayName, personEmail);

            var personWithRoles = new PersonRolesSummaryResponse(
                personId, 
                new List<RoleId>() { RoleId.Administrator });
            
            adminRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateAdminRepoRequest>()))
                .ReturnsAsync(createdAdmin);
            
            personsRepositoryMock
                .Setup(x => x.GetPersonRolesByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(personWithRoles);

            // Act
            var personRolesSummary = await personsService.CreatePersonAsAdminAsync(personEmail, personDisplayName);

            // Assert
            Assert.NotNull(personRolesSummary);
            Assert.Equal(createdAdmin.Id, personRolesSummary!.Id);
            Assert.Single(personRolesSummary.Roles);
        }

        [Fact]
        public async Task CreatePersonAsAdminAsync_WhenPersonNotCreated_ShouldReturnNull()
        {
            // Act
            var personRolesSummary = await personsService.CreatePersonAsAdminAsync(personEmail, personDisplayName);

            // Assert
            Assert.Null(personRolesSummary);
        }
    }
}