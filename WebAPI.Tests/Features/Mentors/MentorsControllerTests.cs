using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Mentors.Support;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialties.Entities;
using Core.Features.Users.Interfaces;
using Core.Features.Identity.ResponseModels;
using Core.Features.Users.Entities;
using Core.Features.Users.RequestModels;
using Core.Features.Users.ResponseModels;
using Core.Features.Campaigns.ResponseModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Features.Mentors.ApiRequestModels;
using WebAPI.Features.Mentors;
using WebAPI.Features.Mentors.Support;
using Xunit;

namespace WebAPI.Tests.Features.Mentors
{
    public class MentorsControllerTests
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly string displayName = "First Last";
        private readonly string email = "first.last@example.bg";

        private readonly int validPageNum = 1;
        private readonly int invalidPageNum = -1;
        private readonly int validPageSize = 10;
        private readonly int invalidPageSize = 0;

        private readonly Mock<IMentorsService> mentorsServiceMock;
        private readonly Mock<IUsersService> usersServiceMock;
        private readonly MentorsController mentorsController;

        private readonly SpecialitySummaryResponse specialitySummaryResponse;
        private Speciality speciality;
        private List<SpecialitySummaryResponse> specialitySummaries;
        private List<Guid> specialityIds;
        private List<CampaignSummaryResponse> campaignSummaries;
        private IdentitySummaryResponse userIdentitySummary;
        private MentorDetailsResponse mentorDetails;

        public MentorsControllerTests()
        {
            var createMentorRequestValidator = new CreateMentorRequestValidator();
            var updateMentorRequestValidator = new UpdateMentorRequestValidator();
            var createMentorApiRequestValidator = new CreateMentorApiRequestValidator();
            var paginationRequestValidator = new PaginationRequestValidator();

            mentorsServiceMock = new Mock<IMentorsService>();
            usersServiceMock = new Mock<IUsersService>();

            var loggerMock = new Mock<ILogger<MentorsController>>();

            var invitationUrls = new InvitationUrlSettings
            {
                BackOfficeUrl = "https://test.com",
                FrontOfficeUrl = "https://test.com"
            };

            var invitationUrlSettings = Options.Create<InvitationUrlSettings>(invitationUrls);
        
            mentorsController = new(
                mentorsServiceMock.Object,
                usersServiceMock.Object,
                loggerMock.Object,
                createMentorRequestValidator,
                updateMentorRequestValidator,
                createMentorApiRequestValidator,
                paginationRequestValidator,
                invitationUrlSettings);

            speciality = new Speciality()
            {
                Id = Guid.NewGuid(),
                Name = "Backend"
            };

            specialityIds = new List<Guid>() { speciality.Id };

            specialitySummaryResponse = new SpecialitySummaryResponse(speciality.Id, speciality.Name);

            specialitySummaries = new List<SpecialitySummaryResponse>() { specialitySummaryResponse };

            var campaignSummary = new CampaignSummaryResponse(
                Guid.NewGuid(),
                "Test Campaign",
                DateTime.Today.AddDays(5),
                DateTime.Today.AddDays(35),
                false
            );

            campaignSummaries = new List<CampaignSummaryResponse>() { campaignSummary };

            var identityId = Guid.NewGuid().ToString();
            userIdentitySummary = new IdentitySummaryResponse(email, displayName);

            mentorDetails = new MentorDetailsResponse(id, displayName, email, specialitySummaries, campaignSummaries);
        }

        [Fact]
        public async Task CreateAsync_WhenDataIsCorrect_ShouldReturnCorrectData()
        {
            //Arrange
            var request = new CreateMentorApiRequest(email, specialityIds);
   
            var userSummary = new UserSummaryResponse(Guid.NewGuid(), email, RoleEnum.Mentor);

            var mentorSummary = new MentorSummaryResponse(id, displayName, email, specialitySummaries);

            mentorsServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateMentorRequest>()))
                .ReturnsAsync(mentorSummary);
            
            usersServiceMock
                .Setup(x => x.ExistsByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            usersServiceMock
                .Setup(x => x.SendInvitationByEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(userIdentitySummary);

            usersServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateUserRequest>()))
                .ReturnsAsync(userSummary);

            //Act
            var actionResult = await mentorsController.CreateAsync(request);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var createdResponse = jsonResult!.Value as CoreResponse<MentorSummaryResponse>;

            Assert.Equal(mentorSummary.Id, createdResponse.Data.Id);
            Assert.Equal(mentorSummary.DisplayName, createdResponse.Data.DisplayName);
            Assert.Equal(mentorSummary.Email, createdResponse.Data.Email);
            Assert.Equal(mentorSummary.Specialities, createdResponse.Data.Specialities);
        }

        [Theory]
        [InlineData("john@endava.com.")]
        [InlineData("john@endava.co_uk")]
        [InlineData("joh n@endava.co_uk")]
        public async Task CreateAsync_WhenEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {
            //Arrange
            var request = new CreateMentorApiRequest(invalidEmail, specialityIds);

            //Act
            var action = async () => await mentorsController.CreateAsync(request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenUserEmailExists_ShouldThrowException()
        {
            // Arrange
            var request = new CreateMentorApiRequest(email, specialityIds);
            
            usersServiceMock
                .Setup(x => x.ExistsByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            
            // Act
            var action = async () => await mentorsController.CreateAsync(request);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenDataIsCorrectShouldReturnCorrectData()
        {
            //Assert
            var request = new UpdateMentorRequest(id, specialityIds);

            mentorsServiceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateMentorRequest>()))
                .ReturnsAsync(mentorDetails);

            //Act
            var actionResult = await mentorsController.UpdateAsync(id, request);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var updatedResponse = jsonResult!.Value as CoreResponse<MentorDetailsResponse>;

            Assert.Equal(mentorDetails.Id, updatedResponse.Data.Id);
            Assert.Equal(mentorDetails.DisplayName, updatedResponse.Data.DisplayName);
            Assert.Equal(mentorDetails.Email, updatedResponse.Data.Email);
            Assert.Equal(mentorDetails.Specialities, updatedResponse.Data.Specialities);
            Assert.Equal(mentorDetails.Campaigns, updatedResponse.Data.Campaigns);
        }

        [Fact]
        public async Task UpdateAsync_WhenQueryIdAndModelIdDoesNotMatch_ShouldThrowException()
        {
            //Arrange
            var request = new UpdateMentorRequest(id, specialityIds);

            //Act
            var action = async () => await mentorsController.UpdateAsync(Guid.NewGuid(), request);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdNotFound_ShouldThrowException()
        {
            //Arrange
            var newId = Guid.NewGuid();

            mentorsServiceMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new CoreException(String.Empty, System.Net.HttpStatusCode.NotFound));

            //Act
            var action = async () => await mentorsController.GetByIdAsync(newId);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdExists_ShouldReturnCorrectObject()
        {
            //Arrange
            mentorsServiceMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mentorDetails);

            //Act
            var actionResult = await mentorsController.GetByIdAsync(id);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var actualResponse = jsonResult!.Value as CoreResponse<MentorDetailsResponse>;

            Assert.Equal(mentorDetails.Id, actualResponse.Data.Id);
            Assert.Equal(mentorDetails.DisplayName, actualResponse.Data.DisplayName);
            Assert.Equal(mentorDetails.Email, actualResponse.Data.Email);
            Assert.Equal(mentorDetails.Specialities, actualResponse.Data.Specialities);
            Assert.Equal(mentorDetails.Campaigns, actualResponse.Data.Campaigns);
        }

        [Fact]
        public async Task GetAllAsync_WhenPageNumIsLessThanOne_ShouldThrowException()
        {
            //Act
            var action = async () => await mentorsController.GetAllAsync(invalidPageNum, validPageSize);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetAllAsync_WhenPageSizeIsLessThanOne_ShouldThrowException()
        {
            //Act
            var action = async () => await mentorsController.GetAllAsync(validPageNum, invalidPageSize);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetAllAsync_WhenPageParametersSetAndNotEmpty_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var expectedResponse = new MentorDetailsResponse(
                Guid.NewGuid(), "John Smith", "john.smith@gmail.com",
                specialitySummaries,
                campaignSummaries);

            var expectedResponseList = new List<MentorDetailsResponse>() {
                mentorDetails, expectedResponse
            };

            var expectedPaginationResponse = new PaginationResponse<MentorDetailsResponse>(expectedResponseList, 1, 2);

            mentorsServiceMock
                .Setup(x => x.GetPaginationAsync(It.IsAny<PaginationRequest>(), null))
                .ReturnsAsync(expectedPaginationResponse);

            //Act
            var actionResult = await mentorsController.GetAllAsync(validPageNum, validPageSize);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var paginationResponse = jsonResult!.Value as CoreResponse<PaginationResponse<MentorDetailsResponse>>;

            Assert.Equal(expectedResponseList.Count, paginationResponse.Data.Content.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenPageParametersSetAndEmpty_ShouldReturnEmptyCollection()
        {
            //Arrange
            var emptyList = new List<MentorDetailsResponse>();

            mentorsServiceMock
                .Setup(x => x.GetPaginationAsync(It.IsAny<PaginationRequest>(), null))
                .ReturnsAsync(new PaginationResponse<MentorDetailsResponse>(emptyList, 1, 1));

            //Act
            var actionResult = await mentorsController.GetAllAsync(validPageNum, validPageSize);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var okObjectResult = actionResult as JsonResult;

            Assert.NotNull(okObjectResult);

            var paginationResponse = okObjectResult!.Value as CoreResponse<PaginationResponse<MentorDetailsResponse>>;

            Assert.Empty(paginationResponse.Data.Content);
        }

        [Fact]
        public async Task GetAllAsync_WhenOnlyPageNumIsSet_ShouldThrowException()
        {
            //Act
            var action = async () => await mentorsController.GetAllAsync(1);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetAllAsync_WhenOnlyPageSizeIsSet_ShouldThrowException()
        {
            //Act
            var action = async () => await mentorsController.GetAllAsync(pageSize: 1);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetAllAsync_WhenPageParametersNotSetAndEmpty_ShouldReturnEmptyCollection()
        {
            //Act
            var actionResult = await mentorsController.GetAllAsync();

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var okObjectResult = actionResult as JsonResult;

            Assert.NotNull(okObjectResult);

            var response = okObjectResult!.Value as CoreResponse<IEnumerable<MentorDetailsResponse>>;

            Assert.Empty(response.Data);
        }

        [Fact]
        public async Task GetAllAsync_WhenPageParametersNotSetAndNotEmpty_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var expectedResponse = new MentorDetailsResponse(
                Guid.NewGuid(), "John Smith", "john.smith@gmail.com",
                specialitySummaries,
                campaignSummaries);

            var expectedResponseList = new List<MentorDetailsResponse>() {
                mentorDetails, expectedResponse
            };

            mentorsServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(expectedResponseList);

            //Act
            var actionResult = await mentorsController.GetAllAsync();

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var response = jsonResult!.Value as CoreResponse<IEnumerable<MentorDetailsResponse>>;

            Assert.Equal(expectedResponseList.Count, response.Data.Count());
        }
    }
}
