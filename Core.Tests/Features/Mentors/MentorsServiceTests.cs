using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Mentors;
using Core.Features.Mentors.Entities;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
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
        private Mock<ISpecialitiesRepository> specialitiesRepositoryMock;
        private MentorsService mentorsServiceMock;
        private Mentor returnMentor;
        private Speciality speciality;
        private List<Speciality> specialities;
        private List<Guid> specialityIds;

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
            var filterCampaignsRequestValidator = new PaginationFilterRequestValidator();

            mentorsRepositoryMock = new Mock<IMentorsRepository>();
            specialitiesRepositoryMock = new Mock<ISpecialitiesRepository>();

            var mockLogger = new Mock<ILogger<MentorsService>>();

            mentorsServiceMock = new MentorsService(
                mentorsRepositoryMock.Object, specialitiesRepositoryMock.Object, mockLogger.Object,
                createMentorRequestValidator, updateMentorRequestValidator, filterCampaignsRequestValidator);

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

            returnMentor = new Mentor()
            {
                Id = id,
                FirstName = mentorFirstName,
                LastName = mentorLastName,
                Email = mentorEmail,
                Specialities = specialities
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

            mentorsRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Mentor>()))
                .ReturnsAsync(returnMentor);

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

            mentorsRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Mentor>()))
                .ReturnsAsync(returnMentor);

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

            mentorsRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Mentor>()))
                .ReturnsAsync(updatedMentor);

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

        #region GetAllAsyncTests

        [Theory]
        [InlineData(0, 5, 15)]
        [InlineData(5, 5, 15)]
        [InlineData(10, 5, 15)]
        [InlineData(0, 20, 5)]
        [InlineData(1, 1, 10)]
        public async Task GetAllAsync_WhenFilterIsCorrect_ShouldGetData(int skip, int take, int count)
        {
            //Arrange
            var mentorList = new List<Mentor>()
            {
                returnMentor
            };

            var filter = new PaginationFilterRequest()
            {
                Skip = skip,
                Take = take,
                Count = count
            };

            mentorsRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationFilterRequest>()))
                .ReturnsAsync(mentorList);

            //Act
            var mentors = (await mentorsServiceMock.GetAllAsync(filter)).ToList();

            //Assert
            Assert.Equal(mentorList.Count, mentors.Count);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(10)]
        [InlineData(20)]
        public async Task GetAllAsync_WhenSkipIsInvalid_ShouldThrowException(int invalidSkip)
        {
            //Arrange
            var filter = new PaginationFilterRequest()
            {
                Skip = invalidSkip,
                Take = 10,
                Count = 10
            };

            //Act
            var action = async () => await mentorsServiceMock.GetAllAsync(filter);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetAllAsync_WhenTakeIsLessThanOne_ShouldThrowException(int invalidTake)
        {
            //Arrange
            var filter = new PaginationFilterRequest()
            {
                Skip = 0,
                Take = invalidTake,
                Count = 10
            };

            //Act
            var action = async () => await mentorsServiceMock.GetAllAsync(filter);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [InlineData(10, 5, 10)]
        [InlineData(15, 5, 10)]
        public async Task GetAllAsync_WhenSkipIsGreaterThanOrEqualToCount_ShouldThrowException(int skip, int take, int count)
        {
            //Arrange
            var filter = new PaginationFilterRequest()
            {
                Skip = skip,
                Take = take,
                Count = count
            };

            //Act
            var action = async () => await mentorsServiceMock.GetAllAsync(filter);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        #endregion

        #region GetMentorsByCampaignIdAsyncTests

        [Fact]
        public async Task GetMentorsByCampaignIdAsync_WhenCampaignHasMentors_ShouldReturnCorrectCount()
        {
            //Arrange
            var newId = Guid.NewGuid();

            var mentorList = new List<Mentor>() { returnMentor };

            var filter = new PaginationFilterRequest()
            {
                Skip = 0,
                Take = 5,
                Count = 10
            };

            mentorsRepositoryMock
                .Setup(x => x.GetMentorsByCampaignIdAsync(It.IsAny<Guid>(), It.IsAny<PaginationFilterRequest>()))
                .ReturnsAsync(mentorList);

            //Act
            var mentors = await mentorsServiceMock.GetMentorsByCampaignIdAsync(newId, filter);

            //Assert
            Assert.Equal(mentorList.Count, mentors.Count());
        }

        [Fact]
        public async Task GetMentorsByCampaignIdAsync_WhenCampaignHasNoMentors_ShouldReturnEmptyCollection()
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
            var mentors = await mentorsServiceMock.GetMentorsByCampaignIdAsync(newId, filter);

            //Assert
            Assert.Empty(mentors);
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
