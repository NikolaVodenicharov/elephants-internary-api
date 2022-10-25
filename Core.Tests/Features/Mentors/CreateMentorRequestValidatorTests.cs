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

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidEmails), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenEmailIsInvalid_ShouldHaveError(string invalidEmail)
        {
            var request = new CreateMentorRequest(invalidEmail, specialityIds, applicationUrl);

            validator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.ValidEmails), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenEmailIsValid_ShouldNotHaveError(string validEmail)
        {
            var request = new CreateMentorRequest(validEmail, specialityIds, applicationUrl);

            validator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(m => m.Email);
        }
    }
}
