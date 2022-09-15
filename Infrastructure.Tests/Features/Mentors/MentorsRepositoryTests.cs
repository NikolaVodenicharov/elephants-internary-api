using Core.Common.Pagination;
using Core.Features.Mentors.Entities;
using Core.Features.Mentors.Interfaces;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialties.Entities;
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
        private readonly string firstName = "First";
        private readonly string lastName = "Last";
        private readonly string email = "first.last@email.com";
        private readonly InternaryContext context;
        private readonly IMentorsRepository mentorsRepository;
        private readonly Mock<IMentorsRepository> mentorsRepositoryMock;
        private readonly ISpecialitiesRepository specialitiesRepository;
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
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Specialities = specialities
            };

            mentorsRepositoryMock = new Mock<IMentorsRepository>();
        }

        [Fact]
        public async Task AddAsync_AddMentor_ShouldBeAddedToDatabase()
        {
            //Act
            var addedMentor = await mentorsRepository.AddAsync(mentor);

            var count = await mentorsRepository.GetCountAsync();

            //Assert
            Assert.Equal(1, count);
            Assert.Equal(mentor.FirstName, addedMentor.FirstName);
            Assert.Equal(mentor.LastName, addedMentor.LastName);
            Assert.Equal(mentor.Email, addedMentor.Email);
        }

        [Fact]
        public async Task SaveTrackingChangesAsync_UpdateMentor_ShouldBeUpdated()
        {
            //Arrange
            var mentorToUpdate = new Mentor()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };

            //Act
            await mentorsRepository.AddAsync(mentorToUpdate);

            mentorToUpdate.LastName = "Newman";

            await mentorsRepository.SaveTrackingChangesAsync();

            var updatedMentor = await mentorsRepository.GetByIdAsync(mentorToUpdate.Id);

            //Assert
            Assert.NotNull(updatedMentor);
            Assert.Equal(mentorToUpdate.Id, updatedMentor.Id);
            Assert.Equal(mentorToUpdate.FirstName, updatedMentor.FirstName);
            Assert.Equal(mentorToUpdate.LastName, updatedMentor.LastName);
            Assert.Equal(mentorToUpdate.Email, updatedMentor.Email);
        }

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            //Arrange
            var filter = new PaginationFilterRequest()
            {
                Skip = 0,
                Take = 10,
                Count = 8
            };

            //Act
            var mentors = await mentorsRepository.GetAllAsync(filter);

            //Assert
            Assert.Empty(mentors);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var mentor2 = new Mentor()
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com"
            };

            var filter = new PaginationFilterRequest()
            {
                Skip = 0,
                Take = 10,
                Count = 8
            };

            await specialitiesRepository.AddAsync(speciality);
            await mentorsRepository.AddAsync(mentor);
            await mentorsRepository.AddAsync(mentor2);

            //Act
            var mentors = await mentorsRepository.GetAllAsync(filter);

            //Assert
            Assert.Equal(2, mentors.Count());
            Assert.NotNull(mentors.First().Specialities);
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
            Assert.Equal(mentor.FirstName, response.FirstName);
            Assert.Equal(mentor.LastName, response.LastName);
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
        public async Task GetMentorsByCampaignIdAsync_WhenCampaignHasMentors_ReturnCorrectCount()
        {
            //Arrange
            var newId = Guid.NewGuid();
            var filter = new PaginationFilterRequest()
            {
                Skip = 0,
                Take = 5,
                Count = 10
            };

            var mentor2 = new Mentor()
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com"
            };

            var mentorList = new List<Mentor>() { mentor, mentor2 };

            mentorsRepositoryMock
                .Setup(x => x.GetMentorsByCampaignIdAsync(It.IsAny<Guid>(), It.IsAny<PaginationFilterRequest>()))
                .ReturnsAsync(mentorList);

            //Act
            var response = await mentorsRepositoryMock.Object.GetMentorsByCampaignIdAsync(newId, filter);

            //Assert
            Assert.Equal(2, response.Count());
        }

        [Fact]
        public async Task GetMentorsByCampaignIdAsync_WhenCampaignHasNoMentors_ReturnEmptyCollection()
        {
            //Arrange
            var newId = Guid.NewGuid();

            var filter = new PaginationFilterRequest()
            {
                Skip = 0,
                Take = 5,
                Count = 10
            };

            //Act
            var response = await mentorsRepository.GetMentorsByCampaignIdAsync(newId, filter);

            //Assert
            Assert.Empty(response);
        }

        [Fact]
        public async Task GetCountByCampaignIdAsync_WhenCampaignHasMentors_ShouldReturnCorrectCount()
        {
            //Arrange
            var newId = Guid.NewGuid();

            mentorsRepositoryMock
                .Setup(x => x.GetCountByCampaignIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(2);

            //Act
            var response = await mentorsRepositoryMock.Object.GetCountByCampaignIdAsync(newId); ;

            //Assert
            Assert.Equal(2, response);
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
