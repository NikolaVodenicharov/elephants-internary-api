using Core.Common.Exceptions;
using Core.Features.Campaigns;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.Support;
using FluentValidation;
using Moq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        private bool isActive = false;
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
        
        private Mock<ICampaignsRepository> campaignsRepositoryMock;
        private Mock<ILogger<CampaignsService>> campaignsServiceLoggerMock;
        private CampaignsService campaignsService;

        public CampaignsServiceTests()
        {
            returnCampaign = new Campaign
            {
                Id = id,
                Name = campaignName,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = isActive
            };

            campaignsRepositoryMock = new Mock<ICampaignsRepository>();
            campaignsServiceLoggerMock = new Mock<ILogger<CampaignsService>>();
            
            campaignsService = new CampaignsService(campaignsRepositoryMock.Object, campaignsServiceLoggerMock.Object);
        }

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_CallCorrectRepositoryMethod()
        {
            // Arrange
            CreateCampaign createCampaign = CreateValidCreateCampaign();
            
            campaignsRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Campaign>()))
                .ReturnsAsync(returnCampaign);

            // Act
            await campaignsService.CreateAsync(createCampaign);

            // Assert
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

            // Act
            var campaignSummary = await campaignsService.CreateAsync(createCampaign);

            // Assert
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
            var action = async () => await campaignsService.CreateAsync(createCampaign);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }
        
        [Theory]
        [MemberData(nameof(campaignNameTestData))]
        public async Task CreateAsync_WhenNameIsInvalidLength_ShouldThrowException(string invalidCampaignName)
        {
            // Arrange
            var createCampaign = new CreateCampaign(invalidCampaignName, startDate, endDate, isActive);

            // Act
            var action = async () => await campaignsService.CreateAsync(createCampaign);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(endDateTestData))]
        public async Task CreateAsync_WhenEndDateIsInvalid_ShouldThrowException(DateTime testStartDate, DateTime invalidEndDate)
        {
            // Arrange
            var createCampaign = new CreateCampaign(campaignName, testStartDate, invalidEndDate, isActive);

            // Act
            var action = async () => await campaignsService.CreateAsync(createCampaign);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        private CreateCampaign CreateValidCreateCampaign()
        {
            //Arrange
            return new CreateCampaign(campaignName, startDate, endDate, isActive);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateSuccessfully()
        {
            // Arrange
            var updateCampaign = new UpdateCampaign(id, campaignName, startDate, endDate, isActive);

            campaignsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(returnCampaign);
            campaignsRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Campaign>()))
                .ReturnsAsync(returnCampaign);
        
            // Act
            var actualModel = await campaignsService.UpdateAsync(updateCampaign);

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
            var updateCampaign = new UpdateCampaign(id, campaignName, startDate, endDate, isActive);

            // Act    
            var action = async() => await campaignsService.UpdateAsync(updateCampaign);
            
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
            var updateCampaign = new UpdateCampaign(id, updatedName, startDate, endDate, isActive);

            // Act
            var action = async() => await campaignsService.UpdateAsync(updateCampaign);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Theory]
        [MemberData(nameof(campaignNameTestData))]
        public async Task UpdateAsync_WhenNameHasInvalidLength_ShouldThrowException(string invalidCampaignName)
        {
            // Arrange
            var updateCampaign = new UpdateCampaign(id, invalidCampaignName, startDate, endDate, isActive);

            //Act
            var action = async() => await campaignsService.UpdateAsync(updateCampaign);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(endDateTestData))]
        public async Task UpdateAsync_WhenEndDateIsInvalid_ShouldThrowException(DateTime testStartDate, DateTime invalidEndDate)
        {
            // Arrange
            var updateCampaign = new UpdateCampaign(id, campaignName, testStartDate, invalidEndDate, isActive);
            
            // Act
            var action = async() => await campaignsService.UpdateAsync(updateCampaign);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }
        
        #endregion
    }
}
