using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.ResponseModels;
using Core.Features.Campaigns.Support;
using Core.Features.Mentors;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Mentors.Support;
using Core.Features.Persons.Interfaces;
using Core.Features.Persons.ResponseModels;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialities.Support;
using Core.Features.Specialties.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Features.Mentors
{
    public class MentorsServiceTests
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly string displayName = "Display Name";
        private readonly string email = "iliyan.dimitrov@endava.com";
        private readonly string ApplicationUrl = "ApplicationUrl";
        private readonly Guid specialityId = Guid.NewGuid();
        private readonly string specialityName = "Backend";
        private readonly Mock<IMentorsRepository> mentorsRepositoryMock;
        private readonly Mock<ICampaignsRepository> campaignsRepositoryMock;
        private readonly Mock<ISpecialitiesRepository> specialitiesRepositoryMock;
        private readonly Mock<IIdentityRepository> identityRepositoryMock;
        private readonly MentorsService mentorsServiceMock;
        private CreateMentorRequest createMentorRequest = null!;
        private UpdateMentorRequest updateMentorRequest = null!;
        private MentorSummaryResponse mentorSummaryResponse = null!;
        private MentorPaginationResponse mentorPaginationResponse = null!;
        private MentorDetailsResponse mentorDetailsResponse = null!;
        private IEnumerable<MentorPaginationResponse> mentorPaginationResponseList = null!;
        private IdentitySummaryResponse identitySummaryResponse = null!;
        private Campaign campaignMock = null!;
        private Speciality specialityMock = null!;
        private List<Speciality> specialitiesMock = null!;
        private List<Guid> specialityIds = null!;

        public static IEnumerable<object[]> invalidEmails = new List<object[]>
        {
            new object[] { "invalid@example.c" },
            new object[] { "invalid@example..com" },
            new object[] { "invalid@example.com." },
            new object[] { "invalidexample" },
            new object[] { "invalidexample.com" },
            new object[] { "invalidexample.commmm" },
            new object[] { "invalidexample.co.uk." },
            new object[] { "invalidexample.co_uk" },
            new object[] { "invalidexample.co_ukkkk" },
        };

        public MentorsServiceTests()
        {
            var createMentorRequestValidator = new CreateMentorRequestValidator();
            var updateMentorRequestValidator = new UpdateMentorRequestValidator();
            var paginationRequestValidator = new PaginationRequestValidator();

            mentorsRepositoryMock = new Mock<IMentorsRepository>();
            campaignsRepositoryMock = new Mock<ICampaignsRepository>();
            specialitiesRepositoryMock = new Mock<ISpecialitiesRepository>();
            identityRepositoryMock = new Mock<IIdentityRepository>();

            var mockLogger = new Mock<ILogger<MentorsService>>();

            mentorsServiceMock = new MentorsService(
                mentorsRepositoryMock.Object,
                campaignsRepositoryMock.Object,
                specialitiesRepositoryMock.Object,
                identityRepositoryMock.Object,
                mockLogger.Object,
                createMentorRequestValidator,
                updateMentorRequestValidator,
                paginationRequestValidator);

            InitializeMockModels();
        }

        #region CreateAsyncTests

        [Theory]
        [MemberData(nameof(invalidEmails))]
        public async Task CreateAsync_WhenEmailIsIncorrectFormat_ShouldThrowException(string invalidEmail)
        {
            //Arrange
            var invalidCreateMentorRequest = new CreateMentorRequest(
                invalidEmail,
                specialityIds,
                ApplicationUrl);

            //Act
            var action = async () => await mentorsServiceMock.CreateAsync(invalidCreateMentorRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailAlreadyUsed_ShouldThrowException()
        {
            //Arrange
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialitiesMock);

            mentorsRepositoryMock
                .Setup(x => x.IsEmailUsed(It.IsAny<string>()))
                .ReturnsAsync(true);

            //Act
            var action = async () => await mentorsServiceMock.CreateAsync(createMentorRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenValidData_ShouldReturnCorrectData()
        {
            //Arrange
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialitiesMock);

            mentorsRepositoryMock
                .Setup(x => x.IsEmailUsed(It.IsAny<string>()))
                .ReturnsAsync(false);

            mentorsRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateMentorRepoRequest>()))
                .ReturnsAsync(mentorSummaryResponse);

            identityRepositoryMock
                .Setup(i => i.SendUserInviteAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(identitySummaryResponse);

            //Act
            var response = await mentorsServiceMock.CreateAsync(createMentorRequest);

            //Assert
            Assert.Equal(mentorSummaryResponse.DisplayName, response.DisplayName);
            Assert.Equal(mentorSummaryResponse.WorkEmail, response.WorkEmail);
        }

        [Fact]
        public async Task CreateAsync_WhenNotAllSpecialtiesFound_ShouldThrowException()
        {
            //Arrange
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(new List<Speciality>());

            mentorsRepositoryMock
                .Setup(x => x.IsEmailUsed(It.IsAny<string>()))
                .ReturnsAsync(false);

            mentorsRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateMentorRepoRequest>()))
                .ReturnsAsync(mentorSummaryResponse);


            //Act
            var action = async () => await mentorsServiceMock.CreateAsync(createMentorRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        #endregion

        #region UpdateAsyncTests

        [Fact]
        public async Task UpdateAsync_WhenDataIsCorrect_ShouldUpdate()
        {
            //Arrange
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialitiesMock);

            mentorsRepositoryMock
                .Setup(m => m.UpdateAsync(It.IsAny<UpdateMentorRepoRequest>()))
                .ReturnsAsync(mentorDetailsResponse);

            //Act
            var response = await mentorsServiceMock.UpdateAsync(updateMentorRequest);

            //Assert
            Assert.Equal(mentorDetailsResponse, response);
        }

        [Fact]
        public async Task UpdateAsync_WhenMentorNotFound_ShouldThrowException()
        {
            //Arrange
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialitiesMock);

            //Act
            var action = async () => await mentorsServiceMock.UpdateAsync(updateMentorRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotAllSpecialitiesFound_ShouldThrowException()
        {
            //Arrange
            var request = new UpdateMentorRequest(id, new List<Guid>() { specialityId, Guid.NewGuid() });

            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialitiesMock);

            //Act
            var action = async () => await mentorsServiceMock.UpdateAsync(request);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        #endregion

        #region GetByIdAsyncTests

        [Fact]
        public async Task GetByIdAsync_WhenIdExists_ShouldReturnCorrectData()
        {
            //Arrange
            mentorsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mentorDetailsResponse);

            //Act
            var response = await mentorsServiceMock.GetByIdAsync(id);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(id, response.Id);
            Assert.Equal(mentorDetailsResponse.DisplayName, response.DisplayName);
            Assert.Equal(mentorDetailsResponse.WorkEmail, response.WorkEmail);
            Assert.Equal(mentorDetailsResponse.Id, response.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdNotFound_ShouldThrowException()
        {
            //Arrange
            var newId = Guid.NewGuid();

            //Act
            var action = async () => await mentorsServiceMock.GetByIdAsync(newId);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        #endregion

        #region GetPaginationAsyncTests

        [Theory]
        [InlineData(1, 5)]
        [InlineData(2, 10)]
        [InlineData(1, 100)]
        public async Task GetPaginationAsync_WhenFilterIsCorrectAndCampaignIdIsNull_ShouldGetData(int pageNum, int pageSize)
        {
            //Arrange
            var mentorList = new List<MentorPaginationResponse>()
            {
                mentorPaginationResponse
            };

            var filter = new PaginationRequest(pageNum, pageSize);

            mentorsRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(15);

            mentorsRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>(), null))
                .ReturnsAsync(mentorList);

            //Act
            var response = await mentorsServiceMock.GetPaginationAsync(filter);

            //Assert
            Assert.Equal(mentorList.Count(), response.Content.Count());
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(0, 1)]
        [InlineData(1, -1)]
        [InlineData(1, 0)]
        public async Task GetPaginationAsync_WhenPageValuesAreInvalid_ShouldThrowException(int pageNum, int pageSize)
        {
            //Arrange
            var filter = new PaginationRequest(pageNum, pageSize);

            //Act
            var action = async () => await mentorsServiceMock.GetPaginationAsync(filter);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetPaginationAsync_WhenCampaignIdIsSetAndCampaignHasMentors_ShouldReturnCorrectCount()
        {
            //Arrange
            var newId = Guid.NewGuid();

            var expectedPaginationResponse = new PaginationResponse<MentorPaginationResponse>(
                mentorPaginationResponseList, 1, 4);

            var filter = new PaginationRequest(1, 1);

            mentorsRepositoryMock
                .Setup(x => x.GetCountByCampaignIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mentorPaginationResponseList.Count);

            mentorsRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>(), It.IsAny<Guid>()))
                .ReturnsAsync(mentorPaginationResponseList);

            //Act
            var actualPaginationResponse = await mentorsServiceMock.GetPaginationAsync(filter, newId);

            //Assert
            Assert.Equal(expectedPaginationResponse.Content.Count(), actualPaginationResponse.Content.Count());
        }

        [Fact]
        public async Task GetPaginationAsync_WhenPageNumIsBiggerThanTotalPages_ShouldThrowException()
        {
            //Arrange
            var paginationReguquestInvalidPageNum = new PaginationRequest(10, 10);

            mentorsRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>(), null))
                .ReturnsAsync(mentorPaginationResponseList);

            mentorsRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(mentorPaginationResponseList.Count);

            //Act
            var action = async () => await mentorsServiceMock.GetPaginationAsync(paginationReguquestInvalidPageNum);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        #endregion

        #region GetAllAsyncTests

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            //Act
            var mentors = await mentorsServiceMock.GetAllAsync();

            //Assert
            Assert.Empty(mentors);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var mentorList = new List<MentorPaginationResponse>() { mentorPaginationResponse };

            mentorsRepositoryMock
                .Setup(x => x.GetAllAsync(null, null))
                .ReturnsAsync(mentorList);

            //Act
            var response = await mentorsServiceMock.GetAllAsync();

            //Assert
            Assert.Single(response);
        }

        #endregion

        #region GetCountAsyncTests

        [Fact]
        public async Task GetCountAsync_WhenMentorsFound_ShouldReturnCorrectCount()
        {
            //Arrange
            mentorsRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(1);

            //Act
            var response = await mentorsServiceMock.GetCountAsync();

            //Assert
            Assert.Equal(1, response);
        }

        [Fact]
        public async Task GetCountAsync_WhenNoMentorsFound_ShouldReturnZero()
        {
            //Act
            var response = await mentorsServiceMock.GetCountAsync();

            //Assert
            Assert.Equal(0, response);
        }

        #endregion

        #region AddToCampaignAsyncTests

        [Fact]
        public async Task AddToCampaignAsync_WhenCampaignNotFound_ShouldThrowError()
        {
            //Arrange
            var addMentorToCampaignRequest = new AddMentorToCampaignRequest(Guid.NewGuid(), Guid.NewGuid());

            //Act
            var action = async () => await mentorsServiceMock.AddToCampaignAsync(addMentorToCampaignRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task AddToCampaignAsync_WhenMentorNotFound_ShouldThrowError()
        {
            //Arrange
            var addMentorToCampaignRequest = new AddMentorToCampaignRequest(Guid.NewGuid(), Guid.NewGuid());

            var campaign = new Campaign()
            {
                Id = addMentorToCampaignRequest.CampaignId,
                Name = "Test"
            };

            campaignsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaign);

            //Act
            var action = async () => await mentorsServiceMock.AddToCampaignAsync(addMentorToCampaignRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task AddToCampaignAsync_WhenMentorAlreadyAssignedToCampaign_ShouldThrowError()
        {
            //Arrange
            var addMentorToCampaignRequest = new AddMentorToCampaignRequest(Guid.NewGuid(), Guid.NewGuid());

            campaignsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaignMock);

            mentorsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mentorDetailsResponse);

            //Act
            var action = async () => await mentorsServiceMock.AddToCampaignAsync(addMentorToCampaignRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task AddToCampaignAsync_WhenMentorNotAssignedToCampaign_ShouldAddMentorToCampaign()
        {
            //Arrange
            var addMentorToCampaignRequest = new AddMentorToCampaignRequest(Guid.NewGuid(), Guid.NewGuid());

            var campaign = new Campaign()
            {
                Id = addMentorToCampaignRequest.CampaignId,
                Name = "Test"
            };

            campaignsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaign);

            mentorsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mentorDetailsResponse);

            AddMentorToCampaignRepoRequest addMentorToCampaignRepoRequest = null!;

            mentorsRepositoryMock
                .Setup(m => m.AddToCampaignAsync(It.IsAny<AddMentorToCampaignRepoRequest>()))
                .Callback((AddMentorToCampaignRepoRequest request) => addMentorToCampaignRepoRequest = request)
                .ReturnsAsync(true);

            //Act
            var isAdded = await mentorsServiceMock.AddToCampaignAsync(addMentorToCampaignRequest);

            //Arrange
            Assert.True(isAdded);
            Assert.Equal(addMentorToCampaignRequest.CampaignId, addMentorToCampaignRepoRequest.Campaign.Id);
            Assert.Equal(addMentorToCampaignRequest.MentorId, addMentorToCampaignRepoRequest.MentorId);
        }

        #endregion

        private void InitializeMockModels()
        {
            specialityMock = new Speciality()
            {
                Id = specialityId,
                Name = specialityName
            };

            campaignMock = new Campaign()
            {
                Id = Guid.NewGuid(),
                Name = "Campaign name",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(10),
                IsActive = true
            };

            specialitiesMock = new List<Speciality>()
            {
                specialityMock
            };

            specialityIds = new List<Guid>() { specialityMock.Id };

            createMentorRequest = new CreateMentorRequest(
                email, 
                specialityIds, 
                ApplicationUrl);

            updateMentorRequest = new UpdateMentorRequest(id, specialityIds);

            mentorSummaryResponse = new MentorSummaryResponse(
                id, 
                displayName, 
                email,
                new List<SpecialitySummaryResponse>());

            mentorPaginationResponse = new MentorPaginationResponse(
                id,
                displayName,
                email,
                new List<SpecialitySummaryResponse>() { specialityMock.ToSpecialitySummaryResponse() },
                new List<CampaignSummaryResponse>() { campaignMock.ToCampaignSummary() });

            mentorDetailsResponse = new MentorDetailsResponse(
                id,
                displayName,
                email,
                new List<CampaignSummaryResponse>() { campaignMock.ToCampaignSummary()},
                new List<SpecialitySummaryResponse>() { specialityMock.ToSpecialitySummaryResponse() });

            identitySummaryResponse = new IdentitySummaryResponse(
                email,  
                displayName);

            mentorPaginationResponseList = new List<MentorPaginationResponse>() { mentorPaginationResponse };
        }
    }
}
