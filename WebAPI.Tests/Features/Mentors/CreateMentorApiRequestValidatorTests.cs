using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using WebAPI.Features.Mentors.ApiRequestModels;
using WebAPI.Features.Mentors.Support;
using Xunit;

namespace WebAPI.Tests.Features.Mentors
{
    public class CreateMentorApiRequestValidatorTests
    {
        private readonly CreateMentorApiRequestValidator createMentorApiRequestValidator = new ();
        private readonly string userEmail = "user.example@test.com";
        private List<Guid> specialityIds = new List<Guid> { Guid.NewGuid() };

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
        [MemberData(nameof(validEmails))]
        public void Validator_WhenEmailIsValid_ShouldNotHaveError(string validEmail)
        {
            var request = new CreateMentorApiRequest(validEmail, specialityIds);

            createMentorApiRequestValidator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(u => u.Email);   
        }

        [Theory]
        [MemberData(nameof(invalidEmails))]
        public void Validator_WhenEmailIsInvalid_ShouldHaveError(string invalidEmail)
        {
            var request = new CreateMentorApiRequest(invalidEmail, specialityIds);

            createMentorApiRequestValidator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(u => u.Email);   
        }

        [Fact]
        public void Validator_WhenSpecialityIdsAreEmpty_ShouldHaveError()
        {
            var emptySpecialityIds = new List<Guid>();

            var request = new CreateMentorApiRequest(userEmail, emptySpecialityIds);

            createMentorApiRequestValidator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(u => u.SpecialityIds);   
        }
    }
}