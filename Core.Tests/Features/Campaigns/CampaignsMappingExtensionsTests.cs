using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.Support;
using System;
using Xunit;

namespace Core.Tests.Features.Campaigns
{
    public class CampaignsMappingExtensionsTests
    {
        private string campaignName = "CampaignTestName";
        private DateTime startDate = new DateTime(2000, 10, 30);
        private DateTime endDate = new DateTime(2001, 1, 20);
        private bool isActive = false;

        [Fact]
        public void CreateCampaign_ToCampaign_CreateCorrectObject()
        {
            //Arrange
            var systemUnderTest = new CreateCampaign(campaignName, startDate, endDate, isActive);

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
    }
}
