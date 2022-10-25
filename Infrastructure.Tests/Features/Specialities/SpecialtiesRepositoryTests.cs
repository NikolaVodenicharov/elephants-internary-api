using Core.Common.Pagination;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialties.Entities;
using Infrastructure.Features.Specialities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Features.Specialities
{
    public class SpecialtiesRepositoryTests
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly string name = "SpecialtyTestName";
        private readonly InternaryContext context;
        private readonly ISpecialitiesRepository specialtiesRepository;

        public SpecialtiesRepositoryTests()
        {
            DbContextOptionsBuilder<InternaryContext>? dbOptions = new DbContextOptionsBuilder<InternaryContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            context = new InternaryContext(dbOptions.Options);

            specialtiesRepository = new SpecialitiesRepository(context);
        }

        [Fact]
        public async Task AddAsync_AddingOneSpecialty_ShouldBeAddedToDatabase()
        {
            //Arrange
            var specialtyMock = new Speciality() { Name = name };

            //Act
            var specialtyActual = await specialtiesRepository.AddAsync(specialtyMock);

            //Assert
            Assert.Equal(1, await context.Specialties.CountAsync());
            Assert.Equal(specialtyMock.Name, specialtyActual.Name);
        }

        [Fact]
        public async Task AddAsync_AddingMultipleSpecialties_ShouldAddAllToDatabase()
        {
            //Arrange
            var specialtyMock1 = new Speciality() { Name = name };
            var specialtyMock2 = new Speciality() { Name = name + "2" };

            //Act
            var specialtyActual1 = await specialtiesRepository.AddAsync(specialtyMock1);
            var specialtyActual2 = await specialtiesRepository.AddAsync(specialtyMock2);

            //Assert
            Assert.Equal(2, await context.Specialties.CountAsync());
            Assert.Equal(specialtyMock1.Name, specialtyActual1.Name);
            Assert.Equal(specialtyMock2.Name, specialtyActual2.Name);
        }

        [Fact]
        public async Task SaveTrackingChangesAsync_UpdateSpecialty_ShouldSaveNewDataToDatabase()
        {
            //Arrange
            var specialtyMock = new Speciality() { Name = name };

            await specialtiesRepository.AddAsync(specialtyMock);

            specialtyMock.Name = "Updated";

            //Act
            await specialtiesRepository.SaveTrackingChangesAsync();

            var updatedSpecialty = await specialtiesRepository.GetByIdAsync(specialtyMock.Id);

            //Assert
            Assert.NotNull(updatedSpecialty);
            Assert.Equal(specialtyMock.Id, updatedSpecialty!.Id);
            Assert.Equal(specialtyMock.Name, updatedSpecialty.Name);
        }

        [Fact]
        public async Task ExistAsync_WhenNameExist_ShouldReturnTrue()
        {
            //Arrange
            var specialtyMock = new Speciality() { Name = name };

            var specialtyActual = await specialtiesRepository.AddAsync(specialtyMock);

            //Act
            var exist = await specialtiesRepository.ExistsByNameAsync(specialtyActual.Name);

            //Assert
            Assert.True(exist);
        }

        [Fact]
        public async Task ExistAsync_WhenNameDoesNotExist_ShouldReturnFalse()
        {
            //Act
            var exist = await specialtiesRepository.ExistsByNameAsync(name);

            //Assert
            Assert.False(exist);
        }

        [Fact]
        public async Task IsNameTakenByOtherAsync_WhenNameIsNotTakenByOther_ShouldReturnFalse()
        {
            //Arrange
            var specialtyMock = new Speciality() { Name = name };

            var specialtyActual = await specialtiesRepository.AddAsync(specialtyMock);

            //Act
            var isTaken = await specialtiesRepository.IsNameTakenByOtherAsync(specialtyActual.Name + "Updated", specialtyActual.Id);

            //Assert
            Assert.False(isTaken);
        }

        [Fact]
        public async Task IsNameTakenByOtherAsync_WhenNameIsTakenByOther_ShouldReturnTrue()
        {
            //Arrange
            var specialtyMock1 = new Speciality() { Name = name };
            var specialtyMock2 = new Speciality() { Name = name + "2" };

            var specialtyActual1 = await specialtiesRepository.AddAsync(specialtyMock1);
            var specialtyActual2 = await specialtiesRepository.AddAsync(specialtyMock2);

            //Act
            var isTaken = await specialtiesRepository.IsNameTakenByOtherAsync(specialtyActual2.Name, specialtyActual1.Id);

            //Assert
            Assert.True(isTaken);
        }

        [Fact]
        public async Task GetAllAsync_WhenFilterNullAndEmpty_ShouldReturnEmptyCollection()
        {
            //Act
            var specialties = await specialtiesRepository.GetAllAsync();

            //Assert
            Assert.Empty(specialties);
        }

        [Fact]
        public async Task GetAllAsync_WhenFilterNullAndNotEmpty_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var specialtyMock1 = new Speciality() { Name = name };
            var specialtyMock2 = new Speciality() { Name = name + "2" };

            await specialtiesRepository.AddAsync(specialtyMock1);
            await specialtiesRepository.AddAsync(specialtyMock2);

            //Act
            var specialties = await specialtiesRepository.GetAllAsync();

            //Assert
            Assert.Equal(2, specialties.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenFilterNotNullAndEmpty_ShouldReturnEmptyCollection()
        {
            //Arrange
            var filter = new PaginationRequest(1, 10);

            //Act
            var specialties = await specialtiesRepository.GetAllAsync(filter);

            //Assert
            Assert.Empty(specialties);
        }

        [Fact]
        public async Task GetAllAsync_WhenFilterNotNullAndNotEmpty_ShouldReturnCorrectCountElements()
        {
            //Arrange
            var filter = new PaginationRequest(1, 10);

            var specialtyMock1 = new Speciality() { Name = name };

            await specialtiesRepository.AddAsync(specialtyMock1);

            //Act
            var specialties = await specialtiesRepository.GetAllAsync(filter);

            //Assert
            Assert.Single(specialties);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnNull()
        {
            //Arrange
            var specialty = await specialtiesRepository.GetByIdAsync(id);

            //Act-Assert
            Assert.Null(specialty);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdExist_ShouldReturnCorrectObject()
        {
            //Arrange
            var specialtyMock1 = new Speciality() { Name = name };
            var specialtyMock2 = new Speciality() { Name = name + "2" };

            await specialtiesRepository.AddAsync(specialtyMock1);
            await specialtiesRepository.AddAsync(specialtyMock2);

            //Act
            var specialtySummary = await specialtiesRepository.GetByIdAsync(specialtyMock1.Id);

            //Assert
            Assert.Equal(specialtyMock1.Id, specialtySummary!.Id);
            Assert.Equal(specialtyMock1.Name, specialtySummary.Name);
        }

        [Fact]
        public async Task GetByIdsAsync_WhenAllIdsFound_ShouldReturnCorrectCount()
        {
            //Arrange
            var specialtyMock1 = new Speciality() { Name = name };
            var specialtyMock2 = new Speciality() { Name = name + "2" };

            var createdSpeciality1 = await specialtiesRepository.AddAsync(specialtyMock1);
            var createdSpeciality2 = await specialtiesRepository.AddAsync(specialtyMock2);

            var specialityIds = new List<Guid>()
            {
                createdSpeciality1.Id, createdSpeciality2.Id
            };

            //Act
            var response = await specialtiesRepository.GetByIdsAsync(specialityIds);

            //Assert
            Assert.Equal(specialityIds.Count, response.Count);
        }

        [Fact]
        public async Task GetByIdsAsync_WhenNotAllIdsFound_ShouldReturnDifferentCount()
        {
            //Arrange
            var specialtyMock1 = new Speciality() { Name = name };

            var createdSpeciality1 = await specialtiesRepository.AddAsync(specialtyMock1);

            var specialityIds = new List<Guid>()
            {
                createdSpeciality1.Id, Guid.NewGuid(), Guid.NewGuid()
            };

            //Act
            var response = await specialtiesRepository.GetByIdsAsync(specialityIds);

            //Assert
            Assert.NotEqual(specialityIds.Count, response.Count);
        }
    }
}
