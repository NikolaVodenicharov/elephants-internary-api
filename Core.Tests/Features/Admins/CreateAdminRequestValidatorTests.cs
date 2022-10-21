using Core.Features.Admins.RequestModels;
using Core.Features.Admins.Support;
using FluentValidation.TestHelper;
using Xunit;

namespace Core.Tests.Features.Admins
{
    public class CreateAdminRequestValidatorTests
    {
        CreateAdminRequestValidator createAdminValidator = new();
        private readonly string applicationUrl = "ApplicationUrl";

        [Theory]
        [MemberData(nameof(TestHelper.ValidEmails), MemberType = typeof(TestHelper))]
        public void Validator_WhenEmailIsValid_ShouldNotHaveError(string validEmail)
        {
            var createAdminRequest = new CreateAdminRequest(validEmail, applicationUrl);

            createAdminValidator
                .TestValidate(createAdminRequest)
                .ShouldNotHaveValidationErrorFor(f => f.Email);
        }

        [Theory]
        [MemberData(nameof(TestHelper.InvalidEmails), MemberType = typeof(TestHelper))]
        public void Validator_WhenEmailIsInvalid_ShouldHaveError(string invalidEmail)
        {
            var createAdminRequest = new CreateAdminRequest(invalidEmail, applicationUrl);

            createAdminValidator
                .TestValidate(createAdminRequest)
                .ShouldHaveValidationErrorFor(f => f.Email);
        }

        [Fact]
        public void Validator_WhenUrlIsEmpty_ShouldHaveError()
        {
            var createAdminRequest = new CreateAdminRequest(TestHelper.EmailMock, string.Empty);

            createAdminValidator
                .TestValidate(createAdminRequest)
                .ShouldHaveValidationErrorFor(f => f.ApplicationUrl);
        }
    }
}