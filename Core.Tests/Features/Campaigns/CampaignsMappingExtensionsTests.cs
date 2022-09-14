using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.Support;
using System;
using Xunit;

namespace Core.Tests.Features.Campaigns
{
    public class CampaignsMappingExtensionsTests
    {
        private Guid id = Guid.NewGuid();
        private string campaignName = "CampaignTestName";
        private DateTime startDate = DateTime.UtcNow.AddDays(5);
        private DateTime endDate = DateTime.UtcNow.AddDays(50);
        private bool isActive = false;

        [Fact]
        public void CreateCampaign_ToCampaign_CreateCorrectObject()
        {
            //Arrange
            var systemUnderTest = new CreateCampaignRequest(campaignName, startDate, endDate, isActive);

            //Act
            var campaing = systemUnderTest.ToCampaign();

            //Assert
            Assert.Equal(campaignName, campaing.Name);
            Assert.Equal(startDate, campaing.StartDate);
            Assert.Equal(endDate, campaing.EndDate);
            Assert.Equal(isActive, campaing.IsActive);
        }

        [Fact]
        public void Campaign_ToCreateCampaign_CreateCorrectObject()
        {
            //Arrange
            var systemUnderTest = new Campaign()
            {
                Id = Guid.NewGuid(),
                Name = campaignName,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = isActive
            };

            //Act
            var campaingSummary = systemUnderTest.ToCampaignSummary();

            //Assert
            Assert.Equal(campaignName, campaingSummary.Name);
            Assert.Equal(startDate, campaingSummary.StartDate);
            Assert.Equal(endDate, campaingSummary.EndDate);
            Assert.Equal(isActive, campaingSummary.IsActive);
        }

        [Fact]
        public void ToCampaign_WithUpdateCampaignModel_ShouldReturnCorrectObject()
        {
            // Arrange
            var expectedModel = new UpdateCampaignRequest(id, campaignName, startDate, endDate, isActive);
            
            // Act
            var actualModel = expectedModel.ToCampaign();

            // Assert
            Assert.NotNull(actualModel);
            Assert.Equal(expectedModel.Id, actualModel.Id);
            Assert.Equal(expectedModel.StartDate, actualModel.StartDate);
            Assert.Equal(expectedModel.EndDate, actualModel.EndDate);
            Assert.Equal(expectedModel.IsActive, actualModel.IsActive);
        }
    }
}
