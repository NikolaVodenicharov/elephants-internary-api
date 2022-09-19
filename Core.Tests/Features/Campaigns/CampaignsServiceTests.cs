using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.Support;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Features.Campaigns
{
    public class CampaignsServiceTests
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly string campaignName = "CampaignTestName";
        private DateTime startDate = DateTime.UtcNow.AddDays(5);
        private DateTime endDate = DateTime.UtcNow.AddDays(50);
        private bool isActive = true;
        private Mock<ICampaignsRepository> campaignsRepositoryMock;
        private CampaignsService campaignsServiceMock;
        private Campaign returnCampaign;
        public static IEnumerable<object[]> endDateTestData =>
            new List<object[]>
            {
                new object[] { DateTime.UtcNow.AddDays(-3), DateTime.UtcNow.AddDays(-1) },
                new object[] { DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(3) }
            };

        public static IEnumerable<object[]> campaignNameTestData =>
            new List<object[]>
            {
                new object[] { TestHelper.GenerateString(CampaignValidationConstants.NameMinLength - 1) },
                new object[] { TestHelper.GenerateString(CampaignValidationConstants.NameMaxLength + 1) },
            };

        public CampaignsServiceTests()
        {
            var createCampaignValidator = new CreateCampaignRequestValidator();
            var updateCampaignValidator = new UpdateCampaignRequestValidator();
            var filterCampaignsRequestValidator = new PaginationFilterRequestValidator();

            campaignsRepositoryMock = new Mock<ICampaignsRepository>();

            var mockLogger = new Mock<ILogger<CampaignsService>>();

            campaignsServiceMock = new CampaignsService(campaignsRepositoryMock.Object, mockLogger.Object,
                createCampaignValidator, updateCampaignValidator, filterCampaignsRequestValidator);

            returnCampaign = new Campaign
            {
                Id = id,
                Name = campaignName,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = isActive
            };

        }

        #region CreateAsyncTests

        [Fact]
        public async Task CreateAsync_CallCorrectRepositoryMethod()
        {
            //Arrange
            CreateCampaignRequest createCampaign = CreateValidCreateCampaign();

            campaignsRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Campaign>()))
                .ReturnsAsync(returnCampaign);

            //Act
            await campaignsServiceMock.CreateAsync(createCampaign);

            //Assert
            campaignsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Campaign>()), Times.Once());
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCorrectData()
        {
            // Arrange
            var createCampaign = CreateValidCreateCampaign();

            campaignsRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Campaign>()))
                .ReturnsAsync(returnCampaign);

            //Act
            var campaignSummary = await campaignsServiceMock.CreateAsync(createCampaign);

            //Arrange
            Assert.Equal(campaignName, campaignSummary.Name);
            Assert.Equal(startDate, campaignSummary.StartDate);
            Assert.Equal(endDate, campaignSummary.EndDate);
            Assert.Equal(isActive, campaignSummary.IsActive);
        }

        [Fact]
        public async Task CreateAsync_WhenNameIsDuplicated_ShouldThrowException()
        {
            //Arrange
            var createCampaign = CreateValidCreateCampaign();

            campaignsRepositoryMock
                .Setup(x => x.ExistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            //Act
            var action = async () => await campaignsServiceMock.CreateAsync(createCampaign);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Theory]
        [MemberData(nameof(campaignNameTestData))]
        public async Task CreateAsync_WhenNameIsInvalidLength_ShouldThrowException(string invalidCampaignName)
        {
            // Arrange
            var createCampaign = new CreateCampaignRequest(invalidCampaignName, startDate, endDate, isActive);

            // Act
            var action = async () => await campaignsServiceMock.CreateAsync(createCampaign);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(endDateTestData))]
        public async Task CreateAsync_WhenEndDateIsInvalid_ShouldThrowException(DateTime testStartDate, DateTime invalidEndDate)
        {
            // Arrange
            var createCampaign = new CreateCampaignRequest(campaignName, testStartDate, invalidEndDate, isActive);

            // Act
            var action = async () => await campaignsServiceMock.CreateAsync(createCampaign);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        #endregion

        #region UpdateAsyncTests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateSuccessfully()
        {
            // Arrange
            var updateCampaign = new UpdateCampaignRequest(id, campaignName, startDate, endDate, isActive);

            campaignsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(returnCampaign);

            // Act
            var actualModel = await campaignsServiceMock.UpdateAsync(updateCampaign);

            // Assert
            Assert.NotNull(actualModel);
            Assert.Equal(returnCampaign.Id, actualModel.Id);
            Assert.Equal(returnCampaign.Name, actualModel.Name);
            Assert.Equal(returnCampaign.StartDate, actualModel.StartDate);
            Assert.Equal(returnCampaign.EndDate, actualModel.EndDate);
            Assert.Equal(returnCampaign.IsActive, actualModel.IsActive);
        }

        [Fact]
        public async Task UpdateAsync_WhenIdNotExist_ShouldThrowException()
        {
            // Arrange
            var updateCampaign = new UpdateCampaignRequest(id, campaignName, startDate, endDate, isActive);

            // Act    
            var action = async () => await campaignsServiceMock.UpdateAsync(updateCampaign);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenDuplicateName_ShouldThrowException()
        {
            // Arrange
            campaignsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(returnCampaign);

            campaignsRepositoryMock
                .Setup(x => x.ExistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var updatedName = campaignName + "-Updated";
            var updateCampaign = new UpdateCampaignRequest(id, updatedName, startDate, endDate, isActive);

            // Act
            var action = async () => await campaignsServiceMock.UpdateAsync(updateCampaign);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Theory]
        [MemberData(nameof(campaignNameTestData))]
        public async Task UpdateAsync_WhenNameHasInvalidLength_ShouldThrowException(string invalidCampaignName)
        {
            // Arrange
            var updateCampaign = new UpdateCampaignRequest(id, invalidCampaignName, startDate, endDate, isActive);

            //Act
            var action = async () => await campaignsServiceMock.UpdateAsync(updateCampaign);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(endDateTestData))]
        public async Task UpdateAsync_WhenEndDateIsInvalid_ShouldThrowException(DateTime testStartDate, DateTime invalidEndDate)
        {
            // Arrange
            var updateCampaign = new UpdateCampaignRequest(id, campaignName, testStartDate, invalidEndDate, isActive);

            // Act
            var action = async () => await campaignsServiceMock.UpdateAsync(updateCampaign);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        #endregion

        #region GetByIdAsyncTests

        [Fact]
        public async Task GetByIdAsync_WhenIdExists_ShouldReturnCorrectData()
        {
            //Arrange
            campaignsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(returnCampaign);

            //Act
            var campaignSummary = await campaignsServiceMock.GetByIdAsync(id);

            //Assert
            Assert.NotNull(campaignSummary);
            Assert.Equal(id, campaignSummary.Id);
            Assert.Equal(campaignName, campaignSummary.Name);
            Assert.Equal(startDate, campaignSummary.StartDate);
            Assert.Equal(endDate, campaignSummary.EndDate);
            Assert.Equal(isActive, campaignSummary.IsActive);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdNotFound_ShouldThrowException()
        {
            //Arrange
            var campaignId = Guid.NewGuid();

            //Act
            var action = async () => await campaignsServiceMock.GetByIdAsync(campaignId);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        #endregion

        #region GetAllAsyncTests

        [Theory]
        [InlineData(0, 5, 15)]
        [InlineData(5, 5, 15)]
        [InlineData(10, 5, 15)]
        [InlineData(0, 20, 5)]
        [InlineData(1, 1, 10)]
        public async Task GetAllAsync_WhenFilterIsCorrect_ShouldGetData(int skip, int take, int count)
        {
            //Arrange
            var campaignList = new List<Campaign>()
            {
                returnCampaign
            };

            var filter = new PaginationFilterRequest()
            {
                Skip = skip,
                Take = take,
                Count = count
            };

            campaignsRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationFilterRequest>()))
                .ReturnsAsync(campaignList);

            //Act
            var campaigns = (await campaignsServiceMock.GetAllAsync(filter)).ToList();

            //Assert
            Assert.Equal(campaignList.Count, campaigns.Count());
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(10)]
        [InlineData(20)]
        public async Task GetAllAsync_WhenSkipIsInvalid_ShouldThrowException(int invalidSkip)
        {
            //Arrange
            var filter = new PaginationFilterRequest()
            {
                Skip = invalidSkip,
                Take = 10,
                Count = 10
            };

            //Act
            var action = async () => await campaignsServiceMock.GetAllAsync(filter);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetAllAsync_WhenTakeIsLessThanOne_ShouldThrowException(int invalidTake)
        {
            //Arrange
            var filter = new PaginationFilterRequest()
            {
                Skip = 0,
                Take = invalidTake,
                Count = 10
            };

            //Act
            var action = async () => await campaignsServiceMock.GetAllAsync(filter);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        #endregion

        private CreateCampaignRequest CreateValidCreateCampaign()
        {
            //Arrange
            return new CreateCampaignRequest(campaignName, startDate, endDate, isActive);
        }
    }
}
