using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Mentors.Entities;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Mentors.Support;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialties.Entities;
using Infrastructure.Features.Campaigns;
using Infrastructure.Features.Mentors;
using Infrastructure.Features.Specialities;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Features.Mentors
{
    public class MentorsRepositoryTests
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly string displayName = "First Last";
        private readonly string email = "first.last@email.com";
        private readonly InternaryContext context;
        private readonly IMentorsRepository mentorsRepository;
        private readonly ISpecialitiesRepository specialitiesRepository;
        private readonly ICampaignsRepository campaignsRepository;
        private Mentor mentor;
        private Speciality speciality;
        private List<Speciality> specialities;

        public MentorsRepositoryTests()
        {
            DbContextOptionsBuilder<InternaryContext>? dbOptions = new DbContextOptionsBuilder<InternaryContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString());

            context = new InternaryContext(dbOptions.Options);

            mentorsRepository = new MentorsRepository(context);
            specialitiesRepository = new SpecialitiesRepository(context);
            campaignsRepository = new CampaignsRepository(context);

            speciality = new Speciality()
            {
                Id = Guid.NewGuid(),
                Name = "Testing"
            };

            specialities = new List<Speciality>()
            {
                speciality
            };

            mentor = new Mentor()
            {
                Id = id,
                DisplayName = displayName,
                Email = email,
                Specialities = specialities
            };
        }

        [Fact]
        public async Task AddAsync_AddMentor_ShouldBeAddedToDatabase()
        {
            //Act
            var addedMentor = await mentorsRepository.AddAsync(mentor);

            var count = await mentorsRepository.GetCountAsync();

            //Assert
            Assert.Equal(1, count);
            Assert.Equal(mentor.DisplayName, addedMentor.DisplayName);
            Assert.Equal(mentor.Email, addedMentor.Email);
        }

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            //Arrange
            var filter = new PaginationRequest(1, 10);

            //Act
            var response = await mentorsRepository.GetAllAsync(filter);

            //Assert
            Assert.Empty(response);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var mentor2 = new Mentor()
            {
                Id = Guid.NewGuid(),
                DisplayName = "John Doe",
                Email = "john.doe@email.com"
            };

            var filter = new PaginationRequest(1, 3);

            await specialitiesRepository.AddAsync(speciality);
            await mentorsRepository.AddAsync(mentor);
            await mentorsRepository.AddAsync(mentor2);

            //Act
            var response = await mentorsRepository.GetAllAsync(filter);

            //Assert
            Assert.Equal(2, response.Count());
            Assert.NotNull(response.First().Specialities);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdExists_ShouldReturnCorrectObject()
        {
            //Arrange
            await specialitiesRepository.AddAsync(speciality);

            await mentorsRepository.AddAsync(mentor);

            //Act
            var response = await mentorsRepository.GetByIdAsync(mentor.Id);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(mentor.Id, response.Id);
            Assert.Equal(mentor.DisplayName, response.DisplayName);
            Assert.Equal(mentor.Email, response.Email);
            Assert.NotNull(response.Specialities);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdNotFound_ShouldReturnNull()
        {
            //Act
            var response = await mentorsRepository.GetByIdAsync(Guid.NewGuid());

            //Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task GetCountAsync_WhenNotEmpty_ShouldReturnCorrectCount()
        {
            //Arrange
            await mentorsRepository.AddAsync(mentor);

            //Act
            var count = await mentorsRepository.GetCountAsync();

            //Assert
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetCountAsync_WhenEmpty_ShouldReturnCorrectCount()
        {
            //Act
            var count = await mentorsRepository.GetCountAsync();

            //Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task IsEmailUsed_WhenInUse_ShouldReturnTrue()
        {
            //Arrange
            await mentorsRepository.AddAsync(mentor);

            //Act
            var response = await mentorsRepository.IsEmailUsed(mentor.Email);

            //Assert
            Assert.True(response);
        }

        [Fact]
        public async Task IsEmailUsed_WhenNotInUse_ShouldReturnFalse()
        {
            //Arrange
            await mentorsRepository.AddAsync(mentor);

            //Act
            var response = await mentorsRepository.IsEmailUsed("new.email@test.com");

            //Assert
            Assert.False(response);
        }

        [Fact]
        public async Task GetAllAsync_WhenCampaignIdNotNullAndCampaignHasMentors_ReturnCorrectCount()
        {
            //Arrange
            var filter = new PaginationRequest(1, 10);

            var mentor2 = new Mentor()
            {
                Id = Guid.NewGuid(),
                DisplayName = "John Doe",
                Email = "john.doe@email.com",
                Specialities = new List<Speciality>()
            };

            var campaign = new Campaign()
            {
                Id = Guid.NewGuid(),
                Name = "Test Campaign",
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(5),
                IsActive = true,
                Mentors = new List<Mentor>() { mentor, mentor2 }
            };

            var mentorList = new List<Mentor>() { mentor, mentor2 };

            await campaignsRepository.AddAsync(campaign);

            //Act
            var response = await mentorsRepository.GetAllAsync(filter, campaign.Id);

            //Assert
            Assert.Equal(mentorList.Count(), response.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenCampaignIdNotNullAndCampaignHasNoMentors_ReturnEmptyCollection()
        {
            //Arrange
            var newId = Guid.NewGuid();

            var filter = new PaginationRequest(1, 20);

            //Act
            var response = await mentorsRepository.GetAllAsync(filter, newId);

            //Assert
            Assert.Empty(response);
        }

        [Fact]
        public async Task GetCountByCampaignIdAsync_WhenCampaignHasMentors_ShouldReturnCorrectCount()
        {
            //Arrange
            var campaign = new Campaign()
            {
                Id = Guid.NewGuid(),
                Name = "Test Campaign",
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(5),
                IsActive = true,
                Mentors = new List<Mentor>() { mentor }
            };

            await campaignsRepository.AddAsync(campaign);

            //Act
            var response = await mentorsRepository.GetCountByCampaignIdAsync(campaign.Id); ;

            //Assert
            Assert.Equal(1, response);
        }

        [Fact]
        public async Task GetCountByCampaignIdAsync_WhenCampaignHasNoMentors_ShouldReturnCorrectCount()
        {
            //Arrange
            var newId = Guid.NewGuid();

            //Act
            var response = await mentorsRepository.GetCountByCampaignIdAsync(newId);

            //Assert
            Assert.Equal(0, response);
        }
    }
}
