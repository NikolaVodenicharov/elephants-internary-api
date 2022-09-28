using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Specialities;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.RequestModels;
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

namespace Core.Tests.Features.Specialities
{
    public class SpecialitiesServiceTests
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly string name = "SpecialityTestName";

        private readonly Mock<ISpecialitiesRepository> specialtiesRepositoryMock;
        private readonly SpecialitiesService specialtiesService;

        private readonly IValidator<CreateSpecialityRequest> createSpecialityValidator;
        private readonly IValidator<UpdateSpecialityRequest> updateSpecialityValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;

        public SpecialitiesServiceTests()
        {
            specialtiesRepositoryMock = new Mock<ISpecialitiesRepository>();
            createSpecialityValidator = new CreateSpecialityValidator();
            updateSpecialityValidator = new UpdateSpecialityValidator();
            paginationRequestValidator = new PaginationRequestValidator();


            var loggerMock = new Mock<ILogger<SpecialitiesService>>();

            specialtiesService = new SpecialitiesService(specialtiesRepositoryMock.Object, 
                loggerMock.Object, 
                createSpecialityValidator, 
                updateSpecialityValidator,
                paginationRequestValidator);
        }

        #region CreateAsyncTests

        [Fact]
        public async Task CreateAsync_WhenNameIsDuplicated_ShouldThrowException()
        {
            //Arrange
            specialtiesRepositoryMock
                .Setup(x => x.ExistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var createSpeciality = new CreateSpecialityRequest(name);

            //Act
            var action = async () => await specialtiesService.CreateAsync(createSpeciality);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
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

            specialtiesRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Speciality>()))
                .ReturnsAsync(specialitySummaryMock);

            //Act
            var specialitySummary = await specialtiesService.CreateAsync(createSpeciality);

            //Assert
            Assert.Equal(specialitySummaryMock.Id, specialitySummary.Id);
            Assert.Equal(specialitySummaryMock.Name, specialitySummary.Name);
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
            var action = async () => await specialtiesService.CreateAsync(createSpeciality);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenNameIsContainsDigit_ShouldThrowException()
        {
            //Arrange
            var createSpeciality = new CreateSpecialityRequest(NameEdgeCaseTestHelper.NameWithDigit);

            //Act
            var action = async () => await specialtiesService.CreateAsync(createSpeciality);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        #endregion

        #region UpdateAsyncTest

        [Fact]
        public async Task UpdateAsync_WhenIdDoesNotExist_ShouldThrowException()
        {
            //Arrange
            var updateSpecialityRequest = new UpdateSpecialityRequest(id, name);

            //Act
            var action = async () => await specialtiesService.UpdateAsync(updateSpecialityRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenNameIsDuplicated_ShouldThrowException()
        {
            //Arrange
            specialtiesRepositoryMock
                .Setup(x => x.IsNameTakenByOtherAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var updateSpeciality = new UpdateSpecialityRequest(id, name);

            //Act
            var action = async () => await specialtiesService.UpdateAsync(updateSpeciality);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Theory]
        [InlineData(SpecialityValidationConstants.NameMinLength)]
        [InlineData(SpecialityValidationConstants.NameMaxLength)]
        public async Task UpdateAsync_WhenNameIsInAllowLength_ShouldReturnCorrectData(int nameLength)
        {
            //Arrange
            var nameInAllowLength = TestHelper.GenerateString(nameLength);

            var specialityMock = new Speciality()
            {
                Id = Guid.NewGuid(),
                Name = nameInAllowLength
            };

            specialtiesRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(specialityMock);

            var updateSpecialityRequest = new UpdateSpecialityRequest(specialityMock.Id, specialityMock.Name);

            //Act
            var specialitySummaryResponse = await specialtiesService.UpdateAsync(updateSpecialityRequest);

            //Assert
            Assert.Equal(updateSpecialityRequest.Id, specialitySummaryResponse.Id);
            Assert.Equal(updateSpecialityRequest.Name, specialitySummaryResponse.Name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(SpecialityValidationConstants.NameMinLength - 1)]
        [InlineData(SpecialityValidationConstants.NameMaxLength + 1)]
        public async Task UpdateAsync_WhenNameLengthIsOutOfRange_ShouldThrowException(int nameLength)
        {
            //Arrange
            var nameLengthOutOfRange = TestHelper.GenerateString(nameLength);

            var createSpeciality = new UpdateSpecialityRequest(id, nameLengthOutOfRange);

            //Act
            var action = async () => await specialtiesService.UpdateAsync(createSpeciality);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        #endregion

        #region GetByIdAsyncTests

        [Fact]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldThrowException()
        {
            //Act
            var action = async () => await specialtiesService.GetByIdAsync(Guid.NewGuid());

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdExist_ShouldReturnCorrectObject()
        {
            //Arrange
            var specialitySummaryMock = new Speciality() { Id = id, Name = name };

            specialtiesRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(specialitySummaryMock);

            //Act
            var specialitySummaryActual = await specialtiesService.GetByIdAsync(id);

            //Assert
            Assert.Equal(specialitySummaryMock.Id, specialitySummaryActual.Id);
            Assert.Equal(specialitySummaryMock.Name, specialitySummaryActual.Name);
        }

        #endregion

        #region GetAllAsyncTests

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            //Act
            var specialties = await specialtiesService.GetAllAsync();

            //Assert
            Assert.Empty(specialties);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var specialitySummary1 = new SpecialitySummaryResponse(id, name);

            var specialitySummary2 = new SpecialitySummaryResponse(Guid.NewGuid(), "New");

            var specialityList = new List<SpecialitySummaryResponse>() { specialitySummary1, specialitySummary2 };

            specialtiesRepositoryMock
                .Setup(s => s.GetCountAsync())
                .ReturnsAsync(specialityList.Count);

            specialtiesRepositoryMock
                .Setup(s => s.GetAllAsync(null))
                .ReturnsAsync(specialityList);

            //Act
            var specialties = await specialtiesService.GetAllAsync();

            //Assert
            Assert.Equal(2, specialties.Count());
        }

        #endregion

        #region GetPaginationAsyncTests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetPaginationAsync_WhenPageNumIsLessThanOne_ShouldThrowException(int pageNum)
        {
            //Arrange
            var filter = new PaginationRequest(pageNum, 5);

            //Act
            var action = async () => await specialtiesService.GetPaginationAsync(filter);

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
            var action = async () => await specialtiesService.GetPaginationAsync(filter);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetPaginationAsync_WhenFilterIsCorrect_ShouldReturnCorrectCountElements()
        {
            //Assert
            var filter = new PaginationRequest(1, 10);

            var specialitySummary1 = new SpecialitySummaryResponse(id, name);

            var specialityList = new List<SpecialitySummaryResponse>() { specialitySummary1 };

            specialtiesRepositoryMock
                .Setup(s => s.GetCountAsync())
                .ReturnsAsync(specialityList.Count);

            specialtiesRepositoryMock
                .Setup(s => s.GetAllAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(specialityList);

            //Act
            var response = await specialtiesService.GetPaginationAsync(filter);

            //Assert
            Assert.Equal(specialityList.Count, response.Content.Count());
        }

        #endregion
    }
}
