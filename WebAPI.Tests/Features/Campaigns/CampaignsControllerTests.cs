using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
using Core.Features.Campaigns.Support;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Specialities.ResponseModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Tests.Common;
using WebAPI.Features.Campaigns;
using Xunit;

namespace WebAPI.Tests.Features.Campaigns
{
    public class CampaignsControllerTests
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly string campaignName = "Summer Campaign 2022";
        private readonly DateTime campaignStartDate = DateTime.Today;
        private readonly DateTime campaignEndDate = DateTime.Today.AddDays(30);
        private readonly bool campaignIsActive = true;

        private CreateCampaignRequest createCampaignRequest;
        private UpdateCampaignRequest updateCampaignRequest;
        private CampaignSummaryResponse campaignSummary;

        private readonly Mock<ICampaignsService> campaignsServiceMock;
        private readonly Mock<IMentorsService> mentorsServiceMock;
        private readonly CampaignsController campaignsController;

        public static IEnumerable<object[]> invalidNames =>
            new List<object[]>
            {
                new object[] { TestHelper.GenerateString(CampaignValidationConstants.NameMinLength - 1) },
                new object[] { TestHelper.GenerateString(CampaignValidationConstants.NameMaxLength + 1) },
                new object[] { " Campaign" },
                new object[] { "Test " },
                new object[] { "-Test-" },
            };

        public static IEnumerable<object[]> invalidDates =>
            new List<object[]>
            {
                new object[] {DateTime.Today.AddDays(3), DateTime.Today.AddDays(1),},
                new object[] {DateTime.Today.AddDays(1), DateTime.Today.AddDays(-1),},
            };

        public CampaignsControllerTests()
        {
            var createCampaignRequestValidator = new CreateCampaignRequestValidator();
            var updateCampaignRequestValidator = new UpdateCampaignRequestValidator();
            var paginationFilterRequestValidator = new PaginationFilterRequestValidator();

            var loggerMock = new Mock<ILogger<CampaignsController>>();

            campaignsServiceMock = new Mock<ICampaignsService>();
            mentorsServiceMock = new Mock<IMentorsService>();

            campaignsController = new CampaignsController(
                    campaignsServiceMock.Object,
                    mentorsServiceMock.Object,
                    createCampaignRequestValidator,
                    updateCampaignRequestValidator,
                    paginationFilterRequestValidator,
                    loggerMock.Object
                );

            createCampaignRequest = new CreateCampaignRequest(campaignName, campaignStartDate, campaignEndDate, campaignIsActive);

            updateCampaignRequest = new UpdateCampaignRequest(id, campaignName, campaignStartDate, campaignEndDate, campaignIsActive);

            campaignSummary = new CampaignSummaryResponse(id, campaignName, campaignStartDate, campaignEndDate, campaignIsActive);
        }

        #region CreateAsyncTests

        [Fact]
        public async Task CreateAsync_WhenDataIsCorrect_ShouldReturnCorrectData()
        {
            //Arrange
            campaignsServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateCampaignRequest>()))
                .ReturnsAsync(campaignSummary);

            //Act
            var actionResult = await campaignsController.CreateAsync(createCampaignRequest);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var createdCampaign = jsonResult!.Value as CoreResponse<CampaignSummaryResponse>;

            Assert.Equal(campaignSummary.Id, createdCampaign.Data.Id);
            Assert.Equal(campaignSummary.Name, createdCampaign.Data.Name);
            Assert.Equal(campaignSummary.StartDate, createdCampaign.Data.StartDate);
            Assert.Equal(campaignSummary.EndDate, createdCampaign.Data.EndDate);
            Assert.Equal(campaignSummary.IsActive, createdCampaign.Data.IsActive);
        }

        [Theory]
        [MemberData(nameof(invalidNames))]
        public async Task CreateAsync_WhenNameIsInvalid_ShouldThrowException(string invalidName)
        {
            //Arrange
            var request = new CreateCampaignRequest(invalidName, campaignStartDate, campaignEndDate, campaignIsActive);

            //Act
            var action = async () => await campaignsController.CreateAsync(request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenStartDateIsInvalid_ShouldThrowException()
        {
            //Arrange
            var request = new CreateCampaignRequest(campaignName, DateTime.Today.AddDays(8), 
                DateTime.Today.AddDays(5), campaignIsActive);

            //Act
            var action = async () => await campaignsController.CreateAsync(request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(invalidDates))]
        public async Task CreateAsync_WhenEndDateIsInvalid_ShouldThrowError(DateTime startDate, DateTime endDate)
        {
            //Arrange
            var request = new CreateCampaignRequest(campaignName, startDate, endDate, campaignIsActive);

            //Act
            var action = async () => await campaignsController.CreateAsync(request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        #endregion

        #region UpdateAsyncTests

        [Fact]
        public async Task UpdateAsync_WhenDataIsCorrect_ShouldReturnCorrectData()
        {
            //Arrange
            campaignsServiceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateCampaignRequest>()))
                .ReturnsAsync(campaignSummary);

            //Act
            var actionResult = await campaignsController.UpdateAsync(updateCampaignRequest.Id, updateCampaignRequest);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var createdCampaign = jsonResult!.Value as CoreResponse<CampaignSummaryResponse>;

            Assert.Equal(campaignSummary.Id, createdCampaign.Data.Id);
            Assert.Equal(campaignSummary.Name, createdCampaign.Data.Name);
            Assert.Equal(campaignSummary.StartDate, createdCampaign.Data.StartDate);
            Assert.Equal(campaignSummary.EndDate, createdCampaign.Data.EndDate);
            Assert.Equal(campaignSummary.IsActive, createdCampaign.Data.IsActive);
        }

        [Fact]
        public async Task UpdateAsync_WhenQueryIdAndModelIdDoesNotMatch_ShouldThrowException()
        {
            //Act
            var action = async () => await campaignsController.UpdateAsync(Guid.NewGuid(), updateCampaignRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Theory]
        [MemberData(nameof(invalidNames))]
        public async Task UpdateAsync_WhenNameIsInvalid_ShouldThrowException(string invalidName)
        {
            //Arrange
            var request = new UpdateCampaignRequest(id, invalidName, campaignStartDate, campaignEndDate, campaignIsActive);

            //Act
            var action = async () => await campaignsController.UpdateAsync(request.Id, request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenStartDateIsInvalid_ShouldThrowException()
        {
            //Arrange
            var request = new UpdateCampaignRequest(id, campaignName, DateTime.Today.AddDays(10), 
                DateTime.Today.AddDays(5), campaignIsActive);

            //Act
            var action = async () => await campaignsController.UpdateAsync(request.Id, request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(invalidDates))]
        public async Task UpdateAsync_WhenEndDateIsInvalid_ShouldThrowException(DateTime startDate, DateTime endDate)
        {
            //Arrange
            var request = new UpdateCampaignRequest(id, campaignName, startDate, endDate, campaignIsActive);

            //Act
            var action = async () => await campaignsController.UpdateAsync(request.Id, request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenCampaignIsInactive_ShouldThrowException()
        {
            //Arrange
            var request = new UpdateCampaignRequest(id, campaignName, campaignStartDate, campaignEndDate, false);

            campaignsServiceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateCampaignRequest>()))
                .ThrowsAsync(new CoreException(It.IsAny<string>(), It.IsAny<System.Net.HttpStatusCode>()));

            //Act
            var action = async () => await campaignsController.UpdateAsync(request.Id, request);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        #endregion

        #region GetByIdAsyncTests

        [Fact]
        public async Task GetByIdAsync_WhenIdFound_ShouldReturnCorrectData()
        {
            //Arrange
            campaignsServiceMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaignSummary);

            //Act
            var actionResult = await campaignsController.GetByIdAsync(id);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var response = jsonResult!.Value as CoreResponse<CampaignSummaryResponse>;

            //Assert
            Assert.Equal(campaignSummary.Id, response.Data.Id);
            Assert.Equal(campaignSummary.Name, response.Data.Name);
            Assert.Equal(campaignSummary.StartDate, response.Data.StartDate);
            Assert.Equal(campaignSummary.EndDate, response.Data.EndDate);
            Assert.Equal(campaignSummary.IsActive, response.Data.IsActive);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdNotFound_ShouldThrowException()
        {
            //Arrange
            campaignsServiceMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new CoreException(It.IsAny<string>(), It.IsAny<System.Net.HttpStatusCode>()));

            //Act
            var action = async () => await campaignsController.GetByIdAsync(Guid.NewGuid());

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        #endregion

        #region GetPageAsyncTests

        [Fact]
        public async Task GetPageAsync_WhenNotEmpty_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var campaignSummary2 = new CampaignSummaryResponse(Guid.NewGuid(), "Campaign 2", 
                DateTime.Today.AddDays(40), DateTime.Today.AddDays(71), true);

            var expectedResponse = new List<CampaignSummaryResponse>() { campaignSummary, campaignSummary2 };

            campaignsServiceMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(expectedResponse.Count);

            campaignsServiceMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationFilterRequest>()))
                .ReturnsAsync(expectedResponse);

            var validPageNum = 1;
            var validPageSize = 10;

            //Act
            var actionResult = await campaignsController.GetPageAsync(validPageNum, validPageSize);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var actualResponse = jsonResult!.Value as CoreResponse<PaginationResponse<CampaignSummaryResponse>>;

            Assert.Equal(expectedResponse.Count, actualResponse.Data.Content.Count());
        }

        [Fact]
        public async Task GetPageAsync_WhenEmpty_ShouldThrowException()
        {
            //Arrange
            var validPageNum = 1;
            var validPageSize = 10;

            campaignsServiceMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(0);

            //Act
            var actionResult = async () => await campaignsController.GetPageAsync(validPageNum, validPageSize);

            //Assert
            await Assert.ThrowsAsync<CoreException>(actionResult);
        }

        [Fact]
        public async Task GetPageAsync_WhenPageNumIsLessThanOne_ShouldThrowException()
        {
            //Arrange
            var invalidPageNum = 0;
            var validPageSize = 10;

            campaignsServiceMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(15);

            //Act
            var action = async () => await campaignsController.GetPageAsync(invalidPageNum, validPageSize);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetPageAsync_WhenPageNumIsBiggerThanTotalCount_ShouldThrowException()
        {
            //Arrange
            var invalidPageNum = 7;
            var validPageSize = 10;

            campaignsServiceMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(15);

            //Act
            var action = async() => await campaignsController.GetPageAsync(invalidPageNum, validPageSize);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetPageAsync_WhenPageSizeIsLessThanOne_ShouldThrowException()
        {
            var validPageNum = 1;
            var invalidPageSize = 0;

            campaignsServiceMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(10);

            //Act
            var action = async () => await campaignsController.GetPageAsync(validPageNum, invalidPageSize);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        #endregion

        #region GetMentorsByCampaignIdAsyncTests

        [Fact]
        public async Task GetMentorsByCampaignIdAsync_WhenMentorsFound_ShouldReturnCorrectCount()
        {
            //Arrange
            var specialtySummary = new SpecialitySummaryResponse(Guid.NewGuid(), "Backend");

            var mentorSummary = new MentorSummaryResponse(Guid.NewGuid(), "John", "Doe", "john.doe@endava.com",
                new List<SpecialitySummaryResponse>() { specialtySummary });

            var expectedResponse = new List<MentorSummaryResponse>() { mentorSummary };


            mentorsServiceMock
                .Setup(x => x.GetCountByCampaignIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(1);
            
            mentorsServiceMock
                .Setup(x => x.GetMentorsByCampaignIdAsync(It.IsAny<Guid>(), It.IsAny<PaginationFilterRequest>()))
                .ReturnsAsync(expectedResponse);

            var validPageNum = 1;
            var validPageSize = 10;

            //Act
            var actionResult = await campaignsController.GetMentorsByCampaignIdAsync(id, validPageNum, validPageSize);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var actualResponse = jsonResult!.Value as CoreResponse<PaginationResponse<MentorSummaryResponse>>;

            Assert.Equal(expectedResponse.Count, actualResponse.Data.Content.Count());
        }

        [Fact]
        public async Task GetMentorsByCampaignIdAsync_WhenNoMentorsFound_ShouldThrowException()
        {
            //Arrange
            mentorsServiceMock
                .Setup(x => x.GetCountByCampaignIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(0);

            var validPageNum = 1;
            var validPageSize = 5;

            //Act
            var action = async () => await campaignsController.GetMentorsByCampaignIdAsync(id, validPageNum, validPageSize);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetMentorsByCampaignIdAsync_WhenPageNumIsLessThanOne_ShouldThrowException()
        {
            //Arrange
            mentorsServiceMock
                .Setup(x => x.GetCountByCampaignIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(2);

            var invalidPageNum = 0;
            var validPageSize = 4;

            //Act
            var action = async () => await campaignsController.GetMentorsByCampaignIdAsync(id, invalidPageNum, validPageSize);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetMentorsByCampaignIdAsync_WhenPageNumIsBiggerThanTotalCount_ShouldThrowException()
        {
            //Arrange
            mentorsServiceMock
                .Setup(x => x.GetCountByCampaignIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(10);

            var invalidPageNum = 3;
            var validPageSize = 10;

            //Act
            var action = async () => await campaignsController.GetMentorsByCampaignIdAsync(id, invalidPageNum, validPageSize);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetMentorsByCampaignIdAsync_WhenPageSizeIsLessThanOne_ShouldThrowException()
        {
            //Arrange
            mentorsServiceMock
                .Setup(x => x.GetCountByCampaignIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(15);

            var validPageNum = 1;
            var invalidPageSize = 0;

            //Act
            var action = async () => await campaignsController.GetMentorsByCampaignIdAsync(id, validPageNum, invalidPageSize);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        #endregion
    }
}
