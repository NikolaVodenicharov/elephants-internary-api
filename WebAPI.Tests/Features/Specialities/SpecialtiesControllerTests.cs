using Core.Common.Exceptions;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.RequestModels;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialities.Support;
using Core.Tests;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Features.Specialities;
using Xunit;

namespace WebAPI.Tests.Features.Specialities
{
    public class SpecialtiesControllerTests
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly Mock<ISpecialitiesService> specialtiesServiceMock;
        private readonly SpecialitiesController specialtiesController;

        public SpecialtiesControllerTests()
        {
            var createSpecialityValidator = new CreateSpecialityValidator();
            var updateSpecialityValidator = new UpdateSpecialityValidator();
            specialtiesServiceMock = new Mock<ISpecialitiesService>();

            var loggerMock = new Mock<ILogger<SpecialitiesController>>();

            specialtiesController = new(
                specialtiesServiceMock.Object,
                loggerMock.Object,
                createSpecialityValidator,
                updateSpecialityValidator);
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
            Assert.IsType<OkObjectResult>(actionResult);

            var okObjectResult = actionResult as OkObjectResult;

            Assert.NotNull(okObjectResult);

            var specialitySummaryResult = okObjectResult!.Value as SpecialitySummaryResponse;

            Assert.Equal(specialitySummaryMock.Id, specialitySummaryResult!.Id);
            Assert.Equal(specialitySummaryMock.Name, specialitySummaryResult.Name);
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
            Assert.IsType<OkObjectResult>(actionResult);

            var okObjectResult = actionResult as OkObjectResult;

            Assert.NotNull(okObjectResult);

            var specialitySummaryResult = okObjectResult!.Value as SpecialitySummaryResponse;

            Assert.Equal(specialitySummaryMock.Id, specialitySummaryResult!.Id);
            Assert.Equal(specialitySummaryMock.Name, specialitySummaryResult.Name);
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
            Assert.IsType<OkObjectResult>(actionResult);

            var okObjectResult = actionResult as OkObjectResult;

            Assert.NotNull(okObjectResult);

            var specialitySummaryResult = okObjectResult!.Value as SpecialitySummaryResponse;

            Assert.Equal(specialitySummaryMock.Id, specialitySummaryResult!.Id);
            Assert.Equal(specialitySummaryMock.Name, specialitySummaryResult.Name);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectCountElements()
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
            var actionResult = await specialtiesController.GetAllAsync();

            //Assert
            Assert.IsType<OkObjectResult>(actionResult);

            var okObjectResult = actionResult as OkObjectResult;

            Assert.NotNull(okObjectResult);

            var specialitySummaryList = okObjectResult!.Value as List<SpecialitySummaryResponse>;

            Assert.Equal(2, specialitySummaryList!.Count);
        }

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturEmptyCollection()
        {
            //Arrange
            specialtiesServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<SpecialitySummaryResponse>());

            //Act
            var actionResult = await specialtiesController.GetAllAsync();

            //Assert
            Assert.IsType<OkObjectResult>(actionResult);

            var okObjectResult = actionResult as OkObjectResult;

            Assert.NotNull(okObjectResult);

            var specialitySummaryList = okObjectResult!.Value as List<SpecialitySummaryResponse>;

            Assert.Empty(specialitySummaryList);
        }
    }
}
