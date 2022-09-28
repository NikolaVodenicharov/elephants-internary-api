using Core.Features.Interns.RequestModels;
using Core.Features.Interns.Support;
using FluentValidation.TestHelper;
using System;
using Xunit;

namespace Core.Tests.Features.Interns
{
    public class UpdateInternRequestValidatorTests
    {
        private readonly UpdateInternRequestValidator updateInternRequestValidator = new();
        private readonly Guid Id = Guid.NewGuid();

        [Fact]
        public void Validator_WhenIdIsEmpty_ShouldHaveError()
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(Guid.Empty, NameEdgeCaseTestHelper.FirstNameMock, NameEdgeCaseTestHelper.LastNameMock, NameEdgeCaseTestHelper.EmailMock);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldHaveValidationErrorFor(c => c.Id);
        }

        [Theory]
        [MemberData(nameof(NameEdgeCaseTestHelper.InvalidPersonNames), MemberType = typeof(NameEdgeCaseTestHelper))]
        public void Validator_WhenFirstNameIsInvalid_ShouldHaveError(string invalidFirstName)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(Id, invalidFirstName, NameEdgeCaseTestHelper.LastNameMock, NameEdgeCaseTestHelper.EmailMock);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldHaveValidationErrorFor(c => c.FirstName);
        }

        [Theory]
        [MemberData(nameof(NameEdgeCaseTestHelper.ValidPersonNames), MemberType = typeof(NameEdgeCaseTestHelper))]
        public void Validator_WhenFirstNameIsValid_ShouldNotHaveError(string validFirstName)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(Id, validFirstName, NameEdgeCaseTestHelper.LastNameMock, NameEdgeCaseTestHelper.EmailMock);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldNotHaveValidationErrorFor(c => c.FirstName);
        }

        [Theory]
        [MemberData(nameof(NameEdgeCaseTestHelper.InvalidPersonNames), MemberType = typeof(NameEdgeCaseTestHelper))]
        public void Validator_WhenLastNameIsInvalid_ShouldHaveError(string invalidLastName)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(Id, NameEdgeCaseTestHelper.FirstNameMock, invalidLastName, NameEdgeCaseTestHelper.EmailMock);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldHaveValidationErrorFor(c => c.LastName);
        }

        [Theory]
        [MemberData(nameof(NameEdgeCaseTestHelper.ValidPersonNames), MemberType = typeof(NameEdgeCaseTestHelper))]
        public void Validator_WhenLastNameIsValid_ShouldNotHaveError(string validLastName)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(Id, NameEdgeCaseTestHelper.FirstNameMock, validLastName, NameEdgeCaseTestHelper.EmailMock);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldNotHaveValidationErrorFor(c => c.LastName);
        }

        [Theory]
        [MemberData(nameof(NameEdgeCaseTestHelper.InvalidEmails), MemberType = typeof(NameEdgeCaseTestHelper))]
        public void Validator_WhenEmailIsInvalid_ShouldHaveError(string invalidEmail)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(Id, NameEdgeCaseTestHelper.FirstNameMock, NameEdgeCaseTestHelper.LastNameMock, invalidEmail);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [MemberData(nameof(NameEdgeCaseTestHelper.ValidEmails), MemberType = typeof(NameEdgeCaseTestHelper))]
        public void Validator_WhenEmailIsValid_ShouldNotHaveError(string validEmail)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(Id, NameEdgeCaseTestHelper.FirstNameMock, NameEdgeCaseTestHelper.LastNameMock, validEmail);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldNotHaveValidationErrorFor(c => c.Email);
        }
    }
}
