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
        private readonly CreateMentorRequestValidator validator = new();
        private readonly string applicationUrl = "ApplicationUrl";
        private readonly List<Guid> specialityIds = new()
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

        [Theory]
        [MemberData(nameof(invalidEmails))]
        public void Validator_WhenEmailIsInvalid_ShouldHaveError(string invalidEmail)
        {
            var request = new CreateMentorRequest(invalidEmail, specialityIds, applicationUrl);

            validator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [MemberData(nameof(validEmails))]
        public void Validator_WhenEmailIsValid_ShouldNotHaveError(string validEmail)
        {
            var request = new CreateMentorRequest(validEmail, specialityIds, applicationUrl);

            validator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(m => m.Email);
        }
    }
}
