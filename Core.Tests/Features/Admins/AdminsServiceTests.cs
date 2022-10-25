using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Admins;
using Core.Features.Admins.Interfaces;
using Core.Features.Admins.RequestModels;
using Core.Features.Admins.ResponseModels;
using Core.Features.Admins.Support;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Persons.Interfaces;
using Core.Features.Persons.ResponseModels;
using Core.Features.Specialties.Entities;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.ResponseModels;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Features.Admins
{
    public class AdminsServiceTests
    {
        private readonly AdminsService adminsService;
        private readonly Mock<IAdminsRepository> adminsRepostitoryMock;
        private readonly Mock<IMentorsRepository> mentorsRepositoryMock;
        private readonly Mock<IIdentityRepository> identityRepositoryMock;
        private readonly Mock<ISpecialitiesRepository> specialitiesRepositoryMock;
        private readonly CreateAdminRequest createAdminRequest;
        private readonly AddMentorRoleRequest addMentorRoleRequest;
        private readonly AdminSummaryResponse adminSummaryResponse;
        private readonly IdentitySummaryResponse identitySummaryResponse;
        private readonly MentorSummaryResponse mentorSummaryResponse;
        private readonly Guid adminId = Guid.NewGuid();
        private readonly List<Guid> specialityIds;
        private readonly List<Speciality> specialities;
        private readonly Speciality speciality;
        
        public AdminsServiceTests()
        {
            adminsRepostitoryMock = new Mock<IAdminsRepository>();
            mentorsRepositoryMock = new Mock<IMentorsRepository>();
            identityRepositoryMock = new Mock<IIdentityRepository>();
            specialitiesRepositoryMock = new Mock<ISpecialitiesRepository>();

            var adminServiceLogger = new Mock<ILogger<AdminsService>>();

            var createAdminValidator = new CreateAdminRequestValidator();
            var addMentorRoleValidator = new AddMentorRoleRequestValidator();
            
            var adminValidator = new AdminValidator(createAdminValidator, addMentorRoleValidator);

            adminsService = new AdminsService(
                adminsRepostitoryMock.Object,
                identityRepositoryMock.Object,
                mentorsRepositoryMock.Object,
                specialitiesRepositoryMock.Object,
                adminServiceLogger.Object,
                adminValidator,
                new PaginationRequestValidator()
            );

            createAdminRequest = new CreateAdminRequest(TestHelper.EmailMock, TestHelper.ApplicationUrlMock);

            adminSummaryResponse = new AdminSummaryResponse(
                adminId,
                TestHelper.DisplayNameMock,
                TestHelper.EmailMock
            );

            identitySummaryResponse = new IdentitySummaryResponse(
                TestHelper.EmailMock,
                TestHelper.DisplayNameMock
            );

            speciality = new Speciality
            {
                Id = Guid.NewGuid(), 
                Name = "Backend"
            };

            specialityIds = new List<Guid>() { speciality.Id };

            specialities = new List<Speciality>() { speciality };

            var specialitySummaries = new List<SpecialitySummaryResponse>() 
            { 
                new SpecialitySummaryResponse(speciality.Id, speciality.Name)
            };
            
            mentorSummaryResponse = new MentorSummaryResponse(
                adminId,
                TestHelper.DisplayNameMock,
                TestHelper.EmailMock,
                specialitySummaries
            );

            addMentorRoleRequest = new AddMentorRoleRequest(adminId, specialityIds);
        }

        #region CreateAsyncTests

        [Fact]
        public async Task CreateAsync_WhenAllDataIsCorrect_ShouldReturnCorrectObject()
        {
            // Arrange
            adminsRepostitoryMock 
                .Setup(x => x.ExistsByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            identityRepositoryMock
                .Setup(x => x.SendUserInviteAsync(It.IsAny<string>(),It.IsAny<string>()))
                .ReturnsAsync(identitySummaryResponse);

            adminsRepostitoryMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateAdminRepoRequest>()))
                .ReturnsAsync(adminSummaryResponse);

            // Act
            var createAdminResponse = await adminsService.CreateAsync(createAdminRequest);

            // Assert
            Assert.NotNull(createAdminResponse);
            Assert.Equal(adminSummaryResponse.Id, createAdminResponse.Id);
            Assert.Equal(adminSummaryResponse.DisplayName, createAdminResponse.DisplayName);
            Assert.Equal(adminSummaryResponse.WorkEmail, createAdminResponse.WorkEmail);
        }

        [Theory]
        [MemberData(nameof(TestHelper.InvalidEmails), MemberType = typeof(TestHelper))]
        public async Task CreateAsync_WhenAllEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {
            // Arrange
            var invalidCreateAdminRequest = new CreateAdminRequest(invalidEmail, TestHelper.ApplicationUrlMock);

            // Act
            var action = async () => await adminsService.CreateAsync(invalidCreateAdminRequest);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenAllEmailIsAlredyUsed_ShouldThrowException()
        {
            // Arrange
            adminsRepostitoryMock
                .Setup(x => x.ExistsByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var action = async () => await adminsService.CreateAsync(createAdminRequest);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenApplicationIsEmpty_ShouldThrowException()
        {
            // Arrange
            var invalidCreateAdminRequest = new CreateAdminRequest(TestHelper.EmailMock, string.Empty);

            // Act
            var action = async () => await adminsService.CreateAsync(invalidCreateAdminRequest);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        #endregion

        #region GetByIdAsyncTests
        
        [Fact]
        public async Task GetByIdAsync_WhenIdIsFound_ShouldReturnCorrectObject()
        {
            // Arrange
            adminsRepostitoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(adminSummaryResponse);

            // Act
            var createAdminResponse = await adminsService.GetByIdAsync(adminId);

            // Assert
            Assert.NotNull(createAdminRequest);
            Assert.Equal(adminSummaryResponse.Id, createAdminResponse?.Id);
            Assert.Equal(adminSummaryResponse.DisplayName, createAdminResponse?.DisplayName);
            Assert.Equal(adminSummaryResponse.WorkEmail, createAdminResponse?.WorkEmail);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdIsNotFound_ShouldThrowException()
        {
            // Act
            var action = async () => await adminsService.GetByIdAsync(adminId);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        #endregion

        #region GetAllAsyncTests

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectCountElements()
        {
            // Arrange
            var paginationRequest = new PaginationRequest(2, 5);

            var additionalAdmin = new AdminListingResponse(
                Guid.NewGuid(),
                "Jane Doe",
                "Jane.Doe@test.com",
                true
            );

            var adminsListings = new List<AdminListingResponse>() { additionalAdmin };
            
            adminsRepostitoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(6);
            
            adminsRepostitoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(adminsListings);

            // Act
            var admins = await adminsService.GetAllAsync(paginationRequest);

            // Assert
            Assert.Equal(adminsListings.Count, admins.Content.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var paginationRequest = new PaginationRequest(1, 5);

            adminsRepostitoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(0);

            // Act
            var admins = await adminsService.GetAllAsync(paginationRequest);

            // Assert
            Assert.Empty(admins.Content);
        }

        [Theory]
        [InlineData(-1, 10)]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        public async Task GetAllAsync_WhenFilterIsInvalid_ShouldThrowException(int invalidPageNum, int invalidPageSize)
        {
            // Arrange
            var paginationRequest = new PaginationRequest(invalidPageNum, invalidPageSize);
            
            // Act
            var action = async () => await adminsService.GetAllAsync(paginationRequest);
            
            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }
        
        [Fact]
        public async Task GetAllAsync_WhenPageNumMoreThanTotalPages_ShouldThrowException()
        {
            // Arrange
            var paginationRequest = new PaginationRequest(2, 5);

            adminsRepostitoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(1);

            // Act
            var action = async () => await adminsService.GetAllAsync(paginationRequest);
            
            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        #endregion

        #region AddAsMentorAsyncTests

        [Fact]
        public async Task AddAsMentorAsync_WhenAllDataIsCorrect_ShouldReturnCorrectObject()
        {
            // Arrange
            mentorsRepositoryMock
                .Setup(x => x.IsMentorByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);
            
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);
                
            mentorsRepositoryMock
                .Setup(x => x.AddMentorRoleByIdAsync(It.IsAny<AddMentorRoleRepoRequest>()))
                .ReturnsAsync(mentorSummaryResponse);

            // Act
            var addMentorRoleResponse = await adminsService.AddAsMentorAsync(addMentorRoleRequest);

            // Assert
            Assert.NotNull(addMentorRoleResponse);
            Assert.Equal(mentorSummaryResponse.Id, addMentorRoleResponse.Id);
            Assert.Equal(mentorSummaryResponse.DisplayName, addMentorRoleResponse.DisplayName);
            Assert.Equal(mentorSummaryResponse.WorkEmail, addMentorRoleResponse.WorkEmail);
            Assert.Equal(mentorSummaryResponse.Specialities.Count(), addMentorRoleResponse.Specialities.Count());
        }

        [Fact]
        public async Task AddAsMentorAsync_WhenIdIsEmpty_ShouldThrowException()
        {
            // Arrange
            var invalidRequest = new AddMentorRoleRequest(Guid.Empty, specialityIds);

            // Act
            var action = async () => await adminsService.AddAsMentorAsync(invalidRequest);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }   

        [Fact]
        public async Task AddAsMentorAsync_WhenSpecialityIdsAreEmpty_ShouldThrowException()
        {
            // Arrange
            var invalidRequest = new AddMentorRoleRequest(adminId, new List<Guid>());

            // Act
            var action = async () => await adminsService.AddAsMentorAsync(invalidRequest);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        } 

        [Fact]
        public async Task AddAsMentorAsync_WhenAdminIsAlreadyMentor_ShouldThrowException()
        {
            // Arrange
            mentorsRepositoryMock
                .Setup(x => x.IsMentorByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // Act
            var action = async () => await adminsService.AddAsMentorAsync(addMentorRoleRequest);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        } 

        [Fact]
        public async Task AddAsMentorAsync_WhenNotAllSpecialitiesAreFound_ShouldThrowException()
        {
            // Arrange
            var emptySpecialities = new List<Speciality>();

            mentorsRepositoryMock
                .Setup(x => x.IsMentorByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);
            
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(emptySpecialities);
            
            // Act
            var action = async () => await adminsService.AddAsMentorAsync(addMentorRoleRequest);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task AddAsMentorAsync_WhenMentorNotCreated_ShouldThrowException()
        {
            // Arrange
            mentorsRepositoryMock
                .Setup(x => x.IsMentorByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);
            
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);

            // Act
            var action = async () => await adminsService.AddAsMentorAsync(addMentorRoleRequest);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        } 

        #endregion
    }
}