using Core.Features.Users.Entities;
using Core.Features.Users.RequestModels;
using Core.Features.Users.Support;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using Xunit;

namespace Core.Tests.Features.Users
{
    public class CreateUserValidatorTests
    {
        private readonly CreateUserRequestValidator createUserRequestValidator = new ();
        private readonly string userEmail = "user.example@test.com";
        private readonly Guid mentorId = Guid.NewGuid();

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
            var request = new CreateUserRequest(validEmail, RoleEnum.Mentor, mentorId);

            createUserRequestValidator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(u => u.Email);   
        }

        [Theory]
        [MemberData(nameof(invalidEmails))]
        public void Validator_WhenEmailIsInvalid_ShouldHaveError(string invalidEmail)
        {
            var request = new CreateUserRequest(invalidEmail, RoleEnum.Mentor, mentorId);

            createUserRequestValidator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(u => u.Email);   
        }

        [Theory]
        [InlineData(RoleEnum.Intern)]
        [InlineData(RoleEnum.Mentor)]
        [InlineData(RoleEnum.Administrator)]
        public void Validator_WhenRoleIsInEnum_ShouldNotHaveError(RoleEnum roleId)
        {
            var request = new CreateUserRequest(userEmail, roleId, mentorId);

            createUserRequestValidator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(u => u.RoleId);
        }

        [Fact]
        public void Validator_WhenRoleIsMentorWithId_ShouldNotHaveError()
        {
            var request = new CreateUserRequest(userEmail, RoleEnum.Mentor, mentorId);

            createUserRequestValidator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(u => u.MentorId);
        }

        [Fact]
        public void Validator_WhenRoleIsMentorWithoutId_ShouldHaveError()
        {
            var request = new CreateUserRequest(userEmail, RoleEnum.Mentor, Guid.Empty);

            createUserRequestValidator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(u => u.MentorId);
        }

        [Fact]
        public void Validator_WhenRoleNotMentorWithoutId_ShouldNotHaveError()
        {
            var request = new CreateUserRequest(userEmail, RoleEnum.Administrator, Guid.Empty);

            createUserRequestValidator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(u => u.MentorId);
        }
    }
}