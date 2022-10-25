using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.Support;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using Xunit;

namespace Core.Tests.Features.Campaigns
{
    public class CreateCampaignRequestValidatorTests
    {
        private readonly CreateCampaignRequestValidator validator = new();

        private readonly string nameInLengthRange = "InternCampaign2000";
        private readonly DateTime startDate = DateTime.UtcNow.AddDays(5);
        private readonly DateTime endDate = DateTime.UtcNow.AddDays(50);

        public static IEnumerable<object[]> invalidCampaignNameData =>
            new List<object[]>
            {
                new object[] { string.Empty },
                new object[] { TestHelper.GenerateString(CampaignValidationConstants.NameMinLength - 1) },
                new object[] { TestHelper.GenerateString(CampaignValidationConstants.NameMaxLength + 1) },
                new object[] { "Intern Campaign 2000@#$%^&*" }
            };

        public static IEnumerable<object[]> validCampaignNameData =>
            new List<object[]>
            {
                new object[] { "Intern Campaign 2000" },
                new object[] { TestHelper.GenerateString(CampaignValidationConstants.NameMinLength) },
                new object[] { TestHelper.GenerateString(CampaignValidationConstants.NameMaxLength) },
            };
        
        public static IEnumerable<object[]> startDateTestData =>
            new List<object[]>
            {
                new object[] { DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(1) },
                new object[] { DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(3) }
            };

        [Theory]
        [MemberData(nameof(invalidCampaignNameData))]
        public void Validator_WhenNameIsInvalidLength_ShouldHaveError(string invalidCampaignName)
        {
            var createCampaign = new CreateCampaignRequest(
                invalidCampaignName,
                startDate,
                endDate,
                false);

            validator
                .TestValidate(createCampaign)
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Theory]
        [MemberData(nameof(validCampaignNameData))]
        public void Validator_WhenNameIsValidLength_ShouldNotHaveError(string validCampaignName)
        {
            var createCampaign = new CreateCampaignRequest(
                validCampaignName,
                startDate,
                endDate,
                false);

            validator
                .TestValidate(createCampaign)
                .ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Theory]
        [MemberData(nameof(startDateTestData))]
        public void Validator_WhenStartDateIsAfterEndDate_ShouldHaveError(DateTime testStartDate, DateTime testEndDate)
        {
            var createCampaign = new CreateCampaignRequest(
                nameInLengthRange,
                testStartDate,
                testEndDate,
                false
            );

            validator
                .TestValidate(createCampaign)
                .ShouldHaveValidationErrorFor(c => c.StartDate);
        }

        [Fact]
        public void Validator_WhenEndDateIsInThePast_ShouldHaveError()
        {         
            var endDateInThePast = DateTime.UtcNow.AddDays(-5);

            var createCampaign = new CreateCampaignRequest(
                nameInLengthRange,
                startDate,
                endDateInThePast,
                false);

            validator
                .TestValidate(createCampaign)
                .ShouldHaveValidationErrorFor(c => c.EndDate);
        }
    }
}