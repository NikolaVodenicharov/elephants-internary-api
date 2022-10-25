using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Admins.Interfaces;
using Core.Features.Admins.RequestModels;
using Core.Features.Admins.ResponseModels;
using Core.Features.Admins.Support;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Specialities.ResponseModels;
using FluentValidation;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Common.SettingsModels;
using WebAPI.Features.Admins;
using Xunit;

namespace WebAPI.Tests.Features.Admins
{
    public class AdminsControllerTests
    {
        private readonly AdminsController adminsController;
        private readonly Mock<IAdminsService> adminsServiceMock;
        private readonly AdminSummaryResponse adminSummaryResponse;
        private readonly MentorSummaryResponse mentorSummaryResponse;
        private readonly Guid adminId = Guid.NewGuid();
        private readonly List<Guid> specialityIds = null!;
        private readonly InvitationUrlSettings invitationUrls;
        private readonly int pageNum = 1;
        private readonly int pageSize = 10;
        private readonly int defaultTotalPages = 1;

        public AdminsControllerTests()
        {
            adminsServiceMock = new Mock<IAdminsService>();

            var adminsControllerLogger = new Mock<ILogger<AdminsController>>();

            invitationUrls = new InvitationUrlSettings
            {
                BackOfficeUrl = "https://test.com",
                FrontOfficeUrl = "https://test.com"
            };

            var invitationUrlSettings = Options.Create<InvitationUrlSettings>(invitationUrls);

            var adminValidator = new AdminValidator(new CreateAdminRequestValidator(), new AddMentorRoleRequestValidator());

            adminsController = new AdminsController(
                adminsServiceMock.Object,
                adminValidator,
                new PaginationRequestValidator(),
                adminsControllerLogger.Object,
                invitationUrlSettings
            );

            adminSummaryResponse = new AdminSummaryResponse(
                adminId,
                TestHelper.DisplayNameMock,
                TestHelper.EmailMock
            );

            var specialityId = Guid.NewGuid();

            specialityIds = new List<Guid>() { specialityId };

            var specialitySummaryResponse = new SpecialitySummaryResponse(specialityId, "Backend");

            var specialitySummaries = new List<SpecialitySummaryResponse>() { specialitySummaryResponse };

            mentorSummaryResponse = new MentorSummaryResponse(
                Guid.NewGuid(),
                TestHelper.DisplayNameMock,
                TestHelper.EmailMock,
                specialitySummaries
            );
        }

        #region CreateAsyncTests

        [Fact]
        public async Task CreateAsync_WhenDataIsCorrect_ShouldReturnCorrectObject()
        {
            // Arrange
            var adminEmail = TestHelper.EmailMock;

            adminsServiceMock
                .Setup(a => a.CreateAsync(It.IsAny<CreateAdminRequest>()))
                .ReturnsAsync(adminSummaryResponse);

            // Act
            var actionResult = await adminsController.CreateAsync(adminEmail);

            // Assert
            Assert.IsType<JsonResult>(actionResult);

           var jsonResult = actionResult as JsonResult;

           Assert.NotNull(jsonResult);

           var coreResponse = jsonResult!.Value as CoreResponse<AdminSummaryResponse>;

           Assert.Equal(adminSummaryResponse.Id, coreResponse!.Data!.Id);
           Assert.Equal(adminSummaryResponse.DisplayName, coreResponse.Data.DisplayName);
           Assert.Equal(adminSummaryResponse.WorkEmail, coreResponse.Data.WorkEmail);
        }

        [Theory]
        [MemberData(nameof(TestHelper.InvalidEmails), MemberType = typeof(TestHelper))]
        public async Task CreateAsync_WhenEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {   
            // Act
            var action = async () => await adminsController.CreateAsync(invalidEmail);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        #endregion

        #region GetByIdAsyncTests

        [Fact]
        public async Task GetByIdAsync_WhenDataIsValid_ShouldReturnCorrectObject()
        {
            // Arrange
            adminsServiceMock
                .Setup(a => a.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(adminSummaryResponse);

            // Act            
            var actionResult = await adminsController.GetByIdAsync(adminId);

            // Assert
            Assert.IsType<JsonResult>(actionResult);

           var jsonResult = actionResult as JsonResult;

           Assert.NotNull(jsonResult);

           var coreResponse = jsonResult!.Value as CoreResponse<AdminSummaryResponse>;

           Assert.Equal(adminSummaryResponse.Id, coreResponse!.Data!.Id);
           Assert.Equal(adminSummaryResponse.DisplayName, coreResponse.Data.DisplayName);
           Assert.Equal(adminSummaryResponse.WorkEmail, coreResponse.Data.WorkEmail);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdIsNotFound_ShouldThrowException()
        {
            // Arrange
             adminsServiceMock
               .Setup(a => a.GetByIdAsync(It.IsAny<Guid>()))
               .ThrowsAsync(new CoreException(String.Empty, System.Net.HttpStatusCode.NotFound));
            
            // Act
            var action = async () => await adminsController.GetByIdAsync(adminId);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        #endregion

        #region  GetAllAsyncTests

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectCountElements()
        {
            // Arrange
            var additionalAdmin = new AdminListingResponse(
                Guid.NewGuid(),
                "Jane Doe",
                "Jane.Doe@test.com",
                true
            );

            var adminsList = new List<AdminListingResponse>() { additionalAdmin };

            var adminsPaginationResponse = new PaginationResponse<AdminListingResponse>(adminsList, pageNum, defaultTotalPages);

            adminsServiceMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(adminsPaginationResponse);

            // Act
            var actionResult = await adminsController.GetAllAsync(pageNum, pageSize);

            // Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var coreResponse = jsonResult!.Value as CoreResponse<PaginationResponse<AdminListingResponse>>;

            Assert.Equal(adminsPaginationResponse.Content.Count(), coreResponse!.Data!.Content.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var emptyPaginationResponse = new PaginationResponse<AdminListingResponse>(
                new List<AdminListingResponse>(), pageNum, defaultTotalPages);

            adminsServiceMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(emptyPaginationResponse);

            // Act
            var actionResult = await adminsController.GetAllAsync(pageNum, pageSize);

            // Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var actualResponse = jsonResult!.Value as CoreResponse<PaginationResponse<AdminListingResponse>>;

            Assert.Empty(actualResponse!.Data!.Content);
        }
        
        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        [InlineData(5, 0)]
        [InlineData(5, -1)]
        [InlineData(-1, -1)]
        public async Task GetAllAsync_WhenFilterIsInvalid_ShouldThrowException(int invalidPageNum, int invalidPageSize)
        {
            // Act
            var action = async () => await adminsController.GetAllAsync(invalidPageNum, invalidPageSize);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        #endregion

        #region AddMentorRoleAsyncTests

        [Fact]
        public async Task AddMentorRoleAsync_WhenDataIsCorrect_ShouldReturnCorrectObject()
        {
            // Arrange
            var addMentorRoleRequest = new AddMentorRoleRequest(adminId, specialityIds);

            adminsServiceMock
                .Setup(a => a.AddAsMentorAsync(It.IsAny<AddMentorRoleRequest>()))
                .ReturnsAsync(mentorSummaryResponse);

            // Act
            var actionResult = await adminsController.AddMentorRoleAsync(adminId, addMentorRoleRequest);

            // Assert
            Assert.IsType<JsonResult>(actionResult);

           var jsonResult = actionResult as JsonResult;

           Assert.NotNull(jsonResult);

           var coreResponse = jsonResult!.Value as CoreResponse<MentorSummaryResponse>;

           Assert.Equal(mentorSummaryResponse.Id, coreResponse!.Data!.Id);
           Assert.Equal(mentorSummaryResponse.DisplayName, coreResponse.Data.DisplayName);
           Assert.Equal(mentorSummaryResponse.WorkEmail, coreResponse.Data.WorkEmail);
           Assert.Equal(mentorSummaryResponse.Specialities.Count(), coreResponse.Data.Specialities.Count());
        }

        [Fact]
        public async Task AddMentorRoleAsync_WhenQueryIdAndModelIdDoNotMatch_ShouldThrowException()
        {
            // Arrange
            var addMentorRoleRequest = new AddMentorRoleRequest(adminId, specialityIds);
            
            // Act
            var action = async () => await adminsController.AddMentorRoleAsync(Guid.NewGuid(), addMentorRoleRequest);
            
            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task AddMentorRoleAsync_WhenIdIsNotFound_ShouldThrowException()
        {
            // Arrange
            var addMentorRoleRequest = new AddMentorRoleRequest(adminId, specialityIds);

            adminsServiceMock
               .Setup(a => a.AddAsMentorAsync(It.IsAny<AddMentorRoleRequest>()))
               .ThrowsAsync(new CoreException(String.Empty, System.Net.HttpStatusCode.NotFound));
            
            // Act
            var action = async () => await adminsController.AddMentorRoleAsync(adminId, addMentorRoleRequest);
            
            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task AddMentorRoleAsync_WhenNoSpecialitiesArePassed_ShouldThrowException()
        {
            // Arrange
            var addMentorRoleRequest = new AddMentorRoleRequest(adminId, new List<Guid>());
            
            // Act
            var action = async () => await adminsController.AddMentorRoleAsync(adminId, addMentorRoleRequest);
            
            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        #endregion
    }   
}