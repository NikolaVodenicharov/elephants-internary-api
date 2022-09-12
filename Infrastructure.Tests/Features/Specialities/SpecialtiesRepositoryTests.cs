﻿using Core.Features.Specialities.Interfaces;
using Core.Features.Specialties.Entities;
using Infrastructure.Features.Specialities;
using Microsoft.EntityFrameworkCore;
using System;
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
                    Guid.NewGuid().ToString());

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
        public async Task Update_UpdateSpecialty_ShouldSaveNewDataToDatabase()
        {
            //Arrange
            var specialtyMock = new Speciality() { Name = name };

            await specialtiesRepository.AddAsync(specialtyMock);

            specialtyMock.Name = "Updated";

            //Act
            var speciialtySummaryActual = await specialtiesRepository.UpdateAync(specialtyMock);

            //Assert
            Assert.Equal(specialtyMock.Id, speciialtySummaryActual.Id);
            Assert.Equal(specialtyMock.Name, speciialtySummaryActual.Name);
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
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            //Act
            var specialties = await specialtiesRepository.GetAllAsync();

            //Assert
            Assert.Empty(specialties);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectCountElements()
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
            Assert.Equal(specialtyMock1.Id, specialtySummary.Id);
            Assert.Equal(specialtyMock1.Name, specialtySummary.Name);
        }
    }
}