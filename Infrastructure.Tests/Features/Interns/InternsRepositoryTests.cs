using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Interns.Entities;
using Core.Features.Interns.RequestModels;
using Core.Features.Persons.Entities;
using Core.Features.Specialties.Entities;
using Infrastructure.Features.Interns;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Features.Interns
{
    public class InternsRepositoryTests
    {
        private readonly Guid mentorId = Guid.NewGuid();
        private readonly string firstNameMock = "John";
        private readonly string lastNameMock = "Doe";
        private readonly string emailMock = "JohnDoe@gmail.com";
        private readonly string displayName = "John Doe";
        private readonly string workEmail = "JohnDoe@endava.com";
        private readonly PaginationRequest paginationRequest = new(1, 10);
        private CreateInternRepoRequest createInternRepoRequest1 = null!;
        private CreateInternRepoRequest createInternRepoRequest2 = null!;
        private Campaign campaingMock1 = null!;
        private Campaign campaingMock2 = null!;
        private Speciality specialityMock1 = null!;
        private Speciality specialityMock2 = null!;
        private State stateMock1 = null!;
        private State stateMock2 = null!;
        private Status statusMock1 = null!;
        private Status statusMock2 = null!;
        private InternCampaign internCampaignMock1 = null!;
        private InternCampaign internCampaignMock2 = null!;
        private readonly InternaryContext context;
        private readonly InternsRepository internsRepository;

        public InternsRepositoryTests()
        {
            DbContextOptionsBuilder<InternaryContext>? dbOptions = new DbContextOptionsBuilder<InternaryContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString());

            context = new InternaryContext(dbOptions.Options);

            internsRepository = new InternsRepository(context);

            AddInternAndMentorRoles();

            AddMentorToContext();

            InitializeMockModels();
        }

        [Fact]
        public async Task CreateAsync_AddOneElement_ShouldBeAddedToDatabase()
        {
            //Act
            var internSummaryResponse = await internsRepository.CreateAsync(createInternRepoRequest1);

            //Assert
            var internCampaigns = await context
                .InternCampaigns
                .Where(ic => ic.PersonId == internSummaryResponse.Id)
                .Include(ic => ic.States)
                .FirstOrDefaultAsync();

            Assert.NotNull(internCampaigns!.States);
            Assert.NotEqual(Guid.Empty, internSummaryResponse.Id);
            Assert.Equal(createInternRepoRequest1.FirstName + " " + createInternRepoRequest1.LastName, internSummaryResponse.DisplayName);
            Assert.Equal(createInternRepoRequest1.Email, internSummaryResponse.Email);
        }

        [Fact]
        public async Task CreateAsync_AddMultipleElements_ShouldAddAllToDatabase()
        {
            //Act
            await internsRepository.CreateAsync(createInternRepoRequest1);
            await internsRepository.CreateAsync(createInternRepoRequest2);

            //Assert
            var count = await context
                .Persons
                .Where(p => p.PersonRoles.Any(pr => pr.RoleId == RoleId.Intern))
                .CountAsync();

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task UpdateAsync_WhenFound_ShouldSaveNewDataToDatabase()
        {
            //Arrange
            var createdIntern = await internsRepository.CreateAsync(createInternRepoRequest1);

            var updateInternRequest = new UpdateInternRequest(
                createdIntern.Id,
                createInternRepoRequest1.FirstName + "AAA",
                createInternRepoRequest1.LastName,
                createInternRepoRequest1.Email);

            //Act
            var updatedIntern = await internsRepository.UpdateAsync(updateInternRequest);

            //Assert
            Assert.Equal(updateInternRequest.Id, updatedIntern!.Id);
            Assert.Equal(updateInternRequest.FirstName + " " + updateInternRequest.LastName, updatedIntern.DisplayName);
            Assert.Equal(updateInternRequest.Email, updatedIntern.Email);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ShouldReturnNull()
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(
                Guid.NewGuid(),
                string.Empty,
                string.Empty,
                string.Empty);

            //Act
            var internSummaryResponse = await internsRepository.UpdateAsync(updateInternRequest);

            //Assert
            Assert.Null(internSummaryResponse);
        }

        [Fact]
        public async Task UpdateInternCampaignAsync_WhenSpecialtyIsChanged_ShouldUpdateObject()
        {
            //Arrange
            await internsRepository.CreateAsync(createInternRepoRequest1);

            await context
                .Specialties
                .AddAsync(specialityMock2);

            await context.SaveChangesAsync();

            internCampaignMock1.Speciality = specialityMock2;

            //Act
            var internCampaignResponse = await internsRepository.UpdateInternCampaignAsync(internCampaignMock1);

            //Assert
            Assert.Equal(specialityMock2.Name, internCampaignResponse.Speciality.Name);
        }

        [Fact]
        public async Task UpdateInternCampaignAsync_WhenStateIsChanged_ShouldUpdateObject()
        {
            //Arrange
            await internsRepository.CreateAsync(createInternRepoRequest1);

            internCampaignMock1.States.Add(stateMock2);

            //Act
            var internCampaignResponse = await internsRepository.UpdateInternCampaignAsync(internCampaignMock1);

            //Assert
            Assert.Equal(stateMock2.Status.Name, internCampaignResponse.StateResponse.Status);
        }

        [Fact]
        public async Task AddInternCampaignAsync_WhenPersonNotFound_ShouldReturnNull()
        {
            //Arrange
            var addInternCampaignRepoRequest = new AddInternCampaignRepoRequest(Guid.NewGuid(), new InternCampaign());

            //Act
            var internCampaignSummaryResponse = await internsRepository.AddInternCampaignAsync(addInternCampaignRepoRequest);

            //Assert
            Assert.Null(internCampaignSummaryResponse);
        }

        [Fact]
        public async Task AddInternCampaignAsync_WhenValidData_ShouldReturnCorrectObject()
        {
            //Arrange
            var internSummaryResponse = await internsRepository.CreateAsync(createInternRepoRequest1);

            var addInternCampaignRepoRequest = new AddInternCampaignRepoRequest(
                internSummaryResponse.Id,
                internCampaignMock2);

            //Act
            var internCampaignSummaryResponse = await internsRepository.AddInternCampaignAsync(addInternCampaignRepoRequest);

            //Assert
            var internCampaigns = await context
                .InternCampaigns
                .Where(ic => ic.PersonId == internSummaryResponse.Id)
                .Include(ic => ic.States)
                .ToListAsync();

            Assert.NotNull(internCampaignSummaryResponse);
            Assert.Equal(2, internCampaigns.Count);
            Assert.Equal(1, internCampaigns.First().States.Count);
        }

        [Fact]
        public async Task AddIdentity_WhenIdNotFound_ShouldReturnNull()
        {
            //Arrange
            var addInternIdentityRepoRequest = new AddInternIdentityRepoRequest(
                Guid.NewGuid(),
                workEmail,
                displayName);

            //Act
            var internCampaignSummaryResponse = await internsRepository.AddIdentityAsync(addInternIdentityRepoRequest);

            //Assert
            Assert.Null(internCampaignSummaryResponse);
        }

        [Fact]
        public async Task AddIdentity_WhenIdBelongToMentor_ShouldReturnNull()
        {
            //Arrange
            var addInternIdentityRepoRequest = new AddInternIdentityRepoRequest(
                mentorId,
                workEmail,
                displayName);

            //Act
            var internCampaignSummaryResponse = await internsRepository.AddIdentityAsync(addInternIdentityRepoRequest);

            //Assert
            Assert.Null(internCampaignSummaryResponse);
        }

        [Fact]
        public async Task AddIdentity_WhenIdFound_ShouldReturnCorrectObject()
        {
            //Arrange
            var createdIntern = await internsRepository.CreateAsync(createInternRepoRequest1);

            var addInternIdentityRepoRequest = new AddInternIdentityRepoRequest(
                createdIntern.Id,
                workEmail,
                displayName);

            //Act
            var internCampaignSummaryResponse = await internsRepository.AddIdentityAsync(addInternIdentityRepoRequest);

            //Assert
            Assert.Equal(addInternIdentityRepoRequest.WorkEmail, internCampaignSummaryResponse!.Email);
            Assert.Equal(addInternIdentityRepoRequest.DisplayName, internCampaignSummaryResponse.DisplayName);
        }

        [Fact]
        public async Task ExistsByEmailAsync_WhenEmailExist_ShouldReturnTrue()
        {
            //Arrange
            await internsRepository.CreateAsync(createInternRepoRequest1);

            //Act
            var exist = await internsRepository.ExistsByPersonalEmailAsync(createInternRepoRequest1.Email);

            //Assert
            Assert.True(exist);
        }

        [Fact]
        public async Task ExistsByEmailAsync_WhenEmailDoesNotExist_ShouldReturnFalse()
        {
            //Act
            var exist = await internsRepository.ExistsByPersonalEmailAsync(emailMock);

            //Assert
            Assert.False(exist);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdFound_ShouldReturnCorrectObject()
        {
            //Arrange
            var intenrSummaryRepsponse = await internsRepository.CreateAsync(createInternRepoRequest1);

            //Act
            var internById = await internsRepository.GetByIdAsync(intenrSummaryRepsponse.Id);

            //Assert
            Assert.Equal(createInternRepoRequest1.FirstName + " " + createInternRepoRequest1.LastName, internById!.DisplayName);
            Assert.Equal(createInternRepoRequest1.Email, internById.Email);

        }

        [Fact]
        public async Task GetByIdAsync_WhenIdNotFound_ShouldReturnNull()
        {
            //Act
            var intern = await internsRepository.GetByIdAsync(Guid.NewGuid());

            //Assert
            Assert.Null(intern);
        }

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            //Act
            var internListingResponses = await internsRepository.GetAllAsync();

            //Assert
            Assert.Empty(internListingResponses);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectObject()
        {
            //Arrange
            await internsRepository.CreateAsync(createInternRepoRequest1);
            await internsRepository.CreateAsync(createInternRepoRequest2);

            //Act
            var internListingResponses = await internsRepository.GetAllAsync();

            //Assert
            var personCount = await context.Persons.CountAsync();

            Assert.Equal(3, personCount);   // there is one mentor
            Assert.Equal(2, internListingResponses.Count());
            Assert.Single(internListingResponses.First().CampaignIds);
        }

        [Fact]
        public async Task GetPaginationAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            //Act
            var paginationResponse = await internsRepository.GetPaginationAsync(paginationRequest);

            //Assert
            Assert.Empty(paginationResponse.Content);
            Assert.Equal(paginationRequest.PageNum, paginationResponse.PageNum);
            Assert.Equal(PaginationConstants.DefaultPageCount, paginationResponse.TotalPages);
        }

        [Fact]
        public async Task GetPaginationAsync_WhenNotEmpty_ShouldReturnCorrectObject()
        {
            //Arrange
            await internsRepository.CreateAsync(createInternRepoRequest1);
            await internsRepository.CreateAsync(createInternRepoRequest2);

            //Act
            var paginationResponse = await internsRepository.GetPaginationAsync(paginationRequest);

            //Assert
            var personCount = await context.Persons.CountAsync();

            Assert.Equal(3, personCount);   // there is one mentor
            Assert.Equal(2, paginationResponse.Content.Count());
            Assert.Equal(paginationRequest.PageNum, paginationResponse.PageNum);
            Assert.Equal(1, paginationResponse.TotalPages);
        }

        [Fact]
        public async Task GetPaginationByCampaignIdAsync_WhenNotEmpty_ShouldReturnCorrectObjects()
        {
            //Arrange
            await internsRepository.CreateAsync(createInternRepoRequest1);

            //Act
            var internsByCampaignSummaryResponse = await internsRepository.GetPaginationByCampaignIdAsync(paginationRequest, campaingMock1.Id);

            Assert.Single(internsByCampaignSummaryResponse.Content);
        }

        [Fact]
        public async Task GetPaginationByCampaignIdAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            //Act
            var internsByCampaignSummaryResponse = await internsRepository.GetPaginationByCampaignIdAsync(paginationRequest, campaingMock1.Id);

            Assert.Empty(internsByCampaignSummaryResponse.Content);
        }

        [Fact]
        public async Task GetInternCampaignByIdsAsync_WhenIdFound_ShouldReturnCorrectObject()
        {
            //Arrange
            var intenrSummaryRepsponse = await internsRepository.CreateAsync(createInternRepoRequest1);

            //Act
            var internCampaign = await internsRepository.GetInternCampaignByIdsAsync(
                    intenrSummaryRepsponse.Id,
                    internCampaignMock1.CampaignId);

            //Assert
            Assert.NotNull(internCampaign);
            Assert.NotNull(internCampaign!.Person);
            Assert.NotNull(internCampaign.Campaign);
            Assert.NotNull(internCampaign.Speciality);
            Assert.NotEmpty(internCampaign.States);
        }

        [Fact]
        public async Task GetInternCampaignByIdsAsync_WhenIdNotFound_ShouldReturnNull()
        {
            //Act
            var internCampaign = await internsRepository.GetInternCampaignByIdsAsync(Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.Null(internCampaign);
        }

        [Fact]
        public async Task GetAllStatusAsync_WhenNotEmpty_ShouldReturnCorrectElements()
        {
            //Arrange
            await context
                .Status
                .AddRangeAsync(statusMock1, statusMock2);

            await context.SaveChangesAsync();

            //Act
            var statusResponseCollection = await internsRepository.GetAllStatusAsync();

            //Assert
            Assert.Equal(2, statusResponseCollection.Count());
        }

        [Fact]
        public async Task GetDetailsByIdAsync_WhenIdNotFound_ShouldReturnNul()
        {
            //Act
            var internDetailsResponse = await internsRepository.GetDetailsByIdAsync(Guid.NewGuid());

            //Assert
            Assert.Null(internDetailsResponse);
        }

        [Fact]
        public async Task GetDetailsByIdAsync_WhenNotEmpty_ShouldReturnCorrectObject()
        {
            //Arrange
            var InternSummaryResponse = await internsRepository.CreateAsync(createInternRepoRequest1);

            //Act
            var internDetailsResponse = await internsRepository.GetDetailsByIdAsync(InternSummaryResponse.Id);

            //Arrange
            Assert.Equal(InternSummaryResponse.Id, internDetailsResponse!.Id);
            Assert.Equal(createInternRepoRequest1.FirstName, internDetailsResponse.FirstName);
            Assert.Equal(createInternRepoRequest1.InternCampaign.CampaignId, internDetailsResponse.InternCampaignResponses.First().Campaign.Id);
            Assert.Equal(createInternRepoRequest1.InternCampaign.SpecialityId, internDetailsResponse.InternCampaignResponses.First().Speciality.Id);
        }

        private void AddInternAndMentorRoles()
        {
            var roleIntern = new Role() { RoleId = RoleId.Intern, Name = RoleId.Intern.ToString() };
            var roleMentor = new Role() { RoleId = RoleId.Mentor, Name = RoleId.Mentor.ToString() };

            context
                .Roles
                .Add(roleIntern);

            context
                .Roles
                .Add(roleMentor);

            context.SaveChanges();
        }

        // this mentor is required so we can check precisely for intern entities
        private void AddMentorToContext()
        {
            var mentorMock = new Person()
            {
                Id = mentorId,
                FirstName = "MentorFirstName",
                LastName = "MentorLastName",
                WorkEmail = "MentorEmail@mgial.com"
            };

            var personRole =
                new PersonRole()
                {
                    RoleId = RoleId.Mentor,
                    Person = mentorMock
                };

            context
                .PersonRoles
                .Add(personRole);

            context
                .SaveChanges();
        }

        private void InitializeMockModels()
        {
            campaingMock1 = new Campaign()
            {
                Name = "CampaingName",
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(50),
                IsActive = true,
            };

            campaingMock2 = new Campaign()
            {
                Name = "CampaingName2",
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(50),
                IsActive = true,
            };

            specialityMock1 = new Speciality()
            {
                Name = "SpecialityName"
            };

            specialityMock2 = new Speciality()
            {
                Name = "SpecialityName2"
            };

            statusMock1 = new Status()
            {
                StatusId = StatusEnum.Candidate,
                Name = StatusEnum.Candidate.ToString()
            };

            statusMock2 = new Status()
            {
                StatusId = StatusEnum.Rejected,
                Name = StatusEnum.Rejected.ToString()
            };

            stateMock1 = new State()
            {
                Status = statusMock1,
                Created = DateTime.UtcNow,
                Justification = "Lorem ipsum"
            };

            stateMock2 = new State()
            {
                Status = statusMock2,
                Created = DateTime.UtcNow.AddMinutes(10),
                Justification = "Dolor sit amet"
            };

            internCampaignMock1 = new InternCampaign()
            {
                Campaign = campaingMock1,
                Speciality = specialityMock1,
                States = new List<State>() { stateMock1 }
            };

            internCampaignMock2 = new InternCampaign()
            {
                Campaign = campaingMock2,
                Speciality = specialityMock2,
                States = new List<State>() { stateMock2 }
            };

            createInternRepoRequest1 = new CreateInternRepoRequest(
                firstNameMock,
                lastNameMock,
                emailMock,
                internCampaignMock1);

            createInternRepoRequest2 = new CreateInternRepoRequest(
                "FirstNameA",
                "LastNameA",
                "FirstLastA@gmail.com",
                internCampaignMock2);
        }
    }
}
