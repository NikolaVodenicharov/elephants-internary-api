using Core.Features.Interns.RequestModels;
using Core.Features.Interns.Support;
using FluentValidation.TestHelper;
using System;
using Xunit;

namespace Core.Tests.Features.Interns
{
    public class CreateInternRequestValidatorTests
    {
        private readonly CreateInternRequestValidator createInternRequestValidator = new();
        private readonly Guid specialityId = Guid.NewGuid();
        private readonly Guid campaignId = Guid.NewGuid();
        private readonly string justification = "Lorem ipsum.";

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidPersonNames), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenFirstNameIsInvalid_ShouldHaveError(string invalidFirstName)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                invalidFirstName, 
                MockDataTestHelper.LastNameMock, 
                MockDataTestHelper.WorkEmailMock, 
                campaignId, 
                specialityId, 
                justification);

            //Act-Assert
            createInternRequestValidator
                .TestValidate(createInternRequest)
                .ShouldHaveValidationErrorFor(c => c.FirstName);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.ValidPersonNames), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenFirstNameIsValid_ShouldNotHaveError(string validFirstName)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                validFirstName, 
                MockDataTestHelper.LastNameMock, 
                MockDataTestHelper.WorkEmailMock, 
                campaignId, 
                specialityId, 
                justification);

            //Act-Assert
            createInternRequestValidator
                .TestValidate(createInternRequest)
                .ShouldNotHaveValidationErrorFor(c => c.FirstName);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidPersonNames), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenLastNameIsInvalid_ShouldHaveError(string invalidLastName)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                MockDataTestHelper.FirstNameMock, 
                invalidLastName, 
                MockDataTestHelper.WorkEmailMock, 
                campaignId, 
                specialityId, 
                justification);

            //Act-Assert
            createInternRequestValidator
                .TestValidate(createInternRequest)
                .ShouldHaveValidationErrorFor(c => c.LastName);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.ValidPersonNames), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenLastNameIsValid_ShouldNotHaveError(string validLastName)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                MockDataTestHelper.FirstNameMock, 
                validLastName, 
                MockDataTestHelper.WorkEmailMock, 
                campaignId, 
                specialityId, 
                justification);

            //Act-Assert
            createInternRequestValidator
                .TestValidate(createInternRequest)
                .ShouldNotHaveValidationErrorFor(c => c.LastName);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidEmails), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenEmailIsInvalid_ShouldHaveError(string invalidEmail)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                MockDataTestHelper.FirstNameMock, 
                MockDataTestHelper.LastNameMock, 
                invalidEmail, 
                campaignId, 
                specialityId, 
                justification);

            //Act-Assert
            createInternRequestValidator
                .TestValidate(createInternRequest)
                .ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.ValidEmails), MemberType = typeof(MockDataTestHelper))]
        public void Validator_WhenEmailIsValid_ShouldNotHaveError(string validEmail)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                MockDataTestHelper.FirstNameMock, 
                MockDataTestHelper.LastNameMock, 
                validEmail, 
                campaignId, 
                specialityId, 
                justification);

            //Act-Assert
            createInternRequestValidator
                .TestValidate(createInternRequest)
                .ShouldNotHaveValidationErrorFor(c => c.Email);
        }

        [Fact]
        public void Validator_WhenCampaignIdIsEmpty_ShouldHaveError()
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                MockDataTestHelper.FirstNameMock, 
                MockDataTestHelper.LastNameMock, 
                MockDataTestHelper.WorkEmailMock, 
                Guid.Empty, 
                specialityId, 
                justification);

            //Act-Assert
            createInternRequestValidator
                .TestValidate(createInternRequest)
                .ShouldHaveValidationErrorFor(c => c.CampaignId);
        }

        [Fact]
        public void Validator_WhenSpecialityIdIsEmpty_ShouldHaveError()
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                MockDataTestHelper.FirstNameMock, 
                MockDataTestHelper.LastNameMock, 
                MockDataTestHelper.WorkEmailMock, 
                campaignId, 
                Guid.Empty, 
                justification);

            //Act-Assert
            createInternRequestValidator
                .TestValidate(createInternRequest)
                .ShouldHaveValidationErrorFor(c => c.SpecialityId);
        }

        [Fact]
        public void Validator_WhenJustificationLengthIsOutOfRange_ShouldHaveError()
        {
            //Arrange
            var justificationOutOfRange = TestHelper.GenerateString(InternValidationConstants.JustificationMaxLength + 1);

            var createInternRequest = new CreateInternRequest(
                MockDataTestHelper.FirstNameMock, 
                MockDataTestHelper.LastNameMock, 
                MockDataTestHelper.WorkEmailMock, 
                campaignId, 
                specialityId, 
                justificationOutOfRange);

            //Act-Assert
            createInternRequestValidator
                .TestValidate(createInternRequest)
                .ShouldHaveValidationErrorFor(c => c.Justification);
        }
    }
}
