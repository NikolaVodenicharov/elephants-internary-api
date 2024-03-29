﻿using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
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
        private readonly DateTime startDate = DateTime.UtcNow.AddDays(5);
        private readonly DateTime endDate = DateTime.UtcNow.AddDays(50);
        private readonly bool isActive = true;
        private readonly Mock<ICampaignsRepository> campaignsRepositoryMock;
        private readonly CampaignsService campaignsServiceMock;
        private readonly Campaign returnCampaign;
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
            var filterCampaignsRequestValidator = new PaginationRequestValidator();

            var campaignValidator = new CampaignValidator(createCampaignValidator, updateCampaignValidator);

            campaignsRepositoryMock = new Mock<ICampaignsRepository>();

            var mockLogger = new Mock<ILogger<CampaignsService>>();

            campaignsServiceMock = new CampaignsService(campaignsRepositoryMock.Object, mockLogger.Object,
                campaignValidator, filterCampaignsRequestValidator);

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

        [Fact]
        public async Task UpdateAsync_WhenCampaignIsInactiveAndEndDateInThePast_ShouldThrowException()
        {
            //Arrange
            var completedCampaign = new Campaign()
            {
                Id = Guid.NewGuid(), 
                Name = "Completed Campaign",
                StartDate = DateTime.Today.AddDays(-20),
                EndDate = DateTime.Today.AddDays(-15),
                IsActive = false
            };

            campaignsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(completedCampaign);

            var updateRequest = new UpdateCampaignRequest(completedCampaign.Id, "Finished Campaign",
                completedCampaign.StartDate, DateTime.Today.AddDays(2), true);

            //Act
            var action = async () => await campaignsServiceMock.UpdateAsync(updateRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
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
            Assert.Equal(id, campaignSummary!.Id);
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

        #region GetPaginationAsyncTests

        [Theory]
        [InlineData(1, 5)]
        [InlineData(3, 5)]
        [InlineData(2, 10)]
        [InlineData(1, 100)]
        public async Task GetPaginationAsync_WhenFilterIsCorrect_ShouldGetData(int pageNum, int pageSize)
        {
            //Arrange
            var campaignList = new List<Campaign>()
            {
                returnCampaign
            };

            var filter = new PaginationRequest(pageNum, pageSize);

            campaignsRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(campaignList);

            campaignsRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(15);

            //Act
            var response = await campaignsServiceMock.GetPaginationAsync(filter);

            //Assert
            Assert.Equal(campaignList.Count, response.Content.Count());
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GetPaginationAsync_WhenPageNumIsLessThanOne_ShouldThrowException(int pageNum)
        {
            //Arrange
            var filter = new PaginationRequest(pageNum, 10);

            //Act
            var action = async () => await campaignsServiceMock.GetPaginationAsync(filter);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetPaginationAsync_WhenPageSizeIsLessThanOne_ShouldThrowException(int pageSize)
        {
            //Arrange
            var filter = new PaginationRequest(1, pageSize);

            //Act
            var action = async () => await campaignsServiceMock.GetPaginationAsync(filter);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetPaginationAsync_WhenPageNumIsBiggerThanTotalPages_ShouldThrowException()
        {
            //Arrange
            var pageNum = 10;
            var pageSize = 10;
            var count = 1;

            var campaignList = new List<Campaign>()
            {
                returnCampaign
            };

            var filter = new PaginationRequest(pageNum, pageSize);

            campaignsRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(campaignList);

            campaignsRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(count);

            //Act
            var action = async () => await campaignsServiceMock.GetPaginationAsync(filter);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetPaginationAsync_WhenEmptyAndPageNumMoreThanOne_ShouldThrowException()
        {
            //Arrange
            var filter = new PaginationRequest(2, 5);

            campaignsRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(0);

            //Act
            var action = async () => await campaignsServiceMock.GetPaginationAsync(filter);

            //Arrange
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetPaginationAsync_WhenNoCampaignsAndPageNumIsOne_ShouldReturnEmptyCollection()
        {
            //Arrange
            var filter = new PaginationRequest(1, 5);

            campaignsRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(0);

            //Act
            var response = await campaignsServiceMock.GetPaginationAsync(filter);

            //Arrange
            Assert.Empty(response.Content);
        }

        #endregion

        #region GetAllAsyncTests

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            //Act
            var campaigns = await campaignsServiceMock.GetAllAsync();

            //Assert
            Assert.Empty(campaigns);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var campaignList = new List<Campaign>() { returnCampaign };

            campaignsRepositoryMock
                .Setup(x => x.GetAllAsync(null))
                .ReturnsAsync(campaignList);

            //Act
            var response = await campaignsServiceMock.GetAllAsync();

            //Assert
            Assert.Equal(campaignList.Count, response.Count());
        }

        #endregion

        private CreateCampaignRequest CreateValidCreateCampaign()
        {
            //Arrange
            return new CreateCampaignRequest(campaignName, startDate, endDate, isActive);
        }
    }
}
