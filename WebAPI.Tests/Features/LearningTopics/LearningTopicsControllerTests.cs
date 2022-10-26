using Core.Common.Exceptions;
using Core.Features.LearningTopics.Support;
using Core.Features.LearningTopics.Interfaces;
using Core.Features.LearningTopics.RequestModels;
using Core.Features.LearningTopics.ResponseModels;
using Core.Features.Specialties.Entities;
using Core.Features.Specialities.ResponseModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Features.LearningTopics;
using Xunit;
using Core.Common.Pagination;

namespace WebAPI.Tests.Features.LearningTopics
{
    public class LearningTopicsControllerTests
    {
        private readonly Mock<ILearningTopicsService> learningTopicsService;
        private readonly LearningTopicsController learningTopicsController;
        
        private readonly Guid id = Guid.NewGuid();
        private readonly string name = "Test Learning Topic";

        private readonly List<SpecialitySummaryResponse> specialitySummaries;
        private readonly List<Guid> specialityIds;

        private readonly PaginationRequest validPaginationRequest;
        private readonly PaginationRequest emptyPaginationRequest;

        public static IEnumerable<object[]> InvalidPaginationRequests =>
            new List<object[]>
            {
                new object[] { new PaginationRequest(null, 1) },
                new object[] { new PaginationRequest(1, null) },
                new object[] { new PaginationRequest(1, -1) },
                new object[] { new PaginationRequest(0, 1) },
                new object[] { new PaginationRequest(-1, -1) },

            };

        public static readonly IEnumerable<object[]> validNames = new List<object[]>
        {
            new object[] { TestHelper.GenerateString(LearniningTopicValidationConstants.NameMinLength) },
            new object[] { TestHelper.GenerateString(LearniningTopicValidationConstants.NameMaxLength) },
            new object[] { NameEdgeCaseTestHelper.NameWithWhiteSpace }
        };

        public static readonly IEnumerable<object[]> invalidNames = new List<object[]>
        {
            new object[] { string.Empty },
            new object[] { TestHelper.GenerateString(LearniningTopicValidationConstants.NameMinLength - 1) },
            new object[] { TestHelper.GenerateString(LearniningTopicValidationConstants.NameMaxLength + 1) },
            new object[] { NameEdgeCaseTestHelper.NameWithAmpersand },
            new object[] { NameEdgeCaseTestHelper.NameWithDash },
            new object[] { NameEdgeCaseTestHelper.NameWithDot },
            new object[] { NameEdgeCaseTestHelper.NameWithExclamationMark },
            new object[] { NameEdgeCaseTestHelper.NameWithNumberSign },
            new object[] { NameEdgeCaseTestHelper.NameWithDigit }
        };

        public LearningTopicsControllerTests()
        {
            var createLearningTopicValidator = new CreateLearningTopicRequestValidator();
            var updateLearningTopicValidator = new UpdateLearningTopicRequestValidator();
            var paginationRequestValidator = new PaginationRequestValidator();
            var learningTopicsControllerLogger = new Mock<ILogger<LearningTopicsController>>();
            
            learningTopicsService = new Mock<ILearningTopicsService>();

            var learningTopicValidator = new LearningTopicValidator(
                createLearningTopicValidator, updateLearningTopicValidator);

            learningTopicsController = new LearningTopicsController(
                learningTopicsService.Object,
                learningTopicValidator,
                paginationRequestValidator,
                learningTopicsControllerLogger.Object
            );

            var speciality = new Speciality()
            {
                Id = Guid.NewGuid(),
                Name = "Backend"
            };

            var specialitySummary = new SpecialitySummaryResponse(speciality.Id, speciality.Name);

            specialityIds = new List<Guid>() { speciality.Id };
            specialitySummaries = new List<SpecialitySummaryResponse>() { specialitySummary };

            validPaginationRequest = new PaginationRequest(1, 10);
            emptyPaginationRequest = new PaginationRequest(null, null);
        }

        [Fact]
        public async Task CreateAsync_WhenDataIsCorrect_ShouldReturnCorrectData()
        {
            // Arrange
            var learningTopicRequest = new CreateLearningTopicRequest(name, specialityIds);
            var learningTopicSummary = new LearningTopicSummaryResponse(id, name, specialitySummaries);

            learningTopicsService
                .Setup(x => x.CreateAsync(It.IsAny<CreateLearningTopicRequest>()))
                .ReturnsAsync(learningTopicSummary);

            // Act
            var actionResult = await learningTopicsController.CreateAsync(learningTopicRequest);

            // Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;
            
            Assert.NotNull(jsonResult);

            var learningTopicResponse = jsonResult!.Value as CoreResponse<LearningTopicSummaryResponse>;

            Assert.Equal(learningTopicSummary.Id, learningTopicResponse!.Data!.Id);
            Assert.Equal(learningTopicSummary.Name, learningTopicResponse.Data.Name);
            Assert.Equal(learningTopicSummary.Specialities, learningTopicResponse.Data.Specialities);

        }

        [Theory]
        [MemberData(nameof(invalidNames))]
        public async Task CreateAsync_WhenNameIsInvalid_ShouldThrowException(string invalidName)
        {
            // Arrange
            var learningTopicRequest = new CreateLearningTopicRequest(invalidName, specialityIds);

            // Act
            var action = async () => await learningTopicsController.CreateAsync(learningTopicRequest);
            
            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(validNames))]
        public async Task CreateAsync_WhenNameIsValid_ShouldReturnCorrectData(string validName)
        {
            // Arrange
            var learningTopicRequest = new CreateLearningTopicRequest(validName, specialityIds);
            var learningTopicSummary = new LearningTopicSummaryResponse(id, validName, specialitySummaries);

            learningTopicsService
                .Setup(x => x.CreateAsync(It.IsAny<CreateLearningTopicRequest>()))
                .ReturnsAsync(learningTopicSummary);

            // Act
            var actionResult = await learningTopicsController.CreateAsync(learningTopicRequest);

            // Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;
            
            Assert.NotNull(jsonResult);

            var learningTopicResponse = jsonResult!.Value as CoreResponse<LearningTopicSummaryResponse>;

            Assert.Equal(learningTopicSummary.Id, learningTopicResponse!.Data!.Id);
            Assert.Equal(learningTopicSummary.Name, learningTopicResponse.Data.Name);
            Assert.Equal(learningTopicSummary.Specialities, learningTopicResponse.Data.Specialities);
        }

        [Fact]
        public async Task UpdateAsync_WhenDataIsCorrect_ShouldReturnCorrectData()
        {
            // Arrange
            var learningTopicRequest = new UpdateLearningTopicRequest(id, name, specialityIds);
            var learningTopicSummary = new LearningTopicSummaryResponse(id, name, specialitySummaries);

            learningTopicsService
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateLearningTopicRequest>()))
                .ReturnsAsync(learningTopicSummary);

            // Act
            var actionResult = await learningTopicsController.UpdateAsync(id, learningTopicRequest);

            // Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;
            
            Assert.NotNull(jsonResult);

            var learningTopicResponse = jsonResult!.Value as CoreResponse<LearningTopicSummaryResponse>;

            Assert.Equal(learningTopicSummary.Id, learningTopicResponse!.Data!.Id);
            Assert.Equal(learningTopicSummary.Name, learningTopicResponse.Data.Name);
            Assert.Equal(learningTopicSummary.Specialities, learningTopicResponse.Data.Specialities);
        }

        [Fact]
        public async Task UpdateAsync_WhenIdIsEmpty_ShouldThrowException()
        {
            // Arrange
            var emptyId = Guid.Empty;
            var learningTopicRequest = new UpdateLearningTopicRequest(emptyId, name, specialityIds);

            // Act
            var action = async () => await learningTopicsController.UpdateAsync(emptyId, learningTopicRequest);
            
            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(invalidNames))]
        public async Task UpdateAsync_WhenNameIsInvalid_ShouldThrowException(string invalidName)
        {
            // Arrange
            var learningTopicRequest = new UpdateLearningTopicRequest(id, invalidName, specialityIds);

            // Act
            var action = async () => await learningTopicsController.UpdateAsync(id, learningTopicRequest);
            
            // Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(validNames))]
        public async Task UpdateAsync_WhenNameIsValid_ShouldReturnCorrectData(string validName)
        {
            // Arrange
            var learningTopicRequest = new UpdateLearningTopicRequest(id, validName, specialityIds);
            var learningTopicSummary = new LearningTopicSummaryResponse(id, validName, specialitySummaries);

            learningTopicsService
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateLearningTopicRequest>()))
                .ReturnsAsync(learningTopicSummary);

            // Act
            var actionResult = await learningTopicsController.UpdateAsync(id, learningTopicRequest);

            // Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;
            
            Assert.NotNull(jsonResult);

            var learningTopicResponse = jsonResult!.Value as CoreResponse<LearningTopicSummaryResponse>;

            Assert.Equal(learningTopicSummary.Id, learningTopicResponse!.Data!.Id);
            Assert.Equal(learningTopicSummary.Name, learningTopicResponse.Data.Name);
            Assert.Equal(learningTopicSummary.Specialities, learningTopicResponse.Data.Specialities);
        }

        [Fact]
        public async Task UpdateAsync_WhenQueryIdAndModelIdDoNotMatch_ShouldThrowException()
        {
            // Arrange
            var learningTopicRequest = new UpdateLearningTopicRequest(id, name, specialityIds);

            // Act
            var action = async () => await learningTopicsController.UpdateAsync(Guid.NewGuid(), learningTopicRequest);
            
            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdIsFound_ShouldShouldReturnCorrectData()
        {
            // Arrange
            var learningTopicSummary = new LearningTopicSummaryResponse(id, name, specialitySummaries);

            learningTopicsService
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(learningTopicSummary);

            // Act
            var actionResult = await learningTopicsController.GetByIdAsync(id);

            // Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;
            
            Assert.NotNull(jsonResult);

            var learningTopicResponse = jsonResult!.Value as CoreResponse<LearningTopicSummaryResponse>;

            Assert.Equal(learningTopicSummary.Id, learningTopicResponse!.Data!.Id);
            Assert.Equal(learningTopicSummary.Name, learningTopicResponse.Data.Name);
            Assert.Equal(learningTopicSummary.Specialities, learningTopicResponse.Data.Specialities);
        }
        
        [Fact]
        public async Task GetByIdAsync_WhenIdIsNotFound_ShouldThrowException()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            learningTopicsService
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new CoreException(String.Empty, System.Net.HttpStatusCode.NotFound));

            // Act
            var action = async () => await learningTopicsController.GetByIdAsync(invalidId);

            // Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmptyAndNoPageParametersSet_ShouldReturnCorrectCountElements()
        {
            // Arrange
            var learningTopicSummary = new LearningTopicSummaryResponse(id, name, specialitySummaries);
            var additionalLearningTopicSummary = new LearningTopicSummaryResponse(Guid.NewGuid(), "Additional " + name, specialitySummaries);

            var learningTopicSummaries = new List<LearningTopicSummaryResponse>() { learningTopicSummary, additionalLearningTopicSummary };

            learningTopicsService
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(learningTopicSummaries);
            
            // Act
            var actionResult = await learningTopicsController.GetAllAsync(emptyPaginationRequest);

            // Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var learningTopicsResponse = jsonResult!.Value as CoreResponse<IEnumerable<LearningTopicSummaryResponse>>;

            Assert.Equal(learningTopicSummaries.Count, learningTopicsResponse!.Data!.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenEmptyAndNoPageParametersSet_ShouldReturnEmptyCollection()
        {
            // Arrange
            learningTopicsService
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<LearningTopicSummaryResponse>());

            // Act
            var actionResult = await learningTopicsController.GetAllAsync(emptyPaginationRequest);

            // Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var learingTopicsResponse = jsonResult!.Value as CoreResponse<IEnumerable<LearningTopicSummaryResponse>>;

            Assert.Empty(learingTopicsResponse!.Data);
        }


        [Theory]
        [MemberData(nameof(InvalidPaginationRequests))]
        public async Task GetAllAsync_WhenPageParametersAreInvalid_ShouldThrowException(PaginationRequest paginationRequest)
        {
            //Act
            var action = async () => await learningTopicsController.GetAllAsync(paginationRequest);

            //Arrange
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmptyAndBothParametersAreValid_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var learningTopicSummary = new LearningTopicSummaryResponse(id, name, specialitySummaries);
            var additionalLearningTopicSummary = new LearningTopicSummaryResponse(Guid.NewGuid(), "Additional " + name, specialitySummaries);

            var learningTopicSummaries = new List<LearningTopicSummaryResponse>() { learningTopicSummary, additionalLearningTopicSummary };

            var expectedResponse = new PaginationResponse<LearningTopicSummaryResponse>(learningTopicSummaries, 
                validPaginationRequest!.PageNum!.Value, 1);

            learningTopicsService
                .Setup(x => x.GetPaginationAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(expectedResponse);

            //Act
            var actionResult = await learningTopicsController.GetAllAsync(validPaginationRequest);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var actualResponse = jsonResult!.Value as CoreResponse<PaginationResponse<LearningTopicSummaryResponse>>;

            Assert.Equal(expectedResponse.Content.Count(), actualResponse!.Data!.Content.Count());
        }
    }
}