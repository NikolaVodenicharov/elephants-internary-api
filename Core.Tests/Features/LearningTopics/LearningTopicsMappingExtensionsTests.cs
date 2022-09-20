using Core.Features.LearningTopics.Entities;
using Core.Features.LearningTopics.RequestModels;
using Core.Features.LearningTopics.Support;
using Core.Features.Specialties.Entities;
using Core.Features.Specialities.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Core.Tests.Features.LearningTopics
{
    public class LearningTopicsMappingExtensionsTests
    {
        private Guid learningTopicId = Guid.NewGuid();
        private string learningTopicName = "Test Learning Topic";
        private List<SpecialitySummaryResponse> specialitySummaries;
        private List<Speciality> specialities;
        private List<Guid> specialityIds;

        public LearningTopicsMappingExtensionsTests()
        {
            var speciality = new Speciality
            {
                Id = Guid.NewGuid(),
                Name = "Backend"
            };
            
            var specialitySummary = new SpecialitySummaryResponse(speciality.Id, speciality.Name);

            specialities = new List<Speciality>() { speciality };
            specialityIds = new List<Guid>() {speciality.Id};
            specialitySummaries = new List<SpecialitySummaryResponse>() { specialitySummary };
        }

        [Fact]
        public void CreateLearningTopicRequest_ToLearningTopic_CreateCorrectObject()
        {
            // Arrange
            var learningTopicRequest = new CreateLearningTopicRequest(learningTopicName, specialityIds);

            // Act
            var learningTopic = learningTopicRequest.ToLearningTopic();

            // Assert
            Assert.Equal(learningTopicName, learningTopic.Name);
        }

        [Fact]
        public void LearningTopic_ToLearningTopicSummary_CreateCorrectObject()
        {
            // Arrange
            var learningTopic = new LearningTopic
            {
                Id = learningTopicId,
                Name = learningTopicName,
                Specialities = specialities
            };

            // Act
            var learningTopicSummary = learningTopic.ToLearningTopicSummary();

            // Assert
            Assert.Equal(learningTopicId, learningTopicSummary.Id);
            Assert.Equal(learningTopicName, learningTopicSummary.Name);
            Assert.Equal(specialitySummaries, learningTopicSummary.Specialities);
        }

        [Fact]
        public void IEnumerableOfLearningTopics_ToLearningTopicSummaries_CreateCorrectObject()
        {
            // Arrange
            var learningTopics = new List<LearningTopic>() 
            {
                new LearningTopic
                {
                    Id = learningTopicId,
                    Name = learningTopicName,
                    Specialities = specialities
                }
            };

            // Act
            var learningTopicSummaries = learningTopics.ToLearningTopicSummaries().ToList();

            // Assert
            Assert.Equal(learningTopics.Count, learningTopicSummaries.Count);
            Assert.Equal(learningTopics[0].Id, learningTopicSummaries[0].Id);
            Assert.Equal(learningTopics[0].Name, learningTopicSummaries[0].Name);
            Assert.Equal(learningTopics[0].Specialities.Count, learningTopicSummaries[0].Specialities.Count);
        }

    }
}