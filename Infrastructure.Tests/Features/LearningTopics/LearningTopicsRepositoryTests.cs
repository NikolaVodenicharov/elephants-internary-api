using Core.Features.LearningTopics.Interfaces;
using Core.Features.LearningTopics.Entities;
using Core.Features.Specialties.Entities;
using Core.Features.Specialities.Interfaces;
using Infrastructure.Features.Specialities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Features.LearningTopics
{
    public class LearningTopicsRepositoryTests
    {
        private readonly InternaryContext context;
        private readonly ILearningTopicsRepository learningTopicsRepository;
        private readonly ISpecialitiesRepository specialitiesRepository;

        private Guid id = Guid.NewGuid();
        private string name = "Test Learning Topic";
        private string updatedName = "Test Learning Topic Updated";
        private List<Speciality> specialities;
        private Speciality speciality;
        private Speciality additionalSpeciality;
        private LearningTopic  learningTopic;
        private LearningTopic  additionalLearningTopic;

        public LearningTopicsRepositoryTests()
        {
            DbContextOptionsBuilder<InternaryContext>? dbContextOptions = new DbContextOptionsBuilder<InternaryContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString()
                );
            
            context = new InternaryContext(dbContextOptions.Options);

            learningTopicsRepository = new LearningTopicsRepository(context);
            
            specialitiesRepository = new SpecialitiesRepository(context);


            speciality = new Speciality() 
            { 
                Id = Guid.NewGuid(), 
                Name = "Backend"
            };

            additionalSpeciality =  new Speciality() 
            { 
                Id = Guid.NewGuid(), 
                Name = "Frontend"
            };

            specialities = new List<Speciality>() { speciality };

            learningTopic = new LearningTopic()
            {
                Name = name,
                Specialities = specialities
            };

            additionalLearningTopic = new LearningTopic() 
            { 
                Name = updatedName, 
                Specialities = specialities 
            };
        }

        [Fact]
        public async Task CreateAsync_AddLearningTopic_ShouldBeAddedToDatabase()
        {
            // Arrange
            var expectedLearningTopicsCount = 1;
            var expectedSpecialitiesCount = specialities.Count();

            // Act
            var learningTopicResult = await learningTopicsRepository.AddAsync(learningTopic);

            // Assert
            Assert.Equal(expectedLearningTopicsCount, await context.LearningTopics.CountAsync());
            Assert.NotEmpty(learningTopicResult.Specialities);
            Assert.Equal(expectedSpecialitiesCount, learningTopicResult.Specialities.Count());
            Assert.Equal(name, learningTopicResult.Name);
        }

        [Fact]
        public async Task UpdateAsync_UpdateLearningTopicName_ShouldBeUpdatedInDatabase()
        {
            // Arrange
            var expectedLearningTopicsCount = 1;
            var expectedSpecialitiesCount  = specialities.Count();

            await learningTopicsRepository.AddAsync(learningTopic);
            
            learningTopic.Name = updatedName;
            
            // Act
            var learningTopicResult = await learningTopicsRepository.UpdateAsync(learningTopic);

            // Assert
            Assert.Equal(expectedLearningTopicsCount, await context.LearningTopics.CountAsync());
            Assert.Equal(expectedSpecialitiesCount, learningTopicResult.Specialities.Count());
            Assert.Equal(updatedName, learningTopicResult.Name);
        }

        [Fact]
        public async Task UpdateAsync_UpdateLearningTopicAddSpecialities_ShouldBeUpdatedInDatabase()
        {
            // Arrange
            await specialitiesRepository.AddAsync(speciality);
            await specialitiesRepository.AddAsync(additionalSpeciality);
            
            await learningTopicsRepository.AddAsync(learningTopic);
            
            learningTopic.Specialities.Add(additionalSpeciality);
            
            var expectedLearningTopicsCount = 1;
            var expectedSpecialitiesCount = learningTopic.Specialities.Count();
            
            // Act
            var learningTopicResult = await learningTopicsRepository.UpdateAsync(learningTopic);

            // Assert
            Assert.Equal(expectedLearningTopicsCount, await context.LearningTopics.CountAsync());
            Assert.Equal(expectedSpecialitiesCount, learningTopicResult.Specialities.Count());
            Assert.Equal(name, learningTopicResult.Name);
        }

        [Fact]
        public async Task UpdateAsync_UpdateLearningTopicRemoveSpecialities_ShouldBeUpdatedInDatabase()
        {
            // Arrange
            await specialitiesRepository.AddAsync(speciality);
            await specialitiesRepository.AddAsync(additionalSpeciality);
            
            learningTopic.Specialities.Add(additionalSpeciality);

            await learningTopicsRepository.AddAsync(learningTopic);
            
            learningTopic.Specialities.Remove(additionalSpeciality);
            
            var expectedLearningTopicsCount = 1;
            var expectedSpecialitiesCount = learningTopic.Specialities.Count();

            // Act
            var learningTopicResult = await learningTopicsRepository.UpdateAsync(learningTopic);

            // Assert
            Assert.Equal(expectedLearningTopicsCount, await context.LearningTopics.CountAsync());
            Assert.Equal(expectedSpecialitiesCount, learningTopicResult.Specialities.Count());
            Assert.Equal(name, learningTopicResult.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdExists_ShouldReturnCorrectObject()
        {
            // Arrange
            var newLearningTopic = await learningTopicsRepository.AddAsync(learningTopic);
            
            await learningTopicsRepository.AddAsync(additionalLearningTopic);

            // Act
            var learningTopicResult = await learningTopicsRepository.GetByIdAsync(newLearningTopic.Id);
            
            // Assert
            Assert.NotNull(learningTopicResult);
            Assert.Equal(newLearningTopic.Id, learningTopicResult.Id);
            Assert.Equal(newLearningTopic.Name, learningTopicResult.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnNull()
        {
            // Act
            var learningTopicResult = await learningTopicsRepository.GetByIdAsync(id);

            // Assert
            Assert.Null(learningTopicResult);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectCount()
        {
            // Arrange
            var expectedLearningTopicsCount = 2;

            await learningTopicsRepository.AddAsync(learningTopic);
            await learningTopicsRepository.AddAsync(additionalLearningTopic);

            // Act
            var learningTopicsResult = await learningTopicsRepository.GetAllAsync();

            // Assert
            Assert.Equal(expectedLearningTopicsCount, learningTopicsResult.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            // Act
            var learningTopicsResult = await learningTopicsRepository.GetAllAsync();
            
            // Assert
            Assert.Empty(learningTopicsResult);
        }

        [Fact]
        public async Task ExistsByNameAsync_WhenNameExists_ShouldReturnTrue()
        {
            // Arrange
            var learningTopicResult = await learningTopicsRepository.AddAsync(learningTopic);

            // Act
            var existsByName = await learningTopicsRepository.ExistsByNameAsync(name);
            
            // Assert
            Assert.True(existsByName);
        }

        [Fact]
        public async Task ExistsByNameAsync_WhenNameDoesNotExist_ShouldReturnFalse()
        {
            // Act
            var existsByName = await learningTopicsRepository.ExistsByNameAsync(name);
            
            // Assert
            Assert.False(existsByName);
        }
    }

}