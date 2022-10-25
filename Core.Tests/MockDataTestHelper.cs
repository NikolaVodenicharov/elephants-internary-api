using Core.Features.Interns.Support;
using Core.Features.LearningTopics.Support;
using System.Collections.Generic;

namespace Core.Tests
{
    public static class MockDataTestHelper
    {

        public const string FirstNameMock = "John";
        public const string LastNameMock = "Doe";
        public const string DisplayNameMock = "John Doe";
        public const string PersonalEmailMock = "John.Doe@gmail.com";
        public const string WorkEmailMock = "John.Doe@endava.com";
        public const string ApplicationUrlMock = "http://backofficemock";

        public static readonly IEnumerable<object[]> InvalidEmails = new List<object[]>
        {
            new object[] { "k.c.a" },
            new object[] { ".invalid@example.c" },
            new object[] { "invalid@example..com" },
            new object[] { "invalid@example.com." },
            new object[] { "invalidexample" },
            new object[] { "invalidexample.com" },
            new object[] { "invalidexample.co.uk." },
            new object[] { "invalidexample.co_uk" },
            new object[] { "invalidexample.co_ukkkk" },
        };

        public static readonly IEnumerable<object[]> ValidEmails = new List<object[]>
        {
            new object[] { "user.example@test.com" },
            new object[] { "first-last@example.co.uk" },
            new object[] { "random123@example.gov.in" },
        };

        public static readonly IEnumerable<object[]> ValidPersonNames = new List<object[]>
        {
            new object[] { TestHelper.GenerateString(InternValidationConstants.InternNameMinLength) },
            new object[] { TestHelper.GenerateString(InternValidationConstants.InternNameMaxLength) },
            new object[] { "John" },
            new object[] { "Ana-Maria" },
            new object[] { "Mary Alexandra" },
        };

        public static readonly IEnumerable<object[]> InvalidPersonNames = new List<object[]>
        {
            new object[] { TestHelper.GenerateString(InternValidationConstants.InternNameMinLength - 1) },
            new object[] { TestHelper.GenerateString(InternValidationConstants.InternNameMaxLength + 1) },
            new object[] { "Name1" },
            new object[] { " Name" },
            new object[] { "Name " },
        };

        public static readonly IEnumerable<object[]> LearningTopicValidNames = new List<object[]>
        {
            new object[] { TestHelper.GenerateString(LearniningTopicValidationConstants.NameMinLength) },
            new object[] { TestHelper.GenerateString(LearniningTopicValidationConstants.NameMaxLength) },
        };

        public static readonly IEnumerable<object[]> LearningTopicInvalidNames = new List<object[]>
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
    }
}