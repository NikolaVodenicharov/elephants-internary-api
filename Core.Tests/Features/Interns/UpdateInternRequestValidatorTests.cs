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
            var updateInternRequest = new UpdateInternRequest(Guid.Empty, MockDataTestHelper.FirstNameMock, MockDataTestHelper.LastNameMock, MockDataTestHelper.PersonalEmailMock);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldHaveValidationErrorFor(c => c.Id);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidPersonNames), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenFirstNameIsInvalid_ShouldHaveError(string invalidFirstName)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(Id, invalidFirstName, MockDataTestHelper.LastNameMock, MockDataTestHelper.PersonalEmailMock);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldHaveValidationErrorFor(c => c.FirstName);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.ValidPersonNames), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenFirstNameIsValid_ShouldNotHaveError(string validFirstName)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(Id, validFirstName, MockDataTestHelper.LastNameMock, MockDataTestHelper.PersonalEmailMock);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldNotHaveValidationErrorFor(c => c.FirstName);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidPersonNames), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenLastNameIsInvalid_ShouldHaveError(string invalidLastName)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(Id, MockDataTestHelper.FirstNameMock, invalidLastName, MockDataTestHelper.PersonalEmailMock);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldHaveValidationErrorFor(c => c.LastName);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.ValidPersonNames), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenLastNameIsValid_ShouldNotHaveError(string validLastName)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(Id, MockDataTestHelper.FirstNameMock, validLastName, MockDataTestHelper.PersonalEmailMock);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldNotHaveValidationErrorFor(c => c.LastName);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidEmails), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenEmailIsInvalid_ShouldHaveError(string invalidEmail)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(Id, MockDataTestHelper.FirstNameMock, MockDataTestHelper.LastNameMock, invalidEmail);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.ValidEmails), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenEmailIsValid_ShouldNotHaveError(string validEmail)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(Id, MockDataTestHelper.FirstNameMock, MockDataTestHelper.LastNameMock, validEmail);

            //Act-Assert
            updateInternRequestValidator
                .TestValidate(updateInternRequest)
                .ShouldNotHaveValidationErrorFor(c => c.Email);
        }
    }
}
