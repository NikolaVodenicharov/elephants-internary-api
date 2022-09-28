using Core.Features.Interns.RequestModels;
using Core.Features.Interns.Support;
using FluentValidation.TestHelper;
using System;
using Xunit;

namespace Core.Tests.Features.Interns
{
    public class UpdateInternCampaignRequestValidatorTests
    {
        private readonly UpdateInternCampaignRequestValidator updateInternCampaignRequestValidator = new();
        private readonly Guid internId = Guid.NewGuid();
        private readonly Guid campaignId = Guid.NewGuid();
        private readonly Guid specialityId = Guid.NewGuid();
        private readonly string justification = "Lorem ipsum.";

        [Fact]
        public void Validator_WhenInternIdIsEmpty_ShouldHaveError()
        {
            //Arrange
            var updateInternCampaignRequest = new UpdateInternCampaignRequest(
                Guid.Empty,
                campaignId,
                specialityId);

            //Act-Assert
            updateInternCampaignRequestValidator
                .TestValidate(updateInternCampaignRequest)
                .ShouldHaveValidationErrorFor(a => a.InternId);
        }

        [Fact]
        public void Validator_WhenCampaignIdIsEmpty_ShouldHaveError()
        {
            //Arrange
            var updateInternCampaignRequest = new UpdateInternCampaignRequest(
                internId,
                Guid.Empty,
                specialityId);

            //Act-Assert
            updateInternCampaignRequestValidator
                .TestValidate(updateInternCampaignRequest)
                .ShouldHaveValidationErrorFor(a => a.CampaignId);
        }

        [Fact]
        public void Validator_WhenSpecialityIdIsEmpty_ShouldHaveError()
        {
            //Arrange
            var updateInternCampaignRequest = new UpdateInternCampaignRequest(
                internId,
                campaignId,
                Guid.Empty);

            //Act-Assert
            updateInternCampaignRequestValidator
                .TestValidate(updateInternCampaignRequest)
                .ShouldHaveValidationErrorFor(a => a.SpecialityId);
        }
    }
}
