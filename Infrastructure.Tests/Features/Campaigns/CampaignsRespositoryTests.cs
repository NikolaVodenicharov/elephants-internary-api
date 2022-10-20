using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
using Infrastructure.Features.Campaigns;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Features.Campaigns
{
    public class CampaignsRespositoryTests
    {
        private readonly InternaryContext context;

        private readonly ICampaignsRepository campaignsRepository;

        private readonly Guid id = Guid.NewGuid();
        private readonly string campaignName = "Test Campaign 2022";
        private readonly DateTime startDate = DateTime.Today.AddDays(5);
        private readonly DateTime endDate = DateTime.Today.AddDays(35);
        private readonly bool isActive = true;
        private readonly Campaign campaign;

        public CampaignsRespositoryTests()
        {
            DbContextOptionsBuilder<InternaryContext>? dbOptions = new DbContextOptionsBuilder<InternaryContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString());

            context = new InternaryContext(dbOptions.Options);

            campaignsRepository = new CampaignsRepository(context);

            campaign = new Campaign()
            {
                Id = id,
                Name = campaignName,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = isActive
            };
        }

        [Fact]
        public async Task AddAsync_AddCampaign_ShouldBeAddedToDatabase()
        {
            //Act
            var addedCampaign = await campaignsRepository.AddAsync(campaign);

            var count = await campaignsRepository.GetCountAsync();

            //Assert
            Assert.Equal(1, count);
            Assert.Equal(campaign.Name, addedCampaign.Name);
            Assert.Equal(campaign.StartDate, addedCampaign.StartDate);
            Assert.Equal(campaign.EndDate, addedCampaign.EndDate);
            Assert.Equal(campaign.IsActive, addedCampaign.IsActive);
        }

        [Fact]
        public async Task SaveTrackingChangesAsync_UpdateCampaign_ShouldBeUpdated()
        {
            //Arrange
            var campaignToUpdate = new Campaign()
            {
                Id = Guid.NewGuid(),
                Name = "Upcoming Campaign",
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(70),
                IsActive = true
            };

            await campaignsRepository.AddAsync(campaignToUpdate);

            campaignToUpdate.StartDate = DateTime.Today.AddDays(20);
            campaignToUpdate.EndDate = DateTime.Today.AddDays(90);

            //Act
            await campaignsRepository.SaveTrackingChangesAsync();

            //Assert
            var updatedCampaign = await campaignsRepository.GetByIdAsync(campaignToUpdate.Id);

            Assert.Equal(campaignToUpdate.Id, updatedCampaign!.Id);
            Assert.Equal(campaignToUpdate.Name, updatedCampaign.Name);
            Assert.Equal(campaignToUpdate.StartDate, updatedCampaign.StartDate);
            Assert.Equal(campaignToUpdate.EndDate, updatedCampaign.EndDate);
            Assert.Equal(campaignToUpdate.IsActive, updatedCampaign.IsActive);
        }

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            //Arrange
            var filter = new PaginationRequest(1, 10);

            //Act
            var campaigns = await campaignsRepository.GetAllAsync(filter);

            //Assert
            Assert.Empty(campaigns);
        }

        [Fact]
        public async Task GetByIdAsync_WhenFound_ShouldReturnCorrectData()
        {
            //Arrange
            await campaignsRepository.AddAsync(campaign);

            //Act
            var response = await campaignsRepository.GetByIdAsync(campaign.Id);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(campaign.Id, response!.Id);
            Assert.Equal(campaign.Name, response.Name);
            Assert.Equal(campaign.StartDate, response.StartDate);
            Assert.Equal(campaign.EndDate, response.EndDate);
            Assert.Equal(campaign.IsActive, response.IsActive);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ShouldReturnNull()
        {
            //Act
            var response = await campaignsRepository.GetByIdAsync(Guid.NewGuid());

            //Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task GetCountAsync_WhenNotEmpty_ShouldReturnCorrectCount()
        {
            //Arrange
            await campaignsRepository.AddAsync(campaign);

            //Act
            var count = await campaignsRepository.GetCountAsync();

            //Assert
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetCountAsync_WhenEmpty_ShouldReturnZero()
        {
            //Act
            var count = await campaignsRepository.GetCountAsync();

            //Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task ExistsByNameAsync_WhenFound_ShouldReturnTrue()
        {
            //Arrange
            await campaignsRepository.AddAsync(campaign);

            //Act
            var response = await campaignsRepository.ExistsByNameAsync(campaign.Name);

            //Assert
            Assert.True(response);
        }

        [Fact]
        public async Task ExistsByNameAsync_WhenNotFound_ShouldReturnFalse()
        {
            //Act
            var response = await campaignsRepository.ExistsByNameAsync("New name");

            //Assert
            Assert.False(response);
        }
    }
}
