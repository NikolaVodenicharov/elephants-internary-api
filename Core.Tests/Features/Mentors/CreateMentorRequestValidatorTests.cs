using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.Support;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using Xunit;

namespace Core.Tests.Features.Mentors
{
    public class CreateMentorRequestValidatorTests
    {
        CreateMentorRequestValidator validator = new();

        private readonly string displayName = "Ivan Ivanov";
        private readonly string email = "Ivan.Ivanov@endava.com";
        private List<Guid> specialityIds = new List<Guid>()
        {
            Guid.NewGuid()
        };

        public static IEnumerable<object[]> invalidEmails = new List<object[]>
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

        public static IEnumerable<object[]> validEmails = new List<object[]>
        {
            new object[] { "user.example@test.com" },
            new object[] { "first-last@example.co.uk" },
            new object[] { "random123@example.gov.in" },
        };

        [Fact]
        public void Validator_WhenDisplayNameIsNotEmpty_ShouldNotHaveError()
        {
            var request = new CreateMentorRequest(displayName, email, specialityIds);

            validator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(c => c.DisplayName);
        }

        [Fact]
        public void Validator_WhenDisplayNameIsEmpty_ShouldHaveError()
        {
            var request = new CreateMentorRequest(string.Empty, email, specialityIds);

            validator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(c => c.DisplayName);
        }

        [Theory]
        [MemberData(nameof(invalidEmails))]
        public void Validator_WhenEmailIsInvalid_ShouldHaveError(string invalidEmail)
        {
            var request = new CreateMentorRequest(displayName, invalidEmail, specialityIds);

            validator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [MemberData(nameof(validEmails))]
        public void Validator_WhenEmailIsValid_ShouldNotHaveError(string validEmail)
        {
            var request = new CreateMentorRequest(displayName, validEmail, specialityIds);

            validator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(m => m.Email);
        }
    }
}
