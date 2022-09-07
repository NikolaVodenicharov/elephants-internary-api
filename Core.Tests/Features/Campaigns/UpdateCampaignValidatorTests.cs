using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.Support;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using Xunit;
namespace Core.Tests.Features.Campaigns
{
    public class UpdateCampaignValidatorTests
    {
        private readonly UpdateCampaignValidator validator = new UpdateCampaignValidator();

        private Guid id = Guid.NewGuid();
        private string campaignName = "CampaignTestName";
        private DateTime startDate = DateTime.UtcNow.AddDays(5);
        private DateTime endDate = DateTime.UtcNow.AddDays(50);
        private bool isActive = false;

        public static IEnumerable<object[]> endDateTestData =>
            new List<object[]>
            {
                new object[] { DateTime.UtcNow.AddDays(-3), DateTime.UtcNow.AddDays(-1) },
                new object[] { DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(3) }
            };

        public static IEnumerable<object[]> invalidCampaignNameData =>
            new List<object[]>
            {
                new object[] { null },
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
        
        [Fact]
        public void Validator_WhenIdIsEmpty_ShouldHaveError()
        {
            var updateCampaign = new UpdateCampaign(Guid.Empty, campaignName, startDate, endDate, isActive);

            validator
                .TestValidate(updateCampaign)
                .ShouldHaveValidationErrorFor(a => a.Id);
        }

        [Theory]
        [MemberData(nameof(validCampaignNameData))]
        public void Validator_WhenNameLengthIsValid_ShouldNotHaveError(string validCampaignName)
        {
            var updateCampaign = new UpdateCampaign(id, validCampaignName, startDate, endDate, isActive);

            validator
                .TestValidate(updateCampaign)
                .ShouldNotHaveValidationErrorFor(a => a.Name);
        }

        [Theory]
        [MemberData(nameof(invalidCampaignNameData))]
        public void Validator_WhenInvalidNameLength_ShouldHaveError(string invalidCampaignName)
        {
            var updateCampaign = new UpdateCampaign(id, invalidCampaignName, startDate, endDate, isActive);

            validator
                .TestValidate(updateCampaign)
                .ShouldHaveValidationErrorFor(a => a.Name);
        }
        
        [Fact]
        public void Validator_StartDateIsValid_ShouldNotHaveError()
        {
            var updateCampaign = new UpdateCampaign(id, campaignName, startDate, endDate, isActive);

            validator
                .TestValidate(updateCampaign)
                .ShouldNotHaveValidationErrorFor(a => a.StartDate);
        }

        [Fact]
        public void Validator_EndDateIsValid_ShouldNotHaveError()
        {
            var updateCampaign = new UpdateCampaign(id, campaignName, startDate, endDate, isActive);

            validator
                .TestValidate(updateCampaign)
                .ShouldNotHaveValidationErrorFor(a => a.EndDate);
        }

        [Theory]
        [MemberData(nameof(endDateTestData))]
        public void Validator_EndDateIsInvalid_ShouldHaveError(DateTime testStartDate, DateTime invalidEndDate)
        {
            var updateCampaign = new UpdateCampaign(id, campaignName, testStartDate, invalidEndDate, isActive);

            validator
                .TestValidate(updateCampaign)
                .ShouldHaveValidationErrorFor(a => a.EndDate);
        }
    }
}