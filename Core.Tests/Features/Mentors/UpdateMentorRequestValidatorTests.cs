using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.Support;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using Xunit;

namespace Core.Tests.Features.Mentors
{
    public class UpdateMentorRequestValidatorTests
    {
        UpdateMentorRequestValidator validator = new();

        private Guid id = Guid.NewGuid();
        private string firstName = "Jane";
        private string lastName = "Doe";
        private string email = "jane.doe@example.com";
        private List<Guid> specialityIds = new List<Guid>() { Guid.NewGuid() };

        public static IEnumerable<object[]> invalidNames = new List<object[]>
        {
            new object[] { null },
            new object[] { string.Empty },
            new object[] { TestHelper.GenerateString(MentorValidationConstraints.NamesMinLength - 1) },
            new object[] { TestHelper.GenerateString(MentorValidationConstraints.NamesMaxLength + 1) },
            new object[] { "Test name !" },
            new object[] { "Name1" },
            new object[] { "Name " },
        };

        public static IEnumerable<object[]> invalidEmails = new List<object[]>
        {
            new object[] { null },
            new object[] { string.Empty },
             new object[] { "k.c.a" },
            new object[] { ".invalid@example.c" },
            new object[] { "invalid@example..com" },
            new object[] { "invalid@example.com." },
            new object[] { "invalidexample" },
        };

        public static IEnumerable<object[]> validNames = new List<object[]>
        {
            new object[] { TestHelper.GenerateString(MentorValidationConstraints.NamesMinLength) },
            new object[] { TestHelper.GenerateString(MentorValidationConstraints.NamesMaxLength) },
            new object[] { "John" },
            new object[] { "Ana-Maria" },
            new object[] { "Mary Alexandra" },
        };


        public static IEnumerable<object[]> validEmails = new List<object[]>
        {
            new object[] { "user.example@test.com" },
            new object[] { "first-last@example.co.uk" },
            new object[] { "random123@example.gov.in" },
        };

        [Fact]
        public void Validator_WhenIdIsEmpty_ShouldHaveError()
        {
            var request = new UpdateMentorRequest(Guid.Empty, firstName, lastName, email, specialityIds);

            validator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(m => m.Id);
        }

        [Theory]
        [MemberData(nameof(invalidNames))]
        public void Validator_WhenFirstNameIsInvalid_ShouldHaveError(string invalidFirstName)
        {
            var request = new UpdateMentorRequest(id, invalidFirstName, lastName, email, specialityIds);

            validator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(m => m.FirstName);
        }

        [Theory]
        [MemberData(nameof(invalidNames))]
        public void Validator_WhenLastNameIsInvalid_ShouldHaveError(string invalidLastName)
        {
            var request = new UpdateMentorRequest(id, firstName, invalidLastName, email, specialityIds);

            validator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(m => m.LastName);
        }

        [Theory]
        [MemberData(nameof(invalidEmails))]
        public void Validator_WhenEmailInvalid_ShouldHaveError(string invalidEmail)
        {
            var request = new UpdateMentorRequest(id, firstName, lastName, invalidEmail, specialityIds);

            validator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(m => m.Email);
        }

        [Theory]
        [MemberData(nameof(validNames))]
        public void Validator_WhenFirstNameIsValid_ShouldNotHaveError(string validFirstName)
        {
            var request = new UpdateMentorRequest(id, validFirstName, lastName, email, specialityIds);

            validator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(m => m.FirstName);
        }

        [Theory]
        [MemberData(nameof(validNames))]
        public void Validator_WhenLastNameIsValid_ShouldNotHaveError(string validLastName)
        {
            var request = new UpdateMentorRequest(id, firstName, validLastName, email, specialityIds);

            validator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(m => m.LastName);
        }

        [Theory]
        [MemberData(nameof(validEmails))]
        public void Validator_WhenEmailIsValid_ShouldNotHaveError(string validEmail)
        {
            var request = new UpdateMentorRequest(id, firstName, lastName, validEmail, specialityIds);

            validator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(m => m.Email);
        }
    }
}
