using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Mentors.Support;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialties.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Features.Mentors;
using Xunit;

namespace WebAPI.Tests.Features.Mentors
{
    public class MentorsControllerTests
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly string firstName = "First";
        private readonly string lastName = "Last";
        private readonly string email = "first.last@example.bg";

        private readonly int validPageNum = 1;
        private readonly int invalidPageNum = -1;
        private readonly int validPageSize = 10;
        private readonly int invalidPageSize = 0;

        private readonly Mock<IMentorsService> mentorsServiceMock;
        private readonly MentorsController mentorsController;

        private readonly SpecialitySummaryResponse specialitySummaryResponse;
        private Speciality speciality;
        private List<SpecialitySummaryResponse> specialitySummaries;
        private List<Guid> specialityIds;

        public MentorsControllerTests()
        {
            var createMentorRequestValidator = new CreateMentorRequestValidator();
            var updateMentorRequestValidator = new UpdateMentorRequestValidator();
            var paginationRequestValidator = new PaginationRequestValidator();

            mentorsServiceMock = new Mock<IMentorsService>();

            var loggerMock = new Mock<ILogger<MentorsController>>();

            mentorsController = new(
                mentorsServiceMock.Object,
                loggerMock.Object,
                createMentorRequestValidator,
                updateMentorRequestValidator,
                paginationRequestValidator);

            speciality = new Speciality()
            {
                Id = Guid.NewGuid(),
                Name = "Backend"
            };

            specialityIds = new List<Guid>() { speciality.Id };

            specialitySummaryResponse = new SpecialitySummaryResponse(speciality.Id, speciality.Name);

            specialitySummaries = new List<SpecialitySummaryResponse>() { specialitySummaryResponse };
        }

        [Fact]
        public async Task CreateAsync_WhenDataIsCorrect_ShouldReturnCorrectData()
        {
            //Arrange
            var request = new CreateMentorRequest(firstName, lastName, email, specialityIds);

            var mentorSummary = new MentorSummaryResponse(id, firstName, lastName, email, specialitySummaries);

            mentorsServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateMentorRequest>()))
                .ReturnsAsync(mentorSummary);

            //Act
            var actionResult = await mentorsController.CreateAsync(request);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var createdResponse = jsonResult!.Value as CoreResponse<MentorSummaryResponse>;

            Assert.Equal(mentorSummary.Id, createdResponse.Data.Id);
            Assert.Equal(mentorSummary.FirstName, createdResponse.Data.FirstName);
            Assert.Equal(mentorSummary.LastName, createdResponse.Data.LastName);
            Assert.Equal(mentorSummary.Email, createdResponse.Data.Email);
            Assert.Equal(mentorSummary.Specialities, createdResponse.Data.Specialities);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(MentorValidationConstraints.NamesMinLength - 1)]
        [InlineData(MentorValidationConstraints.NamesMaxLength + 1)]
        public async Task CreateAsync_WhenNamesAreWithInvalidLength_ShouldThrowException(int nameLength)
        {
            //Arrange
            var nameWithInvalidLength = TestHelper.GenerateString(nameLength);

            var request = new CreateMentorRequest(nameWithInvalidLength, nameWithInvalidLength, email, specialityIds);

            //Act
            var action = async () => await mentorsController.CreateAsync(request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [InlineData("john@endava.com.")]
        [InlineData("john@endava.co_uk")]
        [InlineData("joh n@endava.co_uk")]
        public async Task CreateAsync_WhenEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {
            //Arrange
            var request = new CreateMentorRequest(firstName, lastName, invalidEmail, specialityIds);

            //Act
            var action = async () => await mentorsController.CreateAsync(request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenDataIsCorrectShouldReturnCorrectData()
        {
            //Assert
            var request = new UpdateMentorRequest(id, firstName, lastName, email, specialityIds);

            var mentorSummary = new MentorSummaryResponse(id, firstName, lastName, email, specialitySummaries);

            mentorsServiceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateMentorRequest>()))
                .ReturnsAsync(mentorSummary);

            //Act
            var actionResult = await mentorsController.UpdateAsync(id, request);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var updatedResponse = jsonResult!.Value as CoreResponse<MentorSummaryResponse>;

            Assert.Equal(mentorSummary.Id, updatedResponse.Data.Id);
            Assert.Equal(mentorSummary.FirstName, updatedResponse.Data.FirstName);
            Assert.Equal(mentorSummary.LastName, updatedResponse.Data.LastName);
            Assert.Equal(mentorSummary.Email, updatedResponse.Data.Email);
            Assert.Equal(mentorSummary.Specialities, updatedResponse.Data.Specialities);
        }

        [Theory]
        [InlineData(MentorValidationConstraints.NamesMinLength)]
        [InlineData(MentorValidationConstraints.NamesMaxLength)]
        public async Task UpdateAsync_WhenNamesAreWithValidLength_ShouldReturnCorrectData(int nameLength)
        {
            //Arrange
            var nameWithValidLength = TestHelper.GenerateString(nameLength);

            var request = new UpdateMentorRequest(id, nameWithValidLength, nameWithValidLength, email, specialityIds);

            var mentorSummary = new MentorSummaryResponse(id, nameWithValidLength, nameWithValidLength, email, specialitySummaries);

            mentorsServiceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateMentorRequest>()))
                .ReturnsAsync(mentorSummary);

            //Act
            var actionResult = await mentorsController.UpdateAsync(id, request);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var updatedResponse = jsonResult!.Value as CoreResponse<MentorSummaryResponse>;

            Assert.Equal(mentorSummary.Id, updatedResponse.Data.Id);
            Assert.Equal(mentorSummary.FirstName, updatedResponse.Data.FirstName);
            Assert.Equal(mentorSummary.LastName, updatedResponse.Data.LastName);
            Assert.Equal(mentorSummary.Email, updatedResponse.Data.Email);
            Assert.Equal(mentorSummary.Specialities, updatedResponse.Data.Specialities);
        }

        [Theory]
        [InlineData("john@endava.com.")]
        [InlineData("john@endava.co_uk")]
        [InlineData("joh n@endava.co_uk")]
        public async Task UpdateAsync_WhenEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {
            //Arrange
            var request = new UpdateMentorRequest(id, firstName, lastName, invalidEmail, specialityIds);

            //Act
            var action = async () => await mentorsController.UpdateAsync(id, request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(MentorValidationConstraints.NamesMinLength - 1)]
        [InlineData(MentorValidationConstraints.NamesMaxLength + 1)]
        public async Task UpdateAsync_WhenNamesAreWithInvalidLength_ShouldThrowException(int nameLength)
        {
            //Arrange
            var nameWithInvalidLength = TestHelper.GenerateString(nameLength);

            var request = new UpdateMentorRequest(id, nameWithInvalidLength, nameWithInvalidLength, email, specialityIds);

            //Act
            var action = async () => await mentorsController.UpdateAsync(id, request);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenQueryIdAndModelIdDoesNotMatch_ShouldThrowException()
        {
            //Arrange
            var request = new UpdateMentorRequest(id, firstName, lastName, email, specialityIds);

            //Act
            var action = async () => await mentorsController.UpdateAsync(Guid.NewGuid(), request);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdNotFound_ShouldThrowException()
        {
            //Arrange
            var newId = Guid.NewGuid();

            mentorsServiceMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new CoreException(String.Empty, System.Net.HttpStatusCode.NotFound));

            //Act
            var action = async () => await mentorsController.GetByIdAsync(newId);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdExists_ShouldReturnCorrectObject()
        {
            //Arrange
            var expectedResponse = new MentorSummaryResponse(id, firstName, lastName, email, specialitySummaries);

            mentorsServiceMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedResponse);

            //Act
            var actionResult = await mentorsController.GetByIdAsync(id);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var actualResponse = jsonResult!.Value as CoreResponse<MentorSummaryResponse>;

            Assert.Equal(expectedResponse.Id, actualResponse.Data.Id);
            Assert.Equal(expectedResponse.FirstName, actualResponse.Data.FirstName);
            Assert.Equal(expectedResponse.LastName, actualResponse.Data.LastName);
            Assert.Equal(expectedResponse.Email, actualResponse.Data.Email);
            Assert.Equal(expectedResponse.Specialities, actualResponse.Data.Specialities);
        }

        [Fact]
        public async Task GetAllAsync_WhenPageNumIsLessThanOne_ShouldThrowException()
        {
            //Act
            var action = async () => await mentorsController.GetAllAsync(invalidPageNum, validPageSize);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetAllAsync_WhenPageSizeIsLessThanOne_ShouldThrowException()
        {
            //Act
            var action = async () => await mentorsController.GetAllAsync(validPageNum, invalidPageSize);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetAllAsync_WhenCampaignIdNullAndNotEmpty_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var expectedResponse1 = new MentorSummaryResponse(id, firstName, lastName, email, specialitySummaries);
            var expectedResponse2 = new MentorSummaryResponse(
                Guid.NewGuid(), "John", "Smith", "john.smith@gmail.com",
                specialitySummaries);

            var expectedResponseList = new List<MentorSummaryResponse>() {
                expectedResponse1, expectedResponse2
            };

            var expectedPaginationResponse = new PaginationResponse<MentorSummaryResponse>(expectedResponseList, 1, 2);

            mentorsServiceMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>(), null))
                .ReturnsAsync(expectedPaginationResponse);

            //Act
            var actionResult = await mentorsController.GetAllAsync(validPageNum, validPageSize);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var paginationResponse = jsonResult!.Value as CoreResponse<PaginationResponse<MentorSummaryResponse>>;

            Assert.Equal(expectedResponseList.Count, paginationResponse.Data.Content.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenCampaignIdNullAndEmpty_ShouldReturnEmptyCollection()
        {
            //Arrange
            var emptyList = new List<MentorSummaryResponse>();

            mentorsServiceMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>(), null))
                .ReturnsAsync(new PaginationResponse<MentorSummaryResponse>(emptyList, 1, 1));

            //Act
            var actionResult = await mentorsController.GetAllAsync(validPageNum, validPageSize);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var okObjectResult = actionResult as JsonResult;

            Assert.NotNull(okObjectResult);

            var paginationResponse = okObjectResult!.Value as CoreResponse<PaginationResponse<MentorSummaryResponse>>;

            Assert.Empty(paginationResponse.Data.Content);
        }
    }
}
