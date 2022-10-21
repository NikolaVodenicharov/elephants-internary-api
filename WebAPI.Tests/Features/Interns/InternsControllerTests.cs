using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns.ResponseModels;
using Core.Features.Interns.Entities;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;
using Core.Features.Interns.Support;
using Core.Features.Specialities.ResponseModels;
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
using WebAPI.Common.SettingsModels;
using WebAPI.Features.Interns;
using WebAPI.Features.Interns.ApiRequestModels;
using Xunit;

namespace WebAPI.Tests.Features.Interns
{
    public class InternsControllerTests
    {
        private readonly InternsController internsController;
        private readonly Mock<IInternsService> internsServiceMock;
        private readonly Mock<IInternCampaignsService> internsCampaignsServiceMock;
        private readonly Guid internId = Guid.NewGuid();
        private readonly Guid specialityId = Guid.NewGuid();
        private readonly Guid campaignId = Guid.NewGuid();
        private readonly string displayName = "John Doe";
        private readonly string justification = "Lorem ipsum.";
        private readonly string workEmail = "WorkEmail@gmail.com";
        private readonly InternSummaryResponse internSummaryResponseMock;
        private readonly InternDetailsResponse internDetailsResponseMock;
        private readonly StateResponse stateResponseMock;
        private readonly int pageNum = 1;
        private readonly int pageSize = 10;

        public InternsControllerTests()
        {
            internsServiceMock = new Mock<IInternsService>();
            internsCampaignsServiceMock = new Mock<IInternCampaignsService>();

            var internsControllerLoggerMock = new Mock<ILogger<InternsController>>();

            var invitationUrls = new InvitationUrlSettings
            {
                BackOfficeUrl = "https://test.com",
                FrontOfficeUrl = "https://test.com"
            };

            var invitationUrlSettings = Options.Create<InvitationUrlSettings>(invitationUrls);

            var internValidator = new InternValidator(
                new CreateInternRequestValidator(),
                new UpdateInternRequestValidator(),
                new AddInternCampaignRequestValidator(),
                new UpdateInternCampaignRequestValidator(),
                new AddStateRequestValidator());


            internsController = new(
                internsServiceMock.Object,
                internsCampaignsServiceMock.Object,
                internsControllerLoggerMock.Object,
                new PaginationRequestValidator(),
                internValidator,
                new InviteInternRequestValidator(),
                invitationUrlSettings);

            internSummaryResponseMock = new InternSummaryResponse(
                internId,
                TestHelper.FirstNameMock + " " + TestHelper.LastNameMock,
                TestHelper.EmailMock);

            internDetailsResponseMock = new InternDetailsResponse(
                internId,
                "DisplayName",
                TestHelper.FirstNameMock,
                TestHelper.LastNameMock,
                TestHelper.EmailMock,
                "WorkEmail",
                new List<InternCampaignSummaryResponse>());

            stateResponseMock = new StateResponse(
                StatusEnum.Rejected.ToString(),
                justification,
                DateTime.UtcNow);
        }

        public static IEnumerable<object[]> AddStateInvalidJustification =>
            new List<object[]>
            {
                    new object[] { string.Empty },
                    new object[] { TestHelper.GenerateString(InternValidationConstants.JustificationMaxLength + 1) },
            };

        #region CreateAsync

        [Theory]
        [MemberData(nameof(TestHelper.InvalidPersonNames), MemberType = typeof(TestHelper))]
        public async Task CreateAsync_WhenFirstNameIsInvalid_ShouldThrowException(string invalidFirstName)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                invalidFirstName,
                TestHelper.LastNameMock,
                TestHelper.EmailMock,
                campaignId,
                specialityId,
                justification);

            //Act
            var action = async () => await internsController.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(TestHelper.InvalidPersonNames), MemberType = typeof(TestHelper))]
        public async Task CreateAsync_WhenLastNameIsInvalid_ShouldThrowException(string invalidLastName)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                TestHelper.FirstNameMock,
                invalidLastName,
                TestHelper.EmailMock,
                campaignId,
                specialityId,
                justification);

            //Act
            var action = async () => await internsController.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(TestHelper.InvalidEmails), MemberType = typeof(TestHelper))]
        public async Task CreateAsync_WhenEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                TestHelper.FirstNameMock,
                TestHelper.LastNameMock,
                invalidEmail,
                campaignId,
                specialityId,
                justification);

            //Act
            var action = async () => await internsController.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenCampaignIdIsEmpty_ShouldThrowException()
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                TestHelper.FirstNameMock,
                TestHelper.LastNameMock,
                TestHelper.EmailMock,
                Guid.Empty,
                specialityId,
                justification);

            //Act
            var action = async () => await internsController.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenSpecialityIdIsEmpty_ShouldThrowException()
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                TestHelper.FirstNameMock,
                TestHelper.LastNameMock,
                TestHelper.EmailMock,
                campaignId,
                Guid.Empty,
                justification);

            //Act
            var action = async () => await internsController.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenJustificationLengthIsOutOfRange_ShouldThrowException()
        {
            //Arrange
            var justificationOutOfRange = TestHelper.GenerateString(InternValidationConstants.JustificationMaxLength + 1);

            var createInternRequest = new CreateInternRequest(
                TestHelper.FirstNameMock,
                TestHelper.LastNameMock,
                TestHelper.EmailMock,
                campaignId,
                specialityId,
                justificationOutOfRange);

            //Act
            var action = async () => await internsController.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenRequestModelIsValid_ShouldPassCorrectObject()
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                TestHelper.FirstNameMock,
                TestHelper.LastNameMock,
                TestHelper.EmailMock,
                campaignId,
                specialityId,
                justification);

            CreateInternRequest passedCreateInternRequest = null!;

            internsServiceMock
                .Setup(r => r.CreateAsync(It.IsAny<CreateInternRequest>()))
                .ReturnsAsync(internSummaryResponseMock)
                .Callback((CreateInternRequest request) => passedCreateInternRequest = request);

            //Act
            var actionResult = await internsController.CreateAsync(createInternRequest);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var coreResponse = jsonResult!.Value as CoreResponse<InternSummaryResponse>;

            Assert.NotNull(coreResponse);
            Assert.Equal(internSummaryResponseMock, coreResponse!.Data);
        }

        #endregion

        #region UpdateAsync

        [Theory]
        [MemberData(nameof(TestHelper.InvalidPersonNames), MemberType = typeof(TestHelper))]
        public async Task UpdateAsync_WhenFirstNameIsInvalid_ShouldThrowException(string invalidFirstName)
        {
            //Arrange
            var updateInternApiRequest = new UpdateInternApiRequest(
                invalidFirstName,
                TestHelper.LastNameMock,
                TestHelper.EmailMock);

            //Act
            var action = async () => await internsController.UpdateAsync(internId, updateInternApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(TestHelper.InvalidPersonNames), MemberType = typeof(TestHelper))]
        public async Task UpdateAsync_WhenLastNameIsInvalid_ShouldThrowException(string invalidLastName)
        {
            //Arrange
            var updateInternApiRequest = new UpdateInternApiRequest(
                TestHelper.FirstNameMock,
                invalidLastName,
                TestHelper.EmailMock);

            //Act
            var action = async () => await internsController.UpdateAsync(internId, updateInternApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(TestHelper.InvalidEmails), MemberType = typeof(TestHelper))]
        public async Task UpdateAsync_WhenEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {
            //Arrange
            var updateInternApiRequest = new UpdateInternApiRequest(
                TestHelper.FirstNameMock,
                TestHelper.LastNameMock,
                invalidEmail);

            //Act
            var action = async () => await internsController.UpdateAsync(internId, updateInternApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenDataIsValid_ShouldReturnCorrectData()
        {
            //Arrange
            var updateInternApiRequest = new UpdateInternApiRequest(
                TestHelper.FirstNameMock,
                TestHelper.LastNameMock,
                TestHelper.EmailMock);

            UpdateInternRequest passedUpdateInternRequest = null!;

            internsServiceMock
                .Setup(r => r.UpdateAsync(It.IsAny<UpdateInternRequest>()))
                .ReturnsAsync(internSummaryResponseMock)
                .Callback((UpdateInternRequest request) => passedUpdateInternRequest = request);

            //Act
            var actionResult = await internsController.UpdateAsync(internId, updateInternApiRequest);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var coreResponse = jsonResult!.Value as CoreResponse<InternSummaryResponse>;

            Assert.NotNull(coreResponse);
            Assert.Equal(internSummaryResponseMock, coreResponse!.Data);
        }

        #endregion

        #region GetDetailsByIdAsync

        [Fact]
        public async Task GetDetailsByIdAsync_WhenDataIsValid_ShouldReturnCorrectData()
        {
            //Arrange
            internsServiceMock
                .Setup(i => i.GetDetailsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(internDetailsResponseMock);

            //Act
            var actionResult = await internsController.GetDetailsByIdAsync(internDetailsResponseMock.Id);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var coreResponse = jsonResult!.Value as CoreResponse<InternDetailsResponse>;

            Assert.NotNull(coreResponse);
            Assert.Equal(internDetailsResponseMock, coreResponse!.Data);
        }

        #endregion

        #region GetAllAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetAllAsync_WhenPageNumIsLessThanMinimum_ShouldThrowExceptionAsync(int invalidPageNum)
        {
            //Act
            var action = async () => await internsController.GetAllAsync(invalidPageNum, pageSize);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetAllAsync_WhenPageSizeIsLessThanMinimum_ShouldThrowExceptionAsync(int invalidPageSize)
        {
            //Act
            var action = async () => await internsController.GetAllAsync(pageNum, invalidPageSize);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetAllAsync_WhenDataIsValid_ShouldReturnCorrectData()
        {
            //Arrange 
            var totalPages = 5;

            var paginationResponseMock = new PaginationResponse<InternListingResponse>(
                new List<InternListingResponse>(),
                pageNum,
                totalPages);

            internsServiceMock
                .Setup(i => i.GetPaginationAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(paginationResponseMock);

            //Act
            var actionResult = await internsController.GetAllAsync(pageNum, pageSize);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var coreResponse = jsonResult!.Value as CoreResponse<PaginationResponse<InternListingResponse>>;

            Assert.NotNull(coreResponse);
            Assert.Equal(paginationResponseMock, coreResponse!.Data);
        }

        [Fact]
        public async Task GetAllAsync_WhenPageNumAndSizeAreNull_ShouldReturnCorrectData()
        {
            //Arrange 
            var internListingResponsesMock = new List<InternListingResponse>()
            {
                new InternListingResponse(
                    internId,
                    displayName,
                    workEmail,
                    new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() })
            };

            internsServiceMock
                .Setup(i => i.GetAllAsync())
                .ReturnsAsync(internListingResponsesMock);

            //Act
            var actionResult = await internsController.GetAllAsync();

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var coreResponse = jsonResult!.Value as CoreResponse<IEnumerable<InternListingResponse>>;

            Assert.NotNull(coreResponse);
            Assert.Single(coreResponse!.Data!);
            Assert.Equal(2, coreResponse!.Data!.First().CampaignIds.Count());
        }

        #endregion

        #region AddInternCampaignAsync

        [Fact]
        public async Task AddInternCampaignAsync_WhenInternIdIsEmpty_ShouldThrowException()
        {
            //Arrange
            var addInternCampaignApiRequest = new AddInternCampaignApiRequest(
                specialityId,
                justification);

            //Act
            var action = async () => await internsController.AddInternCampaignAsync(Guid.Empty, campaignId, addInternCampaignApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task AddInternCampaignAsync_WhenCampaignIdIsEmpty_ShouldThrowException()
        {
            var addInternCampaignApiRequest = new AddInternCampaignApiRequest(
                specialityId,
                justification);

            //Act
            var action = async () => await internsController.AddInternCampaignAsync(internId, Guid.Empty, addInternCampaignApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task AddInternCampaignAsync_WhenSpecialityIdIsEmpty_ShouldThrowException()
        {
            var addInternCampaignApiRequest = new AddInternCampaignApiRequest(
                    Guid.Empty,
                    justification);

            //Act
            var action = async () => await internsController.AddInternCampaignAsync(internId, campaignId, addInternCampaignApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task AddInternCampaignAsync_WhenJustificationIsInvalid_ShouldThrowException()
        {
            var invalidJustification = TestHelper.GenerateString(InternValidationConstants.JustificationMaxLength + 1);

            var addInternCampaignApiRequest = new AddInternCampaignApiRequest(
                specialityId,
                invalidJustification);

            //Act
            var action = async () => await internsController.AddInternCampaignAsync(internId, campaignId, addInternCampaignApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task AddInternCampaignAsync_WhenAllValid_ShouldReturnCorrectObject()
        {
            var addInternCampaignApiRequest = new AddInternCampaignApiRequest(
                specialityId,
                justification);

            var internCampaingResponseMock = new InternCampaignSummaryResponse(
                new CampaignSummaryResponse(Guid.NewGuid(), "Campaing2022", DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(50), true),
                new SpecialitySummaryResponse(Guid.NewGuid(), "Backend"),
                stateResponseMock);

            internsCampaignsServiceMock
                .Setup(i => i.AddInternCampaignAsync(It.IsAny<AddInternCampaignRequest>()))
                .ReturnsAsync(internCampaingResponseMock);

            //Act
            var actionResult = await internsController.AddInternCampaignAsync(internId, campaignId, addInternCampaignApiRequest);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var coreResponse = jsonResult!.Value as CoreResponse<InternCampaignSummaryResponse>;

            Assert.NotNull(coreResponse);
            Assert.Equal(internCampaingResponseMock, coreResponse!.Data);
        }

        #endregion

        #region UpdateInternCampaignAsync

        [Fact]
        public async Task UpdateInternCampaignAsync_WhenInternIdIsEmpty_ShouldThrowException()
        {
            //Arrange
            var updateInternCampaignApiRequest = new UpdateInternCampaignApiRequest(specialityId);

            //Act
            var action = async () => await internsController.UpdateInternCampaignAsync(Guid.Empty, campaignId, updateInternCampaignApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateInternCampaignAsync_WhenCampaignIdIsEmpty_ShouldThrowException()
        {
            var updateInternCampaignApiRequest = new UpdateInternCampaignApiRequest(specialityId);

            //Act
            var action = async () => await internsController.UpdateInternCampaignAsync(internId, Guid.Empty, updateInternCampaignApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateInternCampaignAsync_WhenSpecialityIdIsEmpty_ShouldThrowException()
        {
            var updateInternCampaignApiRequest = new UpdateInternCampaignApiRequest(Guid.Empty);

            //Act
            var action = async () => await internsController.UpdateInternCampaignAsync(internId, campaignId, updateInternCampaignApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateInternCampaignAsync_WhenAllValid_ShouldReturnCorrectObject()
        {
            var updateInternCampaignApiRequest = new UpdateInternCampaignApiRequest(specialityId);

            var internCampaingResponseMock = new InternCampaignSummaryResponse(
                new CampaignSummaryResponse(campaignId, "Campaing2022", DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(50), true),
                new SpecialitySummaryResponse(campaignId, "Backend"),
                stateResponseMock);

            internsCampaignsServiceMock
                .Setup(i => i.UpdateInternCampaignAsync(It.IsAny<UpdateInternCampaignRequest>()))
                .ReturnsAsync(internCampaingResponseMock);

            //Act
            var actionResult = await internsController.UpdateInternCampaignAsync(internId, campaignId, updateInternCampaignApiRequest);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var coreResponse = jsonResult!.Value as CoreResponse<InternCampaignSummaryResponse>;

            Assert.NotNull(coreResponse);
            Assert.Equal(internCampaingResponseMock, coreResponse!.Data);
        }

        #endregion

        #region AddStateAsync

        [Fact]
        public async Task AddStateAsync_WhenInternIdIsEmpty_ShouldThrowExceptionAsync()
        {
            //Arrange
            var addStateApiRequest = new AddStateApiRequest(
                StatusEnum.Rejected,
                justification);

            //Act
            var action = async () => await internsController.AddStateAsync(Guid.Empty, campaignId, addStateApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task AddStateAsync_WhenCampaignIdIsEmpty_ShouldThrowException()
        {
            //Arrange
            var addStateApiRequest = new AddStateApiRequest(
                StatusEnum.Rejected,
                justification);

            //Act
            var action = async () => await internsController.AddStateAsync(internId, Guid.Empty, addStateApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(AddStateInvalidJustification))]
        public async Task AddStateAsync_WhenJustificationIsInvalid_ShouldThrowException(string invalidJustification)
        {
            //Arrange
            var addStateApiRequest = new AddStateApiRequest(
                StatusEnum.Rejected,
                invalidJustification);

            //Act
            var action = async () => await internsController.AddStateAsync(internId, campaignId, addStateApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task AddStateAsync_WhenInternCampaingDoesNotCointainOtherStates_ShouldReturnCorrectObject()
        {
            //Arrange
            var addStateApiRequest = new AddStateApiRequest(
                StatusEnum.Rejected,
                justification);

            var stateResponseMock2 = new StateResponse(
                addStateApiRequest.StatusId.ToString(),
                addStateApiRequest.Justification, 
                DateTime.UtcNow);

            internsCampaignsServiceMock
                .Setup(i => i.AddStateAsync(It.IsAny<AddStateRequest>()))
                .ReturnsAsync(stateResponseMock2);

            //Act
            var actionResult = await internsController.AddStateAsync(internId, campaignId, addStateApiRequest);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var coreResponse = jsonResult!.Value as CoreResponse<StateResponse>;

            Assert.NotNull(coreResponse);
            Assert.Equal(stateResponseMock2, coreResponse!.Data);
        }

        [Fact]
        public async Task AddStateAsync_WhenStatusIsIntern_AndNoEmailIsProvided_ShouldThrowException()
        {
            //Arrange
            var addStateApiRequest = new AddStateApiRequest(
                StatusEnum.Intern,
                justification);

            //Act
            var action = async () => await internsController.AddStateAsync(internId, campaignId, addStateApiRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task AddStateAsync_WhenStatusIsIntern_AndEmailIsProvided_ShouldCallAppropriateMethod()
        {
            //Arrange
            var addStateApiRequest = new AddStateApiRequest(
                StatusEnum.Intern,
                justification,
                workEmail);

            var stateResponseMock2 = new StateResponse(
                addStateApiRequest.StatusId.ToString(),
                addStateApiRequest.Justification,
                DateTime.UtcNow);

            internsCampaignsServiceMock
                .Setup(i => i.AddStateAsync(It.IsAny<AddStateRequest>()))
                .ReturnsAsync(stateResponseMock2);

            //Act
            await internsController.AddStateAsync(internId, campaignId, addStateApiRequest);

            //Assert
            internsServiceMock.Verify(i => i.InviteAsync(It.IsAny<InviteInternRequest>()), Times.Once);
        }

        #endregion

        #region GetAllStatusAsync

        [Fact]
        public async Task GetAllStatusAsync_ShouldReturnCorrectNumberOfElements()
        {
            //Arrange
            var statusResponse1 = new StatusResponse((int)StatusEnum.Candidate, StatusEnum.Candidate.ToString());
            var statusResponse2 = new StatusResponse((int)StatusEnum.RejectedToStart, StatusEnum.RejectedToStart.ToString());

            var statusResponseCollectionMock = new List<StatusResponse>() { statusResponse1, statusResponse2 };

            internsCampaignsServiceMock
                .Setup(i => i.GetAllStatusAsync())
                .ReturnsAsync(statusResponseCollectionMock);

            //Act
            var actionResult = await internsController.GetAllStatusAsync();

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var coreResponse = jsonResult!.Value as CoreResponse<IEnumerable<StatusResponse>>;

            Assert.NotNull(coreResponse);
            Assert.Equal(2, coreResponse!.Data!.Count());
        }

        #endregion
    }
}
