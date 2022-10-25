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

        [Theory]
        [MemberData(nameof(TestHelper.ValidEmails), MemberType = typeof(TestHelper))]
        public void Validator_WhenEmailIsValid_ShouldNotHaveError(string validEmail)
        {
            var request = new CreateMentorApiRequest(validEmail, specialityIds);

            createMentorApiRequestValidator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(u => u.Email);   
        }

        [Theory]
        [MemberData(nameof(TestHelper.InvalidEmails), MemberType = typeof(TestHelper))]
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