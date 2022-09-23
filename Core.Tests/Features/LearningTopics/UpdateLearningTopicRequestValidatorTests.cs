using Core.Features.LearningTopics.RequestModels;
using Core.Features.LearningTopics.Support;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using Xunit;

namespace Core.Tests.Features.LearningTopics
{
    public class UpdateLearningTopicRequestValidatorTests
    {
        private readonly UpdateLearningTopicRequestValidator updateLearningTopicValidator = new();
        private readonly Guid id = Guid.NewGuid();
        private readonly string name = "Learning Topic Test";
        private readonly List<Guid> specialityIds = new List<Guid>() { Guid.NewGuid() };

        public static IEnumerable<object[]> validNames = new List<object[]>
        {
            new object[] { TestHelper.GenerateString(LearniningTopicValidationConstants.NameMinLength) },
            new object[] { TestHelper.GenerateString(LearniningTopicValidationConstants.NameMaxLength) },
            new object[] { NameEdgeCaseTestHelper.NameWithWhiteSpace }
        };

        public static IEnumerable<object[]> invalidNames = new List<object[]>
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

        [Fact]
        public void Validator_WhenIdIsValid_ShouldNotHaveError()
        {
            var updateLearningTopicRequest = new UpdateLearningTopicRequest(id, name, specialityIds);

            updateLearningTopicValidator
                .TestValidate(updateLearningTopicRequest)
                .ShouldNotHaveValidationErrorFor(t => t.Id);
        }

        [Fact]
        public void Validator_WhenIdIsEmpty_ShouldHaveError()
        {
            var updateLearningTopicRequest = new UpdateLearningTopicRequest(Guid.Empty, name, specialityIds);

            updateLearningTopicValidator
                .TestValidate(updateLearningTopicRequest)
                .ShouldHaveValidationErrorFor(t => t.Id);
        }

        [Theory]
        [MemberData(nameof(validNames))]
        public void Validator_WhenNameIsValid_ShouldNotHaveError(string validName)
        {
            var updateLearningTopicRequest = new UpdateLearningTopicRequest(id, validName, specialityIds);

            updateLearningTopicValidator
                .TestValidate(updateLearningTopicRequest)
                .ShouldNotHaveValidationErrorFor(t => t.Name);
        }

        [Theory]
        [MemberData(nameof(invalidNames))]
        public void Validator_WhenNameIsInvalid_ShouldHaveError(string invalidName)
        {
            var updateLearningTopicRequest = new UpdateLearningTopicRequest(id, invalidName, specialityIds);

            updateLearningTopicValidator
                .TestValidate(updateLearningTopicRequest)
                .ShouldHaveValidationErrorFor(t => t.Name);
        }

        [Fact]
        public void Validator_WhenSpecialityIdsAreNotEmpty_ShouldNotHaveError()
        {
            var updateLearningTopicRequest = new UpdateLearningTopicRequest(id, name, specialityIds);

            updateLearningTopicValidator
                .TestValidate(updateLearningTopicRequest)
                .ShouldNotHaveValidationErrorFor(t => t.SpecialityIds);
        }

        [Fact]
        public void Validator_WhenSpecialityIdsAreEmpty_ShouldHaveError()
        {
            var emptySpecialityIds = new List<Guid>();
            var updateLearningTopicRequest = new UpdateLearningTopicRequest(id, name, emptySpecialityIds);

            updateLearningTopicValidator
                .TestValidate(updateLearningTopicRequest)
                .ShouldHaveValidationErrorFor(t => t.SpecialityIds);
        }
    }
}