using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Mentors.Support;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.RequestModels;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialities.Support;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Features.Specialities;
using Xunit;

namespace WebAPI.Tests.Features.Specialities
{
    public class SpecialtiesControllerTests
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly Mock<ISpecialitiesService> specialtiesServiceMock;
        private readonly SpecialitiesController specialtiesController;
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

        public SpecialtiesControllerTests()
        {
            var createSpecialityValidator = new CreateSpecialityValidator();
            var updateSpecialityValidator = new UpdateSpecialityValidator();
            var paginationRequestValidator = new PaginationRequestValidator();

            var specialityValidator = new SpecialityValidator(createSpecialityValidator, updateSpecialityValidator);

            specialtiesServiceMock = new Mock<ISpecialitiesService>();

            var loggerMock = new Mock<ILogger<SpecialitiesController>>();

            specialtiesController = new(
                specialtiesServiceMock.Object,
                loggerMock.Object,
                specialityValidator,
                paginationRequestValidator);

            validPaginationRequest = new PaginationRequest(1, 10);
            emptyPaginationRequest = new PaginationRequest(null, null);
        }

        [Theory]
        [InlineData(SpecialityValidationConstants.NameMinLength)]
        [InlineData(SpecialityValidationConstants.NameMaxLength)]
        public async Task CreateAsync_WhenNameIsInAllowLength_ShouldReturnCorrectData(int nameLength)
        {
            //Arrange
            var nameInAllowLength = TestHelper.GenerateString(nameLength);

            var createSpeciality = new CreateSpecialityRequest(nameInAllowLength);

            var specialitySummaryMock = new SpecialitySummaryResponse(id, nameInAllowLength);

            specialtiesServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateSpecialityRequest>()))
                .ReturnsAsync(specialitySummaryMock);

            //Act
            var actionResult = await specialtiesController.CreateAsync(createSpeciality);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var specialitySummaryResult = jsonResult!.Value as CoreResponse<SpecialitySummaryResponse>;

            Assert.Equal(specialitySummaryMock.Id, specialitySummaryResult!.Data!.Id);
            Assert.Equal(specialitySummaryMock.Name, specialitySummaryResult.Data.Name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(SpecialityValidationConstants.NameMinLength - 1)]
        [InlineData(SpecialityValidationConstants.NameMaxLength + 1)]
        public async Task CreateAsync_WhenNameLengthIsOutOfRange_ShouldThrowException(int nameLength)
        {
            //Arrange
            var nameLengthOutOfRange = TestHelper.GenerateString(nameLength);

            var createSpeciality = new CreateSpecialityRequest(nameLengthOutOfRange);

            //Act
            var action = async () => await specialtiesController.CreateAsync(createSpeciality);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [InlineData(SpecialityValidationConstants.NameMinLength)]
        [InlineData(SpecialityValidationConstants.NameMaxLength)]
        public async Task UpdateAsync_WhenNameIsInAllowLength_ShouldReturnCorrectData(int nameLength)
        {
            //Arrange
            var nameInAllowLength = TestHelper.GenerateString(nameLength);

            var updateSpeciality = new UpdateSpecialityRequest(id, nameInAllowLength);

            var specialitySummaryMock = new SpecialitySummaryResponse(id, nameInAllowLength);

            specialtiesServiceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateSpecialityRequest>()))
                .ReturnsAsync(specialitySummaryMock);

            //Act
            var actionResult = await specialtiesController.UpdateAsync(id, updateSpeciality);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var specialitySummaryResult = jsonResult!.Value as CoreResponse<SpecialitySummaryResponse>;

            Assert.Equal(specialitySummaryMock.Id, specialitySummaryResult!.Data!.Id);
            Assert.Equal(specialitySummaryMock.Name, specialitySummaryResult.Data.Name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(SpecialityValidationConstants.NameMinLength - 1)]
        [InlineData(SpecialityValidationConstants.NameMaxLength + 1)]
        public async Task UpdateAsync_WhenNameLengthIsOutOfRange_ShouldThrowException(int nameLength)
        {
            //Arrange
            var nameLengthOutOfRange = TestHelper.GenerateString(nameLength);

            var updateSpeciality = new UpdateSpecialityRequest(id, nameLengthOutOfRange);

            //Act
            var action = async () => await specialtiesController.UpdateAsync(id, updateSpeciality);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenQueryIdAndModelIdDoesNotMatch_SholdThrowException()
        {
            //Arrange
            var nameInAllowLength = TestHelper.GenerateString(SpecialityValidationConstants.NameMinLength);

            var updateSpeciality = new UpdateSpecialityRequest(id, nameInAllowLength);

            //Act
            var action = async () => await specialtiesController.UpdateAsync(Guid.NewGuid(), updateSpeciality);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdExist_ShouldReturnCorrectObject()
        {
            //Arrange
            var nameInAllowLength = TestHelper.GenerateString(SpecialityValidationConstants.NameMinLength);

            var specialitySummaryMock = new SpecialitySummaryResponse(id, nameInAllowLength);

            specialtiesServiceMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(specialitySummaryMock);

            //Act
            var actionResult = await specialtiesController.GetByIdAsync(id);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var specialitySummaryResult = jsonResult!.Value as CoreResponse<SpecialitySummaryResponse>;

            Assert.Equal(specialitySummaryMock.Id, specialitySummaryResult!.Data!.Id);
            Assert.Equal(specialitySummaryMock.Name, specialitySummaryResult.Data.Name);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmptyAndNoPageParametersSet_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var nameInAllowLength = TestHelper.GenerateString(SpecialityValidationConstants.NameMinLength);

            var specialitySummaryMock1 = new SpecialitySummaryResponse(id, nameInAllowLength);
            var specialitySummaryMock2 = new SpecialitySummaryResponse(Guid.NewGuid(), nameInAllowLength);

            var specialitySummaryListMock = new List<SpecialitySummaryResponse>() { specialitySummaryMock1, specialitySummaryMock2 };

            specialtiesServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(specialitySummaryListMock);

            //Act
            var actionResult = await specialtiesController.GetAllAsync(emptyPaginationRequest);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var specialitySummaryList = jsonResult!.Value as CoreResponse<IEnumerable<SpecialitySummaryResponse>>;

            Assert.Equal(2, specialitySummaryList!.Data!.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenEmptyAndNoPageParametersSet_ShouldReturEmptyCollection()
        {
            //Arrange
            specialtiesServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<SpecialitySummaryResponse>());

            //Act
            var actionResult = await specialtiesController.GetAllAsync(emptyPaginationRequest);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var specialitySummaryList = jsonResult!.Value as CoreResponse<IEnumerable<SpecialitySummaryResponse>>;

            Assert.Empty(specialitySummaryList!.Data);
        }

        [Theory]
        [MemberData(nameof(InvalidPaginationRequests))]
        public async Task GetAllAsync_WhenPageParametersAreInvalid_ShouldThrowException(PaginationRequest paginationRequest)
        {
            //Act
            var action = async () => await specialtiesController.GetAllAsync(paginationRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmptyAndBothParametersAreValid_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var summary1 = new SpecialitySummaryResponse(Guid.NewGuid(), "Backend");
            var summary2 = new SpecialitySummaryResponse(Guid.NewGuid(), "Frontend");

            var specialityList = new List<SpecialitySummaryResponse>() { summary1, summary2 };

            var expectedResponse = new PaginationResponse<SpecialitySummaryResponse>(specialityList, 
                validPaginationRequest!.PageNum!.Value, 1);

            specialtiesServiceMock
                .Setup(x => x.GetPaginationAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(expectedResponse);

            //Act
            var actionResult = await specialtiesController.GetAllAsync(validPaginationRequest);

            //Assert
            Assert.IsType<JsonResult>(actionResult);

            var jsonResult = actionResult as JsonResult;

            Assert.NotNull(jsonResult);

            var actualResponse = jsonResult!.Value as CoreResponse<PaginationResponse<SpecialitySummaryResponse>>;

            Assert.Equal(expectedResponse.Content.Count(), actualResponse!.Data!.Content.Count());
        }
    }
}
