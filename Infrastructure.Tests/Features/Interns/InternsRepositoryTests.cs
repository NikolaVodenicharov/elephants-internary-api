using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Interns.Entities;
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
        private readonly Intern internMock;
        private readonly Intern internMock2;
        private readonly PaginationRequest paginationRequestMock = new(1, 10);
        private readonly Campaign campaingMock;
        private readonly Speciality specialityMock;
        private readonly State stateMock;
        private readonly Status statusMock1;
        private readonly Status statusMock2;
        private readonly InternCampaign internCampaignWithStatesListMock;
        private readonly Intern internWithDetails;

        private readonly InternaryContext context;
        private readonly InternsRepository internsRepository;

        public InternsRepositoryTests()
        {
            DbContextOptionsBuilder<InternaryContext>? dbOptions = new DbContextOptionsBuilder<InternaryContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            context = new InternaryContext(dbOptions.Options);

            internsRepository = new InternsRepository(context);

            internMock = new Intern()
            {
                FirstName = "FirstName",
                LastName = "LastName",
                PersonalEmail = "FirstLast@gmail.com"
            };

            internMock2 = new Intern()
            {
                FirstName = "FirstNameA",
                LastName = "LastNameA",
                PersonalEmail = "FirstLastA@gmail.com"
            };

            campaingMock = new Campaign()
            {
                Name = "CampaingName",
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(50),
                IsActive = true,
            };

            specialityMock = new Speciality()
            {
                Name = "SpecialityName"
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

            stateMock = new State()
            {
                Status = statusMock1,
                Created = DateTime.UtcNow,
                Justification = "Lorem ipsum"
            };

            internCampaignWithStatesListMock = new InternCampaign()
            {
                Campaign = campaingMock,
                Speciality = specialityMock,
                States = new List<State>() { stateMock }
            };

            internWithDetails = new Intern()
            {
                FirstName = "John",
                LastName = "Doe",
                PersonalEmail = "JohnDoew@gmail.comd",
                InternCampaigns = new List<InternCampaign>() { internCampaignWithStatesListMock }
            };
        }

        [Fact]
        public async Task AddAsync_AddOneElement_ShouldBeAddedToDatabase()
        {
            //Act
            var internSummaryResponse = await internsRepository.AddAsync(internMock);

            //Assert
            Assert.Equal(internMock.FirstName, internSummaryResponse.FirstName);
            Assert.Equal(internMock.LastName, internSummaryResponse.LastName);
            Assert.Equal(internMock.PersonalEmail, internSummaryResponse.Email);
        }

        [Fact]
        public async Task AddAsync_AddMultipleElements_ShouldAddAllToDatabase()
        {
            //Act
            await internsRepository.AddAsync(internMock);
            await internsRepository.AddAsync(internMock2);

            //Assert
            Assert.Equal(2, await context.Interns.CountAsync());
        }

        [Fact]
        public async Task SaveTrackingChagesAsync_UpdateElement_ShouldSaveNewDataToDatabase()
        {
            //Arrange
            var internWithInternCampaignsEmptyListMock = new Intern()
            {
                FirstName = "FirstName",
                LastName = "LastName",
                PersonalEmail = "FirstLast@gmail.com",
                InternCampaigns = new List<InternCampaign>()
            };

            await internsRepository.AddAsync(internWithInternCampaignsEmptyListMock);

            var name = "John";

            internWithInternCampaignsEmptyListMock.FirstName = name;

            //Act
            await internsRepository.SaveTrackingChangesAsync();

            //Assert
            var internResult = await internsRepository.GetByIdAsync(internWithInternCampaignsEmptyListMock.Id);

            Assert.Equal(name, internResult.FirstName);
        }

        [Fact]
        public async Task ExistsByEmailAsync_WhenEmailExist_ShouldReturnTrue()
        {
            //Arrange
            await internsRepository.AddAsync(internMock);

            //Act
            var exist = await internsRepository.ExistsByEmailAsync(internMock.PersonalEmail);

            //Assert
            Assert.True(exist);
        }

        [Fact]
        public async Task ExistsByEmailAsync_WhenEmailDoesNotExist_ShouldReturnFalse()
        {
            //Act
            var exist = await internsRepository.ExistsByEmailAsync(internMock.PersonalEmail);

            //Assert
            Assert.False(exist);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdFound_ShouldReturnCorrectObject()
        {
            //Arrange
            var intenrSummaryRepsponse = await internsRepository.AddAsync(internMock);

            //Act
            var intern = await internsRepository.GetByIdAsync(intenrSummaryRepsponse.Id);

            //Assert
            Assert.Equal(internMock.FirstName, intern.FirstName);
            Assert.Equal(internMock.LastName, intern.LastName);
            Assert.Equal(internMock.PersonalEmail, intern.PersonalEmail);

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
            var paginationResponse = await internsRepository.GetAllAsync(paginationRequestMock);

            //Assert
            Assert.Empty(paginationResponse.Content);
            Assert.Equal(paginationRequestMock.PageNum, paginationResponse.PageNum);
            Assert.Equal(PaginationConstants.DefaultPageCount, paginationResponse.TotalPages);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectObject()
        {
            //Arrange
            var count = 21;
            var interns = new List<Intern>(count);

            for (int i = 0; i < count; i++)
            {
                interns.Add(new Intern()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    PersonalEmail = $"JohnDoe{i}@gmail.com"
                });
            }

            context.Interns.AddRange(interns);

            await context.SaveChangesAsync();

            //Act
            var paginationResponse = await internsRepository.GetAllAsync(paginationRequestMock);

            //Assert
            Assert.Equal(paginationRequestMock.PageSize, paginationResponse.Content.Count());
            Assert.Equal(paginationRequestMock.PageNum, paginationResponse.PageNum);
            Assert.Equal(3, paginationResponse.TotalPages);
        }

        [Fact]
        public async Task GetInternCampaignByIdsAsync_WhenIdFound_ShouldReturnCorrectObject()
        {
            //Arrange
            var internCampaignWithEmptyStatesListMock = new InternCampaign()
            {
                Campaign = campaingMock,
                Speciality = specialityMock,
                States = new List<State>() { stateMock }
            };

            internMock.InternCampaigns = new List<InternCampaign>() { internCampaignWithEmptyStatesListMock };

            var intenrSummaryRepsponse = await internsRepository.AddAsync(internMock);

            //Act
            var internCampaign = await internsRepository
                .GetInternCampaignByIdsAsync(
                    intenrSummaryRepsponse.Id, 
                    internCampaignWithEmptyStatesListMock.CampaignId);

            //Assert
            Assert.NotNull(internCampaign);
            Assert.NotNull(internCampaign.Intern);
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
            Assert.Equal((int)StatusEnum.Candidate, statusResponseCollection.First().Id);
            Assert.Equal(StatusEnum.Candidate.ToString(), statusResponseCollection.First().Name);
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
            await internsRepository.AddAsync(internWithDetails);

            //Act
            var internDetailsResponse = await internsRepository.GetDetailsByIdAsync(internWithDetails.Id);

            //Arrange
            Assert.Equal(internWithDetails.Id, internDetailsResponse.Id);
            Assert.Equal(internWithDetails.FirstName, internDetailsResponse.FirstName);
            Assert.Equal(internWithDetails.InternCampaigns.First().CampaignId, internDetailsResponse.InternCampaignResponses.First().Campaign.Id);
            Assert.Equal(internWithDetails.InternCampaigns.First().States.First().Status.Name, internDetailsResponse.InternCampaignResponses.First().StateResponse.Status);
        }

        [Fact]
        public async Task GetAllByCampaignIdAsync_WhenNotEmpty_ShouldReturnCorrectObjects()
        {
            //Arrange
            await internsRepository.AddAsync(internWithDetails);

            var paginationRequest = new PaginationRequest(1, 10);

            //Act
            var internsByCampaignSummaryResponse = await internsRepository.GetAllByCampaignIdAsync(paginationRequest, campaingMock.Id);

            Assert.NotEmpty(internsByCampaignSummaryResponse.Content);
        }

        [Fact]
        public async Task GetAllByCampaignIdAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            //Arrange
            var paginationRequest = new PaginationRequest(1, 10);

            //Act
            var internsByCampaignSummaryResponse = await internsRepository.GetAllByCampaignIdAsync(paginationRequest, campaingMock.Id);

            Assert.Empty(internsByCampaignSummaryResponse.Content);
        }
    }
}
