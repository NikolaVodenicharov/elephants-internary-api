using Core.Features.Interns.RequestModels;
using Core.Features.Interns.Support;
using FluentValidation.TestHelper;
using System;
using Xunit;

namespace Core.Tests.Features.Interns
{
    public class AddInternCampaignRequestTests
    {
        private readonly AddInternCampaignRequestValidator addInternCampaignRequestValidator = new();
        private readonly Guid internId= Guid.NewGuid();
        private readonly Guid campaignId = Guid.NewGuid();
        private readonly Guid specialityId = Guid.NewGuid();
        private readonly string justification = "Lorem ipsum.";

        [Fact]
        public void Validator_WhenInternIdIsEmpty_ShouldHaveError()
        {
            //Arrange
            var addInternCampaignRequest = new AddInternCampaignRequest(
                Guid.Empty,
                campaignId,
                specialityId,
                justification);

            //Act-Assert
            addInternCampaignRequestValidator
                .TestValidate(addInternCampaignRequest)
                .ShouldHaveValidationErrorFor(a => a.InternId);
        }

        [Fact]
        public void Validator_WhenCampaignIdIsEmpty_ShouldHaveError()
        {
            //Arrange
            var addInternCampaignRequest = new AddInternCampaignRequest(
                internId,
                Guid.Empty,
                specialityId,
                justification);

            //Act-Assert
            addInternCampaignRequestValidator
                .TestValidate(addInternCampaignRequest)
                .ShouldHaveValidationErrorFor(a => a.CampaignId);
        }

        [Fact]
        public void Validator_WhenSpecialityIdIsEmpty_ShouldHaveError()
        {
            //Arrange
            var addInternCampaignRequest = new AddInternCampaignRequest(
                internId,
                campaignId,
                Guid.Empty,
                justification);

            //Act-Assert
            addInternCampaignRequestValidator
                .TestValidate(addInternCampaignRequest)
                .ShouldHaveValidationErrorFor(a => a.SpecialityId);
        }

        [Fact]
        public void Validator_WhenJustificationLengthIsOutOfRange_ShouldHaveError()
        {
            //Arrange
            var justificationOutOfRange = TestHelper.GenerateString(InternValidationConstants.JustificationMaxLength + 1);

            var addInternCampaignRequest = new AddInternCampaignRequest(
                internId,
                campaignId,
                specialityId,
                justificationOutOfRange);

            //Act-Assert
            addInternCampaignRequestValidator
                .TestValidate(addInternCampaignRequest)
                .ShouldHaveValidationErrorFor(a => a.Justification);
        }
    }
}
