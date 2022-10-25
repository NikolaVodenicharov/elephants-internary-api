using Core.Features.LearningTopics.RequestModels;
using Core.Features.LearningTopics.Support;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using Xunit;

namespace Core.Tests.Features.LearningTopics
{
    public class CreateLearningTopicRequestValidatorTests
    {
        private readonly CreateLearningTopicRequestValidator createLearningTopicValidator = new();
        private readonly string learningTopicName = "Test Learning Topic";
        private readonly List<Guid> specialityIds = new List<Guid>() { Guid.NewGuid() };

        [Theory]
        [MemberData(nameof(MockDataTestHelper.LearningTopicValidNames), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenNameIsValid_ShouldNotHaveError(string name)
        {
            var createLearningTopicRequest = new CreateLearningTopicRequest(name, specialityIds);

            createLearningTopicValidator
                .TestValidate(createLearningTopicRequest)
                .ShouldNotHaveValidationErrorFor(t => t.Name);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.LearningTopicInvalidNames), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenNameIsInvalid_ShouldHaveError(string name)
        {
            var createLearningTopicRequest = new CreateLearningTopicRequest(name, specialityIds);

            createLearningTopicValidator
                .TestValidate(createLearningTopicRequest)
                .ShouldHaveValidationErrorFor(t => t.Name);
        }

        [Fact]
        public void Validator_WhenSpecialityIdsAreNotEmpty_ShouldNotHaveError()
        {
            var createLearningTopicRequest = new CreateLearningTopicRequest(learningTopicName, specialityIds);

            createLearningTopicValidator
                .TestValidate(createLearningTopicRequest)
                .ShouldNotHaveValidationErrorFor(t => t.SpecialityIds);
        }

        [Fact]
        public void Validator_WhenSpecialityIdsAreEmpty_ShouldHaveError()
        {
            var emptySpecialityIds = new List<Guid>();
            var createLearningTopicRequest = new CreateLearningTopicRequest(learningTopicName, emptySpecialityIds);

            createLearningTopicValidator
                .TestValidate(createLearningTopicRequest)
                .ShouldHaveValidationErrorFor(t => t.SpecialityIds);
        }
    }
}