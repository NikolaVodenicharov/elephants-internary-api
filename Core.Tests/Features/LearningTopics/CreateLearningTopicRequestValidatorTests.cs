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
        private readonly List<Guid> specialityIds = new List<Guid>() { Guid.NewGuid() };

        public static IEnumerable<object[]> validNames = new List<object[]>
        {
            new object[] { TestHelper.GenerateString(LearniningTopicValidationConstants.NameMinLength) },
            new object[] { TestHelper.GenerateString(LearniningTopicValidationConstants.NameMaxLength) },
            new object[] { NameEdgeCaseTestHelper.NameWithWhiteSpace },
            new object[] { NameEdgeCaseTestHelper.NameWithDigit }
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
            new object[] { NameEdgeCaseTestHelper.NameWithNumberSign }
        };

        [Theory]
        [MemberData(nameof(validNames))]
        public void Validator_WhenNameIsValid_ShouldNotHaveError(string name)
        {
            var createLearningTopicRequest = new CreateLearningTopicRequest(name, specialityIds);

            createLearningTopicValidator
                .TestValidate(createLearningTopicRequest)
                .ShouldNotHaveValidationErrorFor(t => t.Name);
        }

        [Theory]
        [MemberData(nameof(invalidNames))]
        public void Validator_WhenNameIsInvalid_ShouldHaveError(string name)
        {
            var createLearningTopicRequest = new CreateLearningTopicRequest(name, specialityIds);

            createLearningTopicValidator
                .TestValidate(createLearningTopicRequest)
                .ShouldHaveValidationErrorFor(t => t.Name);
        }
    }
}