using Core.Common.Exceptions;
using Core.Features.Campaigns;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.Support;
using FluentValidation;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Features.Campaigns
{
    public class CampaignsServiceTests
    {
        private Guid id = Guid.NewGuid();
        private string campaignName = "CampaignTestName";
        private DateTime startDate = DateTime.UtcNow.AddDays(5);
        private DateTime endDate = DateTime.UtcNow.AddDays(50);
        private bool isActive = false;

        [Fact]
        public async Task CreateAsync_CallCorrectRepositoryMethod()
        {
            CreateCampaign createCampaign = CreateValidCreateCampaign();

            var returnCampaign = new Campaign()
            {
                Id = id,
                Name = campaignName,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = isActive
            };

            var repositoryMock = new Mock<ICampaignsRepository>();

            repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Campaign>()))
                .ReturnsAsync(returnCampaign);

            var systemUnderTest = new CampaignsService(repositoryMock.Object);

            //Act
            await systemUnderTest.CreateAsync(createCampaign);

            //Arrange
            repositoryMock.Verify(r => r.AddAsync(It.IsAny<Campaign>()), Times.Once());
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCorrectData()
        {
            //Arrange
            var createCampaign = CreateValidCreateCampaign();

            var returnCampaign = new Campaign()
            {
                Id = id,
                Name = campaignName,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = isActive
            };

            var repositoryMock = new Mock<ICampaignsRepository>();

            repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Campaign>()))
                .ReturnsAsync(returnCampaign);

            var systemUnderTest = new CampaignsService(repositoryMock.Object);

            //Act
            var campaignSummary = await systemUnderTest.CreateAsync(createCampaign);

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

            var repositoryMock = new Mock<ICampaignsRepository>();

            repositoryMock
                .Setup(x => x.ExistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var systemUnderTest = new CampaignsService(repositoryMock.Object);

            //Act
            var action = async () => await systemUnderTest.CreateAsync(createCampaign);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenNameIsLessThanMinLength_ShouldThrowException()
        {
            //Arrange
            var nameLessThanMinlength = TestHelper.GenerateString(CampaignValidationConstants.NameMinLength - 1);

            var createCampaign = new CreateCampaign(nameLessThanMinlength, startDate, endDate, isActive);

            var repositoryMock = new Mock<ICampaignsRepository>();

            var systemUnderTest = new CampaignsService(repositoryMock.Object);

            //Act
            var action = async () => await systemUnderTest.CreateAsync(createCampaign);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenNameIsMoreThanMaxLength_ShouldThrowException()
        {
            //Arrange
            var nameLongerThanMaxlength = TestHelper.GenerateString(CampaignValidationConstants.NameMaxLength + 1);

            var createCampaign = new CreateCampaign(nameLongerThanMaxlength, startDate, endDate, isActive);

            var repositoryMock = new Mock<ICampaignsRepository>();

            var systemUnderTest = new CampaignsService(repositoryMock.Object);

            //Act
            var action = async () => await systemUnderTest.CreateAsync(createCampaign);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenEndDateIsBeforeStartDate_ShouldThrowException()
        {
            //Arrange
            var startDateBiggerThanEndDate = endDate.AddDays(5);

            var createCampaign = new CreateCampaign(campaignName, startDateBiggerThanEndDate, endDate, isActive);

            var repositoryMock = new Mock<ICampaignsRepository>();

            var systemUnderTest = new CampaignsService(repositoryMock.Object);

            //Act
            var action = async () => await systemUnderTest.CreateAsync(createCampaign);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        private CreateCampaign CreateValidCreateCampaign()
        {
            //Arrange
            return new CreateCampaign(campaignName, startDate, endDate, isActive);
        }
    }
}
