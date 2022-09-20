using Core.Common.Exceptions;
using Core.Features.LearningTopics;
using Core.Features.LearningTopics.Entities;
using Core.Features.LearningTopics.Interfaces;
using Core.Features.LearningTopics.RequestModels;
using Core.Features.LearningTopics.ResponseModels;
using Core.Features.LearningTopics.Support;
using Core.Features.Specialties.Entities;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.ResponseModels;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Features.LearningTopics
{
    public class LearningTopicsServiceTests
    {
        private Guid id = Guid.NewGuid();
        private string name = "Test Learning Topic";
        private Guid specialityId = Guid.NewGuid();
        private List<Guid> specialityIds;
        private List<Speciality> specialities;
        private List<SpecialitySummaryResponse> specialitySummaries;
        private LearningTopic  learningTopic;
        private readonly Mock<ILearningTopicsRepository> learningTopicRepositoryMock;
        private readonly Mock<ISpecialitiesRepository> specialitiesRepositoryMock;
        private readonly LearningTopicsService learningTopicsService;

        public static IEnumerable<object[]> invalidNames = new List<object[]>
        {
            new object[] { string.Empty },
            new object[] { TestHelper.GenerateString(LearniningTopicValidationConstants.NameMinLength - 1) },
            new object[] { TestHelper.GenerateString(LearniningTopicValidationConstants.NameMaxLength + 1) },
            new object[] { NameEdgeCaseTestHelper.NameWithAmpersand },
            new object[] { NameEdgeCaseTestHelper.NameWithDash },
            new object[] { NameEdgeCaseTestHelper.NameWithDot },
            new object[] { NameEdgeCaseTestHelper.NameWithExclamationMark },
            new object[] { NameEdgeCaseTestHelper.NameWithNumberSign }
        };

        public LearningTopicsServiceTests()
        {
            var createLearningTopicValidator = new CreateLearningTopicRequestValidator();
            var updateLearningTopicValidator = new UpdateLearningTopicRequestValidator();
            var learningTopicsServiceLoggerMock = new Mock<ILogger<LearningTopicsService>>();

            learningTopicRepositoryMock = new Mock<ILearningTopicsRepository>();
            specialitiesRepositoryMock = new Mock<ISpecialitiesRepository>();

            learningTopicsService = new LearningTopicsService(
                learningTopicRepositoryMock.Object,
                specialitiesRepositoryMock.Object,
                learningTopicsServiceLoggerMock.Object,
                createLearningTopicValidator,
                updateLearningTopicValidator
            );

            var speciality = new Speciality
            {
                Id = specialityId,
                Name = "Backend"
            };

            var specialitySummary = new SpecialitySummaryResponse(speciality.Id, speciality.Name);

            specialities = new List<Speciality>() { speciality };
            specialitySummaries = new List<SpecialitySummaryResponse>() { specialitySummary };
            specialityIds = new List<Guid>() { speciality.Id };

            learningTopic = new LearningTopic
            {
                Id = id,
                Name = name,
                Specialities = specialities
            };
        }

        #region CreateAsyncTest

        [Fact]
        public async Task CreateAsync_WhenRequestDataIsValid_ShouldReturnCorrectObject()
        {
            // Arrange
            var createLearningTopicRequest = new CreateLearningTopicRequest(name, specialityIds);
            
            learningTopicRepositoryMock
                .Setup(x => x.ExistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);
            
            learningTopicRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<LearningTopic>()))
                .ReturnsAsync(learningTopic);

            // Act
            var learningTopicReponse = await learningTopicsService.CreateAsync(createLearningTopicRequest);

            // Assert
            Assert.NotEqual(Guid.Empty, learningTopicReponse.Id);
            Assert.Equal(name, learningTopicReponse.Name);
            Assert.Equal(specialitySummaries, learningTopicReponse.Specialities);
        }

        [Theory]
        [MemberData(nameof(invalidNames))]
        public async void CreateAsync_WhenNameIsInvalid_ShouldThrowException(string invalidName)
        {
            // Arrange
            var createLearningTopicRequest = new CreateLearningTopicRequest(invalidName, specialityIds);
            
            // Act
            var action = async () => await learningTopicsService.CreateAsync(createLearningTopicRequest);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenNameIsDuplicated_ShouldThrowException()
        {
            // Arrange
            var createLearningTopicRequest = new CreateLearningTopicRequest(name, specialityIds);
            
            learningTopicRepositoryMock
                .Setup(x => x.ExistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var action = async () => await learningTopicsService.CreateAsync(createLearningTopicRequest);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenSpecialitiesAreDuplicated_ShouldThrowException()
        {
            // Arrange
            var duplicateSpecialities = specialityIds;

            duplicateSpecialities.Add(specialityIds[0]);

            var createLearningTopicRequest = new CreateLearningTopicRequest(name, duplicateSpecialities);
            
            learningTopicRepositoryMock
                .Setup(x => x.ExistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(specialities);

            // Act
            var action = async () => await learningTopicsService.CreateAsync(createLearningTopicRequest);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenSpecialitiesNotFound_ShouldThrowException()
        {
            // Arrange
            var notFoundSpecialities = specialityIds;

            notFoundSpecialities.Add(Guid.NewGuid());

            var createLearningTopicRequest = new CreateLearningTopicRequest(name, notFoundSpecialities);
            
            learningTopicRepositoryMock
                .Setup(x => x.ExistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(specialities);

            // Act
            var action = async () => await learningTopicsService.CreateAsync(createLearningTopicRequest);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        #endregion
        
        #region UpdateAsyncTest
        
        [Fact]
        public async Task UpdateAsync_WhenRequestDataIsValid_ShouldReturnCorrectObject()
        {
            // Arrange
            var updateLearningTopicRequest = new UpdateLearningTopicRequest(id, name, specialityIds);
            
            learningTopicRepositoryMock
                .Setup(x => x.ExistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(specialities);
            
            learningTopicRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(learningTopic);
            
            learningTopicRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<LearningTopic>()))
                .ReturnsAsync(learningTopic);

            // Act
            var learningTopicReponse = await learningTopicsService.UpdateAsync(updateLearningTopicRequest);

            // Assert
            Assert.Equal(id, learningTopicReponse.Id);
            Assert.Equal(name, learningTopicReponse.Name);
            Assert.Equal(specialitySummaries, learningTopicReponse.Specialities);
        }

        [Theory]
        [MemberData(nameof(invalidNames))]
        public async void UpdateAsync_WhenNameIsInvalid_ShouldThrowException(string invalidName)
        {
            // Arrange
            var updateLearningTopicRequest = new UpdateLearningTopicRequest(id, invalidName, specialityIds);
            
            // Act
            var action = async () => await learningTopicsService.UpdateAsync(updateLearningTopicRequest);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenIdIsNotFound_ShouldThrowException()
        {
            // Arrange
            var updateLearningTopicRequest = new UpdateLearningTopicRequest(id, name, specialityIds);

            // Act
            var action = async () => await learningTopicsService.UpdateAsync(updateLearningTopicRequest);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenNameIsDuplicated_ShouldThrowException()
        {
            // Arrange
            var updatedName = name + " Updated";
            var updateLearningTopicRequest = new UpdateLearningTopicRequest(id, updatedName, specialityIds);
            
            learningTopicRepositoryMock
                .Setup(x => x.ExistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var action = async () => await learningTopicsService.UpdateAsync(updateLearningTopicRequest);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenSpecialitiesAreDuplicated_ShouldThrowException()
        {
            // Arrange
            var duplicateSpecialities = specialityIds;

            duplicateSpecialities.Add(specialityIds[0]);

            var updateLearningTopicRequest = new UpdateLearningTopicRequest(id, name, duplicateSpecialities);
            
            learningTopicRepositoryMock
                .Setup(x => x.ExistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(specialities);

            // Act
            var action = async () => await learningTopicsService.UpdateAsync(updateLearningTopicRequest);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenSpecialitiesNotFound_ShouldThrowException()
        {
            // Arrange
            var notFoundSpecialities = specialityIds;

            notFoundSpecialities.Add(Guid.NewGuid());

            var updateLearningTopicRequest = new UpdateLearningTopicRequest(id, name, notFoundSpecialities);
            
            learningTopicRepositoryMock
                .Setup(x => x.ExistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            specialitiesRepositoryMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(specialities);

            // Act
            var action = async () => await learningTopicsService.UpdateAsync(updateLearningTopicRequest);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        #endregion

        #region GetByIdAsyncTest
        
        [Fact]
        public async Task GetByIdAsync_WhenIdIsFound_ShouldReturnCorrectObject()
        {
            // Arrange
            learningTopicRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(learningTopic);

            // Act
            var learningTopicReponse = await learningTopicsService.GetByIdAsync(id);

            // Assert
            Assert.NotNull(learningTopicReponse);
            Assert.Equal(id, learningTopicReponse.Id);
            Assert.Equal(name, learningTopicReponse.Name);
            Assert.Equal(specialities.Count, learningTopicReponse.Specialities.Count);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdIsNotFound_ShouldReturnCorrectObject()
        {
            // Arrange
            var learningTopicId = Guid.NewGuid();

            // Act
            var action = async () => await learningTopicsService.GetByIdAsync(learningTopicId);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        #endregion

        #region GetAllAsyncTest

        [Fact]
        public async Task GetAllAsync_WhenExist_ShouldReturnCorrectCount()
        {
            // Arrange
            var learningTopics = new List<LearningTopic>() { learningTopic };
            var learningTopicSummaries = new List<LearningTopicSummaryResponse>()
            {
                new LearningTopicSummaryResponse(id, name, specialitySummaries)
            };

            learningTopicRepositoryMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(learningTopics);

            // Act
            var specialties = await learningTopicsService.GetAllAsync();

            // Assert
            Assert.Equal(learningTopics.Count, learningTopicSummaries.Count);
            Assert.Equal(learningTopics[0].Specialities.Count, learningTopicSummaries[0].Specialities.Count);
        }

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            learningTopicRepositoryMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<LearningTopic>());

            // Act
            var learningTopics = await learningTopicsService.GetAllAsync();

            // Assert
            Assert.Empty(learningTopics);
        }

        #endregion
    }
}