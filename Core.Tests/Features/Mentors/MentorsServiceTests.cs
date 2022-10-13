using Core.Common;
using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Mentors;
using Core.Features.Mentors.Entities;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Mentors.Support;
using Core.Features.Specialities.Interfaces;
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
        private readonly string mentorDisplayName = "Iliyan Dimitrov";
        private readonly string mentorEmail = "iliyan.dimitrov@endava.com";
        private readonly Guid specialityId = Guid.NewGuid();
        private readonly string specialityName = "Backend";
        private Mock<IMentorsRepository> mentorsRepositoryMock;
        private Mock<ICampaignsRepository> campaignsRepositoryMock;
        private Mock<ISpecialitiesRepository> specialitiesRepositoryMock;
        private MentorsService mentorsServiceMock;
        private Mentor returnMentor;
        private Speciality speciality;
        private List<Speciality> specialities;
        private List<Guid> specialityIds;
        private List<Campaign> campaigns;

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

            var mockLogger = new Mock<ILogger<MentorsService>>();

            mentorsServiceMock = new MentorsService(
                mentorsRepositoryMock.Object, campaignsRepositoryMock.Object, specialitiesRepositoryMock.Object, 
                mockLogger.Object, createMentorRequestValidator, updateMentorRequestValidator, paginationRequestValidator);

            speciality = new Speciality()
            {
                Id = specialityId,
                Name = specialityName
            };

            specialities = new List<Speciality>()
            {
                speciality
            };

            specialityIds = new List<Guid>() { speciality.Id };

            var campaign = new Campaign()
            {
                Id = Guid.NewGuid(),
                Name = "Test Campaign",
                StartDate =  DateTime.Today.AddDays(5),
                EndDate =  DateTime.Today.AddDays(35),
                IsActive = false
            };

            campaigns = new List<Campaign>() { campaign };

            returnMentor = new Mentor()
            {
                Id = id,
                DisplayName = mentorDisplayName,
                Email = mentorEmail,
                Specialities = specialities,
                Campaigns = campaigns
            };
        }

        #region CreateAsyncTests

        [Fact]
        public async Task CreateAsync_CallCorrectRepositoryMethod()
        {
            //Arrange
            var request = CreateValidCreateMentorRequest();

            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);

            mentorsRepositoryMock
                .Setup(x => x.IsEmailUsed(It.IsAny<string>()))
                .ReturnsAsync(false);

            mentorsRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Mentor>()))
                .ReturnsAsync(returnMentor);

            //Act
            await mentorsServiceMock.CreateAsync(request);

            //Assert
            mentorsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Mentor>()), Times.Once());
        }

        [Fact]
        public async Task CreateAsync_WhenDataIsCorrect_ShouldReturnCorrectData()
        {
            //Arrange
            var request = CreateValidCreateMentorRequest();

            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);

            mentorsRepositoryMock
                .Setup(x => x.IsEmailUsed(It.IsAny<string>()))
                .ReturnsAsync(false);

            mentorsRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Mentor>()))
                .ReturnsAsync(returnMentor);

            //Act
            var response = await mentorsServiceMock.CreateAsync(request);

            //Assert
            Assert.NotEqual(Guid.Empty, response.Id);
            Assert.Equal(mentorDisplayName, response.DisplayName);
            Assert.Equal(mentorEmail, response.Email);
            Assert.NotNull(response.Specialities);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailIsDuplicated_ShouldThrowException()
        {
            //Arrange
            var request = CreateValidCreateMentorRequest();

            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);

            mentorsRepositoryMock
                .Setup(x => x.IsEmailUsed(It.IsAny<string>()))
                .ReturnsAsync(true);

            //Act
            var action = async () => await mentorsServiceMock.CreateAsync(request);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenDisplayNameIsEmpty_ShouldThrowException()
        {
            //Arrange
            var request = new CreateMentorRequest(string.Empty, mentorEmail, specialityIds);

            //Act
            var action = async () => await mentorsServiceMock.CreateAsync(request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(invalidEmails))]
        public async Task CreateAsync_WhenEmailIsIncorrectFormat_ShouldThrowException(string invalidEmail)
        {
            //Arrange
            var request = new CreateMentorRequest(mentorDisplayName, invalidEmail, specialityIds);

            //Act
            var action = async () => await mentorsServiceMock.CreateAsync(request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailAlreadyUsed_ShouldThrowException()
        {
            //Arrange
            var request = CreateValidCreateMentorRequest();

            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);

            mentorsRepositoryMock
                .Setup(x => x.IsEmailUsed(It.IsAny<string>()))
                .ReturnsAsync(true);

            //Act
            var action = async () => await mentorsServiceMock.CreateAsync(request);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailNotUsed_ShouldReturnCorrectData()
        {
            //Arrange
            var request = CreateValidCreateMentorRequest();

            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);

            mentorsRepositoryMock
                .Setup(x => x.IsEmailUsed(It.IsAny<string>()))
                .ReturnsAsync(false);

            mentorsRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Mentor>()))
                .ReturnsAsync(returnMentor);

            //Act
            var response = await mentorsServiceMock.CreateAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.NotEqual(Guid.Empty, response.Id);
            Assert.Equal(request.DisplayName, response.DisplayName);
            Assert.Equal(request.Email, response.Email);
            Assert.NotEmpty(response.Specialities);
            Assert.Equal(request.SpecialityIds.Count(), response.Specialities.Count);
        }

        [Fact]
        public async Task CreateAsync_WhenNotAllSpecialitiesFound_ShouldThrowException()
        {
            //Arrange
            var request = new CreateMentorRequest(mentorDisplayName, mentorEmail, new List<Guid>() { specialityId, Guid.NewGuid() });

            mentorsRepositoryMock
                .Setup(x => x.IsEmailUsed(It.IsAny<string>()))
                .ReturnsAsync(false);

            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);

            //Act
            var action = async () => await mentorsServiceMock.CreateAsync(request);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);

        }

        #endregion

        #region UpdateAsyncTests

        [Fact]
        public async Task UpdateAsync_WhenDataIsCorrect_ShouldUpdate()
        {
            //Arrange
            var request = CreateUpdateMentorRequest();

            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);

            mentorsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(returnMentor);

            mentorsRepositoryMock
                .Setup(x => x.IsEmailUsed(It.IsAny<string>()))
                .ReturnsAsync(false);

            //Act
            var response = await mentorsServiceMock.UpdateAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(request.Id, response.Id);
            Assert.NotNull(response.Specialities);
            Assert.Equal(request.SpecialityIds.Count(), response.Specialities.Count);
        }

        [Fact]
        public async Task UpdateAsync_WhenMentorNotFound_ShouldThrowException()
        {
            //Arrange
            var request = CreateUpdateMentorRequest();

            //Act
            var action = async () => await mentorsServiceMock.UpdateAsync(request);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenMentorExists_ShouldUpdate()
        {
            //Arrange
            var request = CreateUpdateMentorRequest();

            mentorsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(returnMentor);

            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);

            mentorsRepositoryMock
                .Setup(x => x.IsEmailUsed(It.IsAny<string>()))
                .ReturnsAsync(false);

            //Act
            var response = await mentorsServiceMock.UpdateAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(request.Id, response.Id);
            Assert.NotNull(response.Specialities);
            Assert.Equal(request.SpecialityIds.Count(), response.Specialities.Count);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotAllSpecialitiesFound_ShouldThrowException()
        {
            //Arrange
            var request = new UpdateMentorRequest(id, new List<Guid>() { specialityId, Guid.NewGuid() });

            mentorsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(returnMentor);

            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);

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
                .ReturnsAsync(returnMentor);

            //Act
            var response = await mentorsServiceMock.GetByIdAsync(id);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(id, response.Id);
            Assert.Equal(mentorDisplayName, response.DisplayName);
            Assert.Equal(mentorEmail, response.Email);
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
            var mentorList = new List<Mentor>()
            {
                returnMentor
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
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GetPaginationAsync_WhenPageNumIsLessThanOne_ShouldThrowException(int pageNum)
        {
            //Arrange
            var filter = new PaginationRequest(pageNum, 4);

            //Act
            var action = async () => await mentorsServiceMock.GetPaginationAsync(filter);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetPaginationAsync_WhenPageSizeIsLessThanOne_ShouldThrowException(int pageSize)
        {
            //Arrange
            var filter = new PaginationRequest(1, pageSize);

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

            var mentorList = new List<Mentor>() { returnMentor };

            var expectedPaginationResponse = new PaginationResponse<MentorDetailsResponse>(
                mentorList.ToMentorDetailsResponses(), 1, 4);

            var filter = new PaginationRequest(1, 1);

            mentorsRepositoryMock
                .Setup(x => x.GetCountByCampaignIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mentorList.Count);

            mentorsRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>(), It.IsAny<Guid>()))
                .ReturnsAsync(mentorList);

            //Act
            var actualPaginationResponse = await mentorsServiceMock.GetPaginationAsync(filter, newId);

            //Assert
            Assert.Equal(expectedPaginationResponse.Content.Count(), actualPaginationResponse.Content.Count());
        }

        [Fact]
        public async Task GetPaginationAsync_WhenPageNumIsBiggerThanTotalPages_ShouldThrowException()
        {
            //Arrange
            var pageNum = 10;
            var pageSize = 10;
            var count = 1;

            var mentorList = new List<Mentor>()
            {
                returnMentor
            };

            var filter = new PaginationRequest(pageNum, pageSize);

            mentorsRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>(), null))
                .ReturnsAsync(mentorList);

            mentorsRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(count);

            //Act
            var action = async () => await mentorsServiceMock.GetPaginationAsync(filter);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetPaginationAsync_WhenCampaignIdIsNullAndNoMentorsFoundAndPageNumIsBiggerThanTotalPages_ShouldThrowException()
        {
            //Arrange
            var pageNum = 2;
            var pageSize = 10;

            var filter = new PaginationRequest(pageNum, pageSize);

            mentorsRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(0);

            //Act
            var action = async () => await mentorsServiceMock.GetPaginationAsync(filter);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetPaginationAsync_WhenCampaignIdIsNullAndNoMentorsFound_ShouldReturnEmptyCollection()
        {
            //Arrange
            var pageNum = 1;
            var pageSize = 10;

            var filter = new PaginationRequest(pageNum, pageSize);

            mentorsRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(0);

            //Act
            var response = await mentorsServiceMock.GetPaginationAsync(filter);

            //Assert
            Assert.Empty(response.Content);
        }

        [Fact]
        public async Task GetPaginationAsync_WhenCampaignIdIsSetAndCampaignHasNoMentorsAndPageNumIsBiggerThanTotalPages_ShouldThrowException()
        {
            //Arrange
            var pageNum = 2;
            var pageSize = 10;

            var filter = new PaginationRequest(pageNum, pageSize);

            mentorsRepositoryMock
                .Setup(x => x.GetCountByCampaignIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(0);

            //Act
            var action = async () => await mentorsServiceMock.GetPaginationAsync(filter, Guid.NewGuid());

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetPaginationAsync_WhenCampaignIdIsSetAndCampaignHasNoMentors_ShouldThrowException()
        {
            //Arrange
            var pageNum = 1;
            var pageSize = 10;

            var filter = new PaginationRequest(pageNum, pageSize);

            mentorsRepositoryMock
                .Setup(x => x.GetCountByCampaignIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(0);

            //Act
            var response = await mentorsServiceMock.GetPaginationAsync(filter, Guid.NewGuid());

            //Assert
            Assert.Empty(response.Content);
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
            var mentor2 = new Mentor()
            {
                Id = Guid.NewGuid(),
                DisplayName = "Ab Cd",
                Email = "abcd@gmail.com",
                Specialities = new List<Speciality>(),
                Campaigns = new List<Campaign>()
            };

            var mentorList = new List<Mentor>() { returnMentor, mentor2 };

            mentorsRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(mentorList.Count);

            mentorsRepositoryMock
                .Setup(x => x.GetAllAsync(null, null))
                .ReturnsAsync(mentorList);

            //Act
            var response = await mentorsServiceMock.GetAllAsync();

            //Assert
            Assert.Equal(mentorList.Count, response.Count());
        }

        #endregion

        #region GetMentorCountByCampaignIdAsyncTests

        [Fact]
        public async Task GetMentorCountByCampaignIdAsync_WhenCampaignHasMentors_ShouldReturnCorrectCount()
        {
            //Arrange
            var newId = Guid.NewGuid();

            var mentorList = new List<Mentor>() { returnMentor };

            mentorsRepositoryMock
                 .Setup(x => x.GetCountByCampaignIdAsync(It.IsAny<Guid>()))
                 .ReturnsAsync(mentorList.Count);

            //Act
            var count = await mentorsServiceMock.GetCountByCampaignIdAsync(newId);

            //Assert
            Assert.Equal(mentorList.Count, count);
        }

        [Fact]
        public async Task GetMentorCountByCampaignIdAsync_WhenCampaignHasNoMentors_ShouldReturnZeroCount()
        {
            //Arrange
            var newId = Guid.NewGuid();

            //Act
            var count = await mentorsServiceMock.GetCountByCampaignIdAsync(newId);

            //Assert
            Assert.Equal(0, count);
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
        public async Task GetCountAsync_WhenNoMentorsFound_ShouldThrowError()
        {
            //Act
            var response = await mentorsServiceMock.GetCountAsync();

            //Assert
            Assert.Equal(0, response);
        }

        #endregion

        #region AddToCampaignAsyncTests

        [Fact]
        public async Task AssignToCampaignAsync_WhenCampaignNotFound_ShouldThrowError()
        {
            //Arrange
            var request = new AddToCampaignRequest(Guid.NewGuid(), Guid.NewGuid());
            
            //Act
            var action = async() => await mentorsServiceMock.AddToCampaignAsync(request);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task AssignToCampaignAsync_WhenMentorNotFound_ShouldThrowError()
        {
            //Arrange
            var request = new AddToCampaignRequest(Guid.NewGuid(), Guid.NewGuid());

            var campaign = new Campaign()
            {
                Id = request.CampaignId,
                Name = "Test"
            };

            campaignsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaign);

            //Act
            var action = async () => await mentorsServiceMock.AddToCampaignAsync(request);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task AssignToCampaignAsync_WhenMentorAlreadyAssignedToCampaign_ShouldThrowError()
        {
            //Arrange
            var request = new AddToCampaignRequest(Guid.NewGuid(), Guid.NewGuid());

            var campaign = new Campaign()
            {
                Id = request.CampaignId,
                Name = "Test"
            };

            var mentor = new Mentor()
            {
                Id = request.PersonId,
                DisplayName = mentorDisplayName,
                Email = mentorEmail,
                Campaigns = new List<Campaign>() { campaign },
                Specialities = new List<Speciality>()
            };

            campaignsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaign);

            mentorsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mentor);

            //Act
            var action = async () => await mentorsServiceMock.AddToCampaignAsync(request);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task AssignToCampaignAsync_WhenMentorNotAssignedToCampaign_ShouldAddMentorToCampaign()
        {
            //Arrange
            var request = new AddToCampaignRequest(Guid.NewGuid(), Guid.NewGuid());

            var campaign = new Campaign()
            {
                Id = request.CampaignId,
                Name = "Test"
            };

            var mentor = new Mentor()
            {
                Id = request.PersonId,
                DisplayName = mentorDisplayName,
                Email = mentorEmail,
                Campaigns = new List<Campaign>(),
                Specialities = new List<Speciality>()
            };

            campaignsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaign);

            mentorsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mentor);

            //Act
            await mentorsServiceMock.AddToCampaignAsync(request);

            //Arrange
            var response = await mentorsRepositoryMock.Object.GetByIdAsync(request.PersonId);

            Assert.NotNull(response);
            Assert.Equal(campaign, response.Campaigns.First());
        }

        #endregion

        private CreateMentorRequest CreateValidCreateMentorRequest()
        {
            //Arrange
            return new CreateMentorRequest(mentorDisplayName, mentorEmail, specialityIds);
        }

        private UpdateMentorRequest CreateUpdateMentorRequest()
        {
            return new UpdateMentorRequest(id, specialityIds);
        }
    }
}
