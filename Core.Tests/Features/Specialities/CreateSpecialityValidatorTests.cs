using FluentValidation.TestHelper;
using Xunit;
using Core.Features.Specialities.Support;
using Core.Features.Specialities.RequestModels;

namespace Core.Tests.Features.Specialities
{
    public class CreateSpecialityValidatorTests
    {
        private readonly CreateSpecialityValidator createSpecialityValidator = new();

        [Fact]
        public void Validator_WhenNameIsNull_ShouldHaveError()
        {
            //Arrange
            var createSpeciality = new CreateSpecialityRequest(null);

            //Act-Assert
            createSpecialityValidator
                .TestValidate(createSpeciality)
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(SpecialityValidationConstants.NameMinLength - 1)]
        [InlineData(SpecialityValidationConstants.NameMaxLength + 1)]
        public void Validator_WhenNameLengthIsOutOfRange_ShouldHaveError(int nameLength)
        {
            //Arrange
            var nameLessThanMinlength = TestHelper.GenerateString(nameLength);

            var createSpeciality = new CreateSpecialityRequest(nameLessThanMinlength);

            //Act-Assert
            createSpecialityValidator
                .TestValidate(createSpeciality)
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Theory]
        [InlineData(SpecialityValidationConstants.NameMinLength)]
        [InlineData(SpecialityValidationConstants.NameMaxLength)]
        public void Validator_WhenNameIsInAllowLength_ShouldNotHaveError(int nameLength)
        {
            //Arrange
            var nameWithMinimalAllowLength = TestHelper.GenerateString(nameLength);

            var createSpeciality = new CreateSpecialityRequest(nameWithMinimalAllowLength);

            //Act-Assert
            createSpecialityValidator
                .TestValidate(createSpeciality)
                .ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Theory]
        [InlineData(NameEdgeCaseTestHelper.NameWithDot)]
        [InlineData(NameEdgeCaseTestHelper.NameWithExclamationMark)]
        [InlineData(NameEdgeCaseTestHelper.NameWithNumberSign)]
        public void Validator_WhenNameContainsAllowedSpecialCharacters_ShouldNotHaveError(string name)
        {
            //Arrange
            var createSpeciality = new CreateSpecialityRequest(name);

            //Act-Assert
            createSpecialityValidator
                .TestValidate(createSpeciality)
                .ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Theory]
        [InlineData(NameEdgeCaseTestHelper.NameWithAmpersand)]
        [InlineData(NameEdgeCaseTestHelper.NameWithDash)]
        [InlineData(NameEdgeCaseTestHelper.NameWithWhiteSpace)]
        [InlineData(NameEdgeCaseTestHelper.NameWithDigit)]
        public void Validator_WhenNameContainsForbiddenSpecialCharacters_ShouldHaveError(string name)
        {
            //Arrange
            var createSpeciality = new CreateSpecialityRequest(name);

            //Act-Assert
            createSpecialityValidator
                .TestValidate(createSpeciality)
                .ShouldHaveValidationErrorFor(c => c.Name);
        }
    }
}
