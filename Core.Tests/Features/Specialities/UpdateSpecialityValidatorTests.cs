using Core.Features.Specialities.RequestModels;
using Core.Features.Specialities.Support;
using FluentValidation.TestHelper;
using System;
using Xunit;

namespace Core.Tests.Features.Specialities
{
    public class UpdateSpecialityValidatorTests
    {
        private readonly UpdateSpecialityValidator updateSpecialityValidator = new();

        private readonly Guid id = Guid.NewGuid();
        private readonly string name = "SpecialityTestName";

        [Fact]
        public void Validator_IdIsEmpty_ShouldHaveError()
        {
            //Arrange
            var updateSpeciality = new UpdateSpecialityRequest(Guid.Empty, name);

            //Act-Assert
            updateSpecialityValidator
                .TestValidate(updateSpeciality)
                .ShouldHaveValidationErrorFor(u => u.Id);
        }

        [Fact]
        public void Validator_WhenNameIsEmpty_ShouldHaveError()
        {
            //Arrange
            var updateSpeciality = new UpdateSpecialityRequest(id, string.Empty);

            //Act-Assert
            updateSpecialityValidator
                .TestValidate(updateSpeciality)
                .ShouldHaveValidationErrorFor(u => u.Name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(SpecialityValidationConstants.NameMinLength - 1)]
        [InlineData(SpecialityValidationConstants.NameMaxLength + 1)]
        public void Validator_WhenNameLengthIsOutOfRange_ShouldHaveError(int nameLength)
        {
            //Arrange
            var nameLessThanMinlength = TestHelper.GenerateString(nameLength);

            var updateSpeciality = new UpdateSpecialityRequest(id, nameLessThanMinlength);

            //Act-Assert
            updateSpecialityValidator
                .TestValidate(updateSpeciality)
                .ShouldHaveValidationErrorFor(u => u.Name);
        }

        [Theory]
        [InlineData(SpecialityValidationConstants.NameMinLength)]
        [InlineData(SpecialityValidationConstants.NameMaxLength)]
        public void Validator_WhenNameIsInAllowLength_ShouldNotHaveError(int nameLength)
        {
            //Arrange
            var nameWithMinimalAllowLength = TestHelper.GenerateString(nameLength);

            var updateSpeciality = new UpdateSpecialityRequest(id, nameWithMinimalAllowLength);

            //Act-Assert
            updateSpecialityValidator
                .TestValidate(updateSpeciality)
                .ShouldNotHaveValidationErrorFor(u => u.Name);
        }

        [Fact]
        public void Validator_WhenNameContainsDigit_ShouldHaveError()
        {
            //Arrange
            var updateSpeciality = new UpdateSpecialityRequest(id, NameEdgeCaseTestHelper.NameWithDigit);

            //Act-Assert
            updateSpecialityValidator
                .TestValidate(updateSpeciality)
                .ShouldHaveValidationErrorFor(u => u.Name);
        }
    }
}
