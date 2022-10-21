using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Persons.Entities;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialties.Entities;
using Infrastructure.Features.Campaigns;
using Infrastructure.Features.Mentors;
using Infrastructure.Features.Specialities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Features.Mentors
{
    public class MentorsRepositoryTests
    {
        private readonly string email = "first.last@email.com";
        private readonly string displayName = "Eric Evans";
        private CreateMentorRepoRequest createMentorRepoRequest = null!;
        private Campaign campaignMock = null!;
        private Speciality specialityMock = null!;
        private List<Speciality> specialitiesMock = null!;
        private readonly InternaryContext context;
        private readonly IMentorsRepository mentorsRepository;

        public MentorsRepositoryTests()
        {
            DbContextOptionsBuilder<InternaryContext>? dbOptions = new DbContextOptionsBuilder<InternaryContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString());

            context = new InternaryContext(dbOptions.Options);

            mentorsRepository = new MentorsRepository(context);

            AddInternToContext();

            InitializeMockModels();
        }

        [Fact]
        public async Task CreateAsync_AddMentor_ShouldBeAddedToDatabase()
        {
            //Act
            var mentorSummaryResponse = await mentorsRepository.CreateAsync(createMentorRepoRequest);

            //Assert
            var totalCount = await context
                .Persons
                .CountAsync();

            var count = await mentorsRepository.GetCountAsync();

            Assert.Equal(2, totalCount);
            Assert.Equal(1, count);
            Assert.Equal(createMentorRepoRequest.DisplayName, mentorSummaryResponse.DisplayName);
            Assert.Equal(createMentorRepoRequest.WorkEmail, mentorSummaryResponse.WorkEmail);
        }

        [Fact]
        public async Task AddToCampaignAsync_WhenIdNotFound_ShouldReturnFalse()
        {
            //Arrange
            var addMentorToCampaignRequest = new AddMentorToCampaignRepoRequest(Guid.NewGuid(), campaignMock);

            //Act
            var isAdded = await mentorsRepository.AddToCampaignAsync(addMentorToCampaignRequest);

            //Assert
            Assert.False(isAdded);
        }

        [Fact]
        public async Task AddToCampaignAsync_WhenIdFound_ShouldReturnSaveChanges()
        {
            //Arrange
            var mentorSummaryResponse = await mentorsRepository.CreateAsync(createMentorRepoRequest);

            await context
                .Campaigns
                .AddAsync(campaignMock);

            var addMentorToCampaignRequest = new AddMentorToCampaignRepoRequest(mentorSummaryResponse.Id, campaignMock);

            //Act
            var isAdded = await mentorsRepository.AddToCampaignAsync(addMentorToCampaignRequest);

            //Assert
            Assert.True(isAdded);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdExists_ShouldReturnCorrectObject()
        {
            //Arrange

            var createdMentorSummaryResponse = await mentorsRepository.CreateAsync(createMentorRepoRequest);

            //Act
            var mentorSummaryResponse = await mentorsRepository.GetByIdAsync(createdMentorSummaryResponse.Id);

            //Assert
            Assert.Equal(createdMentorSummaryResponse.Id, mentorSummaryResponse!.Id);
            Assert.Equal(createMentorRepoRequest.DisplayName, mentorSummaryResponse.DisplayName);
            Assert.Equal(createMentorRepoRequest.WorkEmail, mentorSummaryResponse.WorkEmail);
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
        public async Task GetDetailsByIdAsync_WhenIdExists_ShouldReturnCorrectObject()
        {
            //Arrange
            var createdMentorSummaryResponse = await mentorsRepository.CreateAsync(createMentorRepoRequest);

            //Act
            var mentorDetailsResponse = await mentorsRepository.GetByIdAsync(createdMentorSummaryResponse.Id);

            //Assert
            Assert.Equal(createdMentorSummaryResponse.Id, mentorDetailsResponse!.Id);
            Assert.Equal(createMentorRepoRequest.DisplayName, mentorDetailsResponse.DisplayName);
            Assert.Equal(createMentorRepoRequest.WorkEmail, mentorDetailsResponse.WorkEmail);
            Assert.Equal(createMentorRepoRequest.Specialities.First().Name, mentorDetailsResponse.Specialities.First().Name);
        }

        [Fact]
        public async Task GetDetailsByIdAsync_WhenIdNotFound_ShouldReturnNull()
        {
            //Act
            var mentorDetailsResponse = await mentorsRepository.GetByIdAsync(Guid.NewGuid());

            //Assert
            Assert.Null(mentorDetailsResponse);
        }

        [Fact]
        public async Task UpdateAsync_WhenFound_ShouldSaveNewDataToDatabase()
        {
            //Arrange
            var mentorSummaryResponse = await mentorsRepository.CreateAsync(createMentorRepoRequest);

            var specialtyMock2 = new Speciality() { Name = "NewSpecialty" };
            var specialtiesColelction = new List<Speciality>() { specialtyMock2 };

            var updateMentorRepoRequest = new UpdateMentorRepoRequest(
                mentorSummaryResponse.Id,
                specialtiesColelction);

            //Act
            var mentorDetailsResponse = await mentorsRepository.UpdateAsync(updateMentorRepoRequest);

            //Assert
            Assert.Equal(updateMentorRepoRequest.Id, mentorDetailsResponse!.Id);
            Assert.Equal(1, mentorDetailsResponse.Specialities.Count);
            Assert.Equal(specialtyMock2.Name, mentorDetailsResponse.Specialities.First().Name);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ShouldReturnNull()
        {
            //Arrange
            var updateMentorRepoRequest = new UpdateMentorRepoRequest(
                Guid.NewGuid(),
                new List<Speciality>());

            //Act
            var mentorDetailsResponse = await mentorsRepository.UpdateAsync(updateMentorRepoRequest);

            //Assert
            Assert.Null(mentorDetailsResponse);
        }

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            //Arrange
            var filter = new PaginationRequest(1, 10);

            //Act
            var mentorsCount = await mentorsRepository.GetAllAsync(filter);

            //Assert
            Assert.Empty(mentorsCount);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectCountElements()
        {
            //Arrange
            await mentorsRepository.CreateAsync(createMentorRepoRequest);

            var createMentroRepoRequest2 = new CreateMentorRepoRequest(
                "Some name",
                "SomeEmail@gmail.com",
                new List<Speciality>() { specialityMock });

            await mentorsRepository.CreateAsync(createMentroRepoRequest2);

            var filter = new PaginationRequest(1, 3);

            //Act
            var response = await mentorsRepository.GetAllAsync(filter);

            //Assert
            Assert.Equal(2, response.Count());
        }

        [Fact]
        public async Task GetCountAsync_WhenNotEmpty_ShouldReturnCorrectCount()
        {
            //Arrange
            await mentorsRepository.CreateAsync(createMentorRepoRequest);

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
            await mentorsRepository.CreateAsync(createMentorRepoRequest);

            //Act
            var isEmailUsed = await mentorsRepository.IsEmailUsed(createMentorRepoRequest.WorkEmail);

            //Assert
            Assert.True(isEmailUsed);
        }

        [Fact]
        public async Task IsEmailUsed_WhenNotInUse_ShouldReturnFalse()
        {
            //Arrange
            await mentorsRepository.CreateAsync(createMentorRepoRequest);

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

            var mentorMock = new Person()
            {
                DisplayName = "Mentor DisplayName",
                WorkEmail = "MentorEmail@mgial.com",
                Campaigns = new List<Campaign>() { campaignMock }
            };

            var mentorMock2 = new Person()
            {
                DisplayName = "Mentor DisplayName",
                WorkEmail = "john.doe@email.com",
                Campaigns = new List<Campaign>() { campaignMock }
            };

            var personRole =
                new PersonRole()
                {
                    RoleId = RoleId.Mentor,
                    Person = mentorMock,
                };

            var personRole2 =
                new PersonRole()
                {
                    RoleId = RoleId.Mentor,
                    Person = mentorMock2,
                };

            await context
                .PersonRoles
                .AddRangeAsync(personRole, personRole2);

            await context.SaveChangesAsync();

            //Act
            var mentors = await mentorsRepository.GetAllAsync(filter, campaignMock.Id);

            //Assert
            Assert.Equal(2, mentors.Count());
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
            var mentorMock = new Person()
            {
                DisplayName = "Mentor DisplayName",
                WorkEmail = "MentorEmail@mgial.com",
                Campaigns = new List<Campaign>() { campaignMock }
            };

            var personRole =
                new PersonRole()
                {
                    RoleId = RoleId.Mentor,
                    Person = mentorMock,
                };

            await context
                .PersonRoles
                .AddAsync(personRole);

            await context.SaveChangesAsync();

            //Act
            var mentorsCount = await mentorsRepository.GetCountByCampaignIdAsync(campaignMock.Id);

            //Assert
            Assert.Equal(1, mentorsCount);
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

        [Fact]
        public async Task RemoveFromCampaignAsync_WhenMentorNotFound_ShouldReturnFalse()
        {
            //Arrange
            var campaign = new Campaign()
            {
                Id = Guid.NewGuid(),
                Name = "Campaign 1",
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(30),
                IsActive = true
            };

            //Act 
            var isRemoved = await mentorsRepository.RemoveFromCampaignAsync(Guid.NewGuid(), campaign);

            //Assert
            Assert.False(isRemoved);
        }

        [Fact]
        public async Task RemoveFromCampaignAsync_WhenMentorFound_ShouldReturnTrue()
        {
            //Arrange
            var campaign = new Campaign()
            {
                Id = Guid.NewGuid(),
                Name = "Campaign 1",
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(30),
                IsActive = true
            };

            await context.Campaigns.AddAsync(campaign);

            var createdMentor = await mentorsRepository.CreateAsync(createMentorRepoRequest);

            var addMentorToCampaignRepoRequest = new AddMentorToCampaignRepoRequest(createdMentor.Id, campaign);

            await mentorsRepository.AddToCampaignAsync(addMentorToCampaignRepoRequest);

            //Act
            var isRemoved = await mentorsRepository.RemoveFromCampaignAsync(createdMentor.Id, campaign);

            //Assert
            Assert.True(isRemoved);
        }

        [Fact]
        public async Task IsMentor_WhenPersonIsMentor_ShouldReturnTrue()
        {
            // Arrange
            var mentorSummaryResponse = await mentorsRepository.CreateAsync(createMentorRepoRequest);

            // Act
            var response = await mentorsRepository.IsMentorByIdAsync(mentorSummaryResponse.Id);

            // Assert
            Assert.True(response);
        }

        [Fact]
        public async Task IsMentor_WhenPersonIsNotMentor_ShouldReturnFalse()
        {
            // Act
            var response = await mentorsRepository.IsMentorByIdAsync(Guid.NewGuid());

            // Assert
            Assert.False(response);
        }
        
        [Fact]
        public async Task AddMentorRoleByIdAsync_WhenDataIsCorrect_ShouldReturnCorrectObject()
        {
            // Arrange
            var person = new Person()
            {
                DisplayName = "Jane Doe",
                WorkEmail = "Jane.Doe@test.com",
            };

            var personRole = new PersonRole()
            {
                RoleId = RoleId.Administrator,
                Person = person
            };

            await context.PersonRoles.AddAsync(personRole);

            await context.Specialties.AddRangeAsync(specialitiesMock);

            await context.SaveChangesAsync();

            var admin = await context.Persons.FirstOrDefaultAsync(a => a.WorkEmail == person.WorkEmail);

            var addRoleMentorRequest = new AddMentorRoleRepoRequest(admin!.Id, specialitiesMock);

            // Act
            var mentorSummaryResponse = await mentorsRepository.AddMentorRoleByIdAsync(addRoleMentorRequest);

            // Assert
            Assert.NotNull(mentorSummaryResponse);
            Assert.Equal(admin.Id, mentorSummaryResponse?.Id);
            Assert.Equal(specialitiesMock.Count, mentorSummaryResponse?.Specialities.Count());
        }
        
        [Fact]
        public async Task AddMentorRoleByIdAsync_WhenPersonNotFound_ShouldReturnNull()
        {
            // Arrange
            var addMentorRequest = new AddMentorRoleRepoRequest(Guid.NewGuid(), specialitiesMock);
            
            // Act
            var mentor = await mentorsRepository.AddMentorRoleByIdAsync(addMentorRequest);

            // Assert
            Assert.Null(mentor);
        }

        // this intern is required so we can check precisely for mentors entities in all persons
        private void AddInternToContext()
        {
            var intern = new Person()
            {
                FirstName = "InternFirstName",
                LastName = "InternLastName",
                PersonalEmail = "InternEmail@mgial.com",
            };

            var personRole =
                new PersonRole()
                {
                    RoleId = RoleId.Intern,
                    Person = intern,
                };

            context
                .PersonRoles
                .Add(personRole);

            context
                .SaveChanges();
        }

        private void InitializeMockModels()
        {
            specialityMock = new Speciality()
            {
                Id = Guid.NewGuid(),
                Name = "Testing"
            };

            specialitiesMock = new List<Speciality>()
            {
                specialityMock
            };

            createMentorRepoRequest = new CreateMentorRepoRequest(
                displayName,
                email,
                specialitiesMock);

            campaignMock = new Campaign()
            {
                Id = Guid.NewGuid(),
                Name = "Test Campaign",
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(5),
                IsActive = true
            };
        }
    }
}
