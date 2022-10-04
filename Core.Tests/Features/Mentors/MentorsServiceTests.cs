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
        private readonly string mentorFirstName = "Iliyan";
        private readonly string mentorLastName = "Dimitrov";
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

        public static IEnumerable<object[]> invalidNames = new List<object[]>
        {
            new object[] { TestHelper.GenerateString(MentorValidationConstraints.NamesMinLength - 1) },
            new object[] { TestHelper.GenerateString(MentorValidationConstraints.NamesMaxLength + 1) },
            new object[] { "Name1" },
            new object[] { " Name" },
            new object[] { "Name " },
        };

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
                FirstName = mentorFirstName,
                LastName = mentorLastName,
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
            Assert.Equal(mentorFirstName, response.FirstName);
            Assert.Equal(mentorLastName, response.LastName);
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

        [Theory]
        [MemberData(nameof(invalidNames))]
        public async Task CreateAsync_WhenFirstNameIsInvalid_ShouldThrowException(string invalidFirstName)
        {
            //Arrange
            var request = new CreateMentorRequest(invalidFirstName, mentorLastName, "test@example.com", specialityIds);

            //Act
            var action = async () => await mentorsServiceMock.CreateAsync(request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(invalidNames))]
        public async Task CreateAsync_WhenLastNameIsInvalid_ShouldThrowException(string invalidLastName)
        {
            //Arrange
            var request = new CreateMentorRequest(mentorFirstName, invalidLastName, "test@example.com", specialityIds);

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
            var request = new CreateMentorRequest(mentorFirstName, mentorLastName, invalidEmail, specialityIds);

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
            Assert.Equal(request.FirstName, response.FirstName);
            Assert.Equal(request.LastName, response.LastName);
            Assert.Equal(request.Email, response.Email);
            Assert.NotEmpty(response.Specialities);
            Assert.Equal(request.SpecialityIds.Count(), response.Specialities.Count);
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
            Assert.Equal(request.FirstName, response.FirstName);
            Assert.Equal(request.LastName, response.LastName);
            Assert.Equal(request.Email, response.Email);
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
            Assert.Equal(request.FirstName, response.FirstName);
            Assert.Equal(request.LastName, response.LastName);
            Assert.Equal(request.Email, response.Email);
            Assert.NotNull(response.Specialities);
            Assert.Equal(request.SpecialityIds.Count(), response.Specialities.Count);
        }

        [Fact]
        public async Task UpdateAsync_WhenUpdatedEmailIsUsed_ShouldThrowException()
        {
            //Arrange
            var request = CreateUpdateMentorRequest();
            request.Email = "used.email@test.com";

            mentorsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(returnMentor);

            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);

            mentorsRepositoryMock
                .Setup(x => x.IsEmailUsed(It.IsAny<string>()))
                .ReturnsAsync(true);

            //Act
            var action = async () => await mentorsServiceMock.UpdateAsync(request);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenUpdatedEmailNotUsed_ShouldUpdate()
        {
            //Arrange
            var updatedEmail = "john.doe@endava.com";
            var request = CreateUpdateMentorRequest();

            var updatedMentor = new Mentor()
            { 
                Id = id,
                FirstName = mentorFirstName,
                LastName = mentorLastName,
                Email = updatedEmail,
                Specialities = specialities
            };

            request.Email = updatedEmail;

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
            Assert.Equal(updatedEmail, response.Email);
        }

        [Theory]
        [MemberData(nameof(invalidNames))]
        public async Task UpdateAsync_WhenFirstNameIsInvalid_ShouldThrowException(string invalidFirstName)
        {
            //Arrange
            var request = CreateUpdateMentorRequest();
            request.FirstName = invalidFirstName;

            //Act
            var action = async () => await mentorsServiceMock.UpdateAsync(request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);

        }

        [Theory]
        [MemberData(nameof(invalidNames))]
        public async Task UpdateAsync_WhenLastNameIsInvalid_ShouldThrowException(string invalidLastName)
        {
            //Arrange
            var request = CreateUpdateMentorRequest();
            request.LastName = invalidLastName;

            //Act
            var action = async () => await mentorsServiceMock.UpdateAsync(request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(invalidEmails))]
        public async Task UpdateAsync_WhenEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {
            //Arrange
            var request = CreateUpdateMentorRequest();
            request.Email = invalidEmail;

            //Act
            var action = async () => await mentorsServiceMock.UpdateAsync(request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
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
            Assert.Equal(mentorFirstName, response.FirstName);
            Assert.Equal(mentorLastName, response.LastName);
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

            var expectedPaginationResponse = new PaginationResponse<MentorSummaryResponse>(
                mentorList.ToMentorSummaryResponses(), 1, 4);

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
        public async Task GetPaginationAsync_WhenCampaignIdIsSetAndCampaignHasNoMentors_ShouldReturnEmptyCollection()
        {
            //Arrange
            var newId = Guid.NewGuid();

            var filter = new PaginationRequest(1, 20);

            var emptyList = new List<Mentor>();

            var expectedResponse = new PaginationResponse<MentorSummaryResponse>(
                emptyList.ToMentorSummaryResponses(), 1, 1);

            mentorsRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>(), It.IsAny<Guid>()))
                .ReturnsAsync(emptyList);

            //Act
            var actualResponse = await mentorsServiceMock.GetPaginationAsync(filter, newId);

            //Assert
            Assert.Empty(actualResponse.Content);
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
                FirstName = "Ab",
                LastName = "Cd",
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
                FirstName = mentorFirstName,
                LastName = mentorLastName,
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
                FirstName = mentorFirstName,
                LastName = mentorLastName,
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
            return new CreateMentorRequest(mentorFirstName, mentorLastName, mentorEmail, specialityIds);
        }

        private UpdateMentorRequest CreateUpdateMentorRequest()
        {
            return new UpdateMentorRequest(id, mentorFirstName, mentorLastName, mentorEmail, specialityIds);
        }
    }
}
