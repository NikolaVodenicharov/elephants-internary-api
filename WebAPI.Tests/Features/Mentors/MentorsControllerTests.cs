using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Mentors.Support;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialties.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Features.Mentors;
using WebAPI.Features.Mentors.Support;
using Xunit;
using Core.Features.Persons.ResponseModels;
using WebAPI.Common.SettingsModels;
using WebAPI.Features.Mentors.ApiRequestModels;
using System.Linq;
using Core.Features.Specialities.Interfaces;
using Core.Features.Campaigns.ResponseModels;

namespace WebAPI.Tests.Features.Mentors
{
    public class MentorsControllerTests
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly string displayName = "Display name";
        private readonly string email = "first.last@example.bg";
        private readonly int validPageNum = 1;
        private readonly int invalidPageNum = -1;
        private readonly int validPageSize = 10;
        private readonly int invalidPageSize = 0;
        private readonly Mock<IMentorsService> mentorsServiceMock;
        private CreateMentorApiRequest createMentorApiRequest = null!;
        private SpecialitySummaryResponse specialitySummaryResponse = null!;
        private List<SpecialitySummaryResponse> specialitySummaryResponses = null!;
        private List<Guid> specialityIds = null!;
        private Speciality specialityMock = null!;
        private List<CampaignSummaryResponse> campaignSummaryResponses = null!;
        private CampaignSummaryResponse campaignSummaryResponse = null!;
        private MentorSummaryResponse mentorSummaryResponse = null!;
        private MentorPaginationResponse mentorPaginationResponse = null!;
        private MentorDetailsResponse mentorDetailsResponse = null!;
        private IEnumerable<MentorPaginationResponse> mentorPaginationResponses = null!;

        private readonly MentorsController mentorsController;

        public MentorsControllerTests()
        {
            var createMentorRequestValidator = new CreateMentorRequestValidator();
            var updateMentorRequestValidator = new UpdateMentorRequestValidator();
            var paginationRequestValidator = new PaginationRequestValidator();

            var mentorValidator = new MentorValidator(createMentorRequestValidator, updateMentorRequestValidator);

            mentorsServiceMock = new Mock<IMentorsService>();

            var loggerMock = new Mock<ILogger<MentorsController>>();

            var invitationUrls = new InvitationUrlSettings
            {
                BackOfficeUrl = "https://test.com",
                FrontOfficeUrl = "https://test.com"
            };

            var invitationUrlSettings = Options.Create<InvitationUrlSettings>(invitationUrls);

            mentorsController = new(
                mentorsServiceMock.Object,
                loggerMock.Object,
                mentorValidator,
                paginationRequestValidator,
                invitationUrlSettings);

            InitiazlizeMockModels();
        }

        private void InitiazlizeMockModels()
        {
            specialityMock = new Speciality()
            {
                Id = Guid.NewGuid(),
                Name = "Backend"
            };

            specialityIds = new List<Guid>() { specialityMock.Id };

            createMentorApiRequest = new CreateMentorApiRequest(email, specialityIds);

            specialitySummaryResponse = new SpecialitySummaryResponse(specialityMock.Id, specialityMock.Name);

            specialitySummaryResponses = new List<SpecialitySummaryResponse>() { specialitySummaryResponse };

            mentorSummaryResponse = new MentorSummaryResponse(
                id,
                displayName,
                email,
                specialitySummaryResponses);

            campaignSummaryResponse = new CampaignSummaryResponse(
                Guid.NewGuid(),
                "Campaign name",
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(100),
                true);

            campaignSummaryResponses = new List<CampaignSummaryResponse>() { campaignSummaryResponse };

            mentorPaginationResponse = new MentorPaginationResponse(id,
                displayName,
                email,
                specialitySummaryResponses,
                campaignSummaryResponses);

            mentorDetailsResponse = new MentorDetailsResponse(
                id,
                displayName,
                email,
                campaignSummaryResponses,
                specialitySummaryResponses);

            mentorPaginationResponses = new List<MentorPaginationResponse>() { mentorPaginationResponse };
        }

        [Fact]
        public async Task CreateAsync_WhenDataIsCorrect_ShouldReturnCorrectData()
        {
            //Arrange
            mentorsServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateMentorRequest>()))
                .ReturnsAsync(mentorSummaryResponse);

            //Act
            var actionResult = await mentorsController.CreateAsync(createMentorApiRequest);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            var createdResponse = jsonResult!.Value as CoreResponse<MentorSummaryResponse>;

            Assert.Equal(mentorSummaryResponse.Id, createdResponse!.Data!.Id);
            Assert.Equal(mentorSummaryResponse.DisplayName, createdResponse.Data.DisplayName);
            Assert.Equal(mentorSummaryResponse.Specialities, createdResponse.Data.Specialities);
        }

        [Fact]
        public async Task CreateAsync_WhenNoSpecialtiesArePassed_ShouldThrowException()
        {
            // Arrange
            var invalidRequest = new CreateMentorApiRequest(email, new List<Guid>());

            // Act
            var action = async () => await mentorsController.CreateAsync(invalidRequest);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
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
        public async Task UpdateAsync_WhenDataIsCorrect_ShouldReturnCorrectData()
        {
            //Assert
            var request = new UpdateMentorRequest(id, specialityIds);

            mentorsServiceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateMentorRequest>()))
                .ReturnsAsync(mentorDetailsResponse);

            //Act
            var actionResult = await mentorsController.UpdateAsync(id, request);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var updatedResponse = jsonResult!.Value as CoreResponse<MentorDetailsResponse>;

            Assert.Equal(mentorDetailsResponse.Id, updatedResponse!.Data!.Id);
            Assert.Equal(mentorDetailsResponse.DisplayName, updatedResponse.Data.DisplayName);
            Assert.Equal(mentorDetailsResponse.WorkEmail, updatedResponse.Data.WorkEmail);
            Assert.Equal(mentorDetailsResponse.Specialities, updatedResponse.Data.Specialities);
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
            var notExistingId = Guid.NewGuid();

            mentorsServiceMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new CoreException(String.Empty, System.Net.HttpStatusCode.NotFound));

            //Act
            var action = async () => await mentorsController.GetByIdAsync(notExistingId);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdExists_ShouldReturnCorrectObject()
        {
            //Arrange
            mentorsServiceMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mentorDetailsResponse);

            //Act
            var actionResult = await mentorsController.GetByIdAsync(id);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var actualResponse = jsonResult!.Value as CoreResponse<MentorDetailsResponse>;

            Assert.Equal(mentorDetailsResponse.Id, actualResponse!.Data!.Id);
            Assert.Equal(mentorDetailsResponse.DisplayName, actualResponse.Data.DisplayName);
            Assert.Equal(mentorDetailsResponse.WorkEmail, actualResponse.Data.WorkEmail);
            Assert.Equal(mentorDetailsResponse.Specialities, actualResponse.Data.Specialities);
            Assert.Equal(mentorDetailsResponse.Campaigns, actualResponse.Data.Campaigns);
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
            var expectedPaginationResponse = new PaginationResponse<MentorPaginationResponse>(mentorPaginationResponses, 1, 2);

            mentorsServiceMock
                .Setup(x => x.GetPaginationAsync(It.IsAny<PaginationRequest>(), null))
                .ReturnsAsync(expectedPaginationResponse);

            //Act
            var actionResult = await mentorsController.GetAllAsync(validPageNum, validPageSize);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var paginationResponse = jsonResult!.Value as CoreResponse<PaginationResponse<MentorPaginationResponse>>;

            Assert.Equal(mentorPaginationResponses.Count(), paginationResponse!.Data!.Content.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenPageParametersSetAndEmpty_ShouldReturnEmptyCollection()
        {
            //Arrange
            var emptyList = new List<MentorPaginationResponse>();

            var paginationResponseMock = new PaginationResponse<MentorPaginationResponse>(emptyList, 1, 1);

            mentorsServiceMock
                .Setup(x => x.GetPaginationAsync(It.IsAny<PaginationRequest>(), null))
                .ReturnsAsync(paginationResponseMock);

            //Act
            var actionResult = await mentorsController.GetAllAsync(validPageNum, validPageSize);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var okObjectResult = actionResult as JsonResult;

            Assert.NotNull(okObjectResult);

            var paginationResponse = okObjectResult!.Value as CoreResponse<PaginationResponse<MentorPaginationResponse>>;

            Assert.Empty(paginationResponse!.Data!.Content);
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
            //Arrange
            mentorsServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MentorPaginationResponse>());

            //Act
            var actionResult = await mentorsController.GetAllAsync();

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var okObjectResult = actionResult as JsonResult;

            Assert.NotNull(okObjectResult);

            var response = okObjectResult!.Value as CoreResponse<IEnumerable<MentorPaginationResponse>>;

            Assert.Empty(response!.Data);
        }

        [Fact]
        public async Task GetAllAsync_WhenPageParametersNotSetAndNotEmpty_ShouldReturnCorrectCountElements()
        {
            //Arrange
            mentorsServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(mentorPaginationResponses);

            //Act
            var actionResult = await mentorsController.GetAllAsync();

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var response = jsonResult!.Value as CoreResponse<IEnumerable<MentorPaginationResponse>>;

            Assert.Equal(mentorPaginationResponses.Count(), response!.Data!.Count());
        }
    }
}
