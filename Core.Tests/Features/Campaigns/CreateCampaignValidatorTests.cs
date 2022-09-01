using FluentValidation.TestHelper;
using Xunit;
using Core.Features.Campaigns.RequestModels;
using System;
using Core.Features.Campaigns.Support;
using System.Text;

namespace Core.Tests.Features.Campaigns
{
    public class CreateCampaignValidatorTests
    {
        private readonly CreateCampaignValidator validator = new();

        private readonly string nameInLengthRange = "InternCampaign2000";
        private readonly DateTime startDate = DateTime.UtcNow.AddDays(5);
        private readonly DateTime endDate = DateTime.UtcNow.AddDays(50);

        [Fact]
        public void Validator_WhenNameIsNull_ShouldHaveError()
        {
            var createCampaing = new CreateCampaign(
                null,
                startDate,
                endDate,
                false);

            validator
                .TestValidate(createCampaing)
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_WhenNameIsEmpty_ShouldHaveError()
        {
            var createCampaing = new CreateCampaign(
                string.Empty,
                startDate,
                endDate,
                false);

            validator
                .TestValidate(createCampaing)
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_WhenNameIsLessThanMinLength_ShouldHaveError()
        {
            var nameLessThanMinlength = TestHelper.GenerateString(CampaignValidationConstants.NameMinLength - 1);

            var createCampaing = new CreateCampaign(
                nameLessThanMinlength,
                startDate,
                endDate,
                false);

            validator
                .TestValidate(createCampaing)
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_WhenNameIsWithMinAllowLength_ShouldNotHaveError()
        {
            var nameWithMinimalAllowLength = TestHelper.GenerateString(CampaignValidationConstants.NameMinLength);

            var createCampaing = new CreateCampaign(
                nameWithMinimalAllowLength,
                startDate,
                endDate,
                false);

            validator
                .TestValidate(createCampaing)
                .ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_WhenNameIsMoreThanMaxLength_ShouldHaveError()
        {
            var nameLongerThanMaxlength = TestHelper.GenerateString(CampaignValidationConstants.NameMaxLength + 1);

            var nameLength = nameLongerThanMaxlength.Length;

            var createCampaing = new CreateCampaign(
                nameLongerThanMaxlength,
                startDate,
                endDate,
                false);

            validator
                .TestValidate(createCampaing)
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_WhenNameIsWithMaxAllowLength_ShouldNotHaveError()
        {
            var nameWithMaxAllowLength = TestHelper.GenerateString(CampaignValidationConstants.NameMaxLength);

            var createCampaing = new CreateCampaign(
                nameWithMaxAllowLength,
                startDate,
                endDate,
                false);

            validator
                .TestValidate(createCampaing)
                .ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_WhenNameIsLengthRange_ShouldNotHaveError()
        {
            var createCampaing = new CreateCampaign(
                nameInLengthRange,
                startDate,
                endDate,
                false);

            validator
                .TestValidate(createCampaing)
                .ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_WhenEndDateIsBeforeStartDate_ShouldHaveError()
        {
            var startDateBiggerThanEndDate = endDate.AddDays(5);

            var createCampaing = new CreateCampaign(
                nameInLengthRange,
                startDateBiggerThanEndDate,
                endDate,
                false);

            validator
                .TestValidate(createCampaing)
                .ShouldHaveValidationErrorFor(c => c.StartDate);
        }

        [Fact]
        public void Validator_WhenStartDateIsInThePast_ShouldHaveError()
        {
            var startDateInThePast = DateTime.UtcNow.AddDays(-5);

            var createCampaing = new CreateCampaign(
                nameInLengthRange,
                startDateInThePast,
                endDate,
                false);

            validator
                .TestValidate(createCampaing)
                .ShouldHaveValidationErrorFor(c => c.StartDate);
        }

        [Fact]
        public void Validator_WhenEndDateIsInThePast_ShouldHaveError()
        {         
            var endDateInThePast = DateTime.UtcNow.AddDays(-5);

            var createCampaing = new CreateCampaign(
                nameInLengthRange,
                startDate,
                endDateInThePast,
                false);

            validator
                .TestValidate(createCampaing)
                .ShouldHaveValidationErrorFor(c => c.EndDate);
        }
    }
}