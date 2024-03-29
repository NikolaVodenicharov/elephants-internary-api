﻿using Core.Features.Interns.Entities;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.Support;
using FluentValidation.TestHelper;
using System;
using Xunit;

namespace Core.Tests.Features.Interns
{
    public class AddStateRequestValidatorTests
    {
        private readonly AddStateRequestValidator addStateRequestValidator = new();
        private readonly Guid internId = Guid.NewGuid();
        private readonly Guid campaignId = Guid.NewGuid();
        private readonly string justification = "Lorem ipsum.";

        [Fact]
        public void Validator_WhenInternIdIsEmpty_ShouldHaveError()
        {
            //Arrange
            var addInternCampaignRequest = new AddStateRequest(
                Guid.Empty,
                campaignId,
                StatusId.Rejected,
                justification);

            //Act-Assert
            addStateRequestValidator
                .TestValidate(addInternCampaignRequest)
                .ShouldHaveValidationErrorFor(a => a.InternId);
        }

        [Fact]
        public void Validator_WhenCampaignIdIsEmpty_ShouldHaveError()
        {
            //Arrange
            var addInternCampaignRequest = new AddStateRequest(
                internId,
                Guid.Empty,
                StatusId.Rejected,
                justification);

            //Act-Assert
            addStateRequestValidator
                .TestValidate(addInternCampaignRequest)
                .ShouldHaveValidationErrorFor(a => a.CampaignId);
        }

        [Fact]
        public void Validator_WhenStatusIdIsInvalid_ShouldHaveError()
        {
            //Arrange
            var addInternCampaignRequest = new AddStateRequest(
                internId,
                campaignId,
                (StatusId)(-1),
                justification);

            //Act-Assert
            addStateRequestValidator
                .TestValidate(addInternCampaignRequest)
                .ShouldHaveValidationErrorFor(a => a.StatusId);
        }

        [Fact]
        public void Validator_WhenJustificationLengthIsOutOfRange_ShouldHaveError()
        {
            //Arrange
            var justificationOutOfRange = TestHelper.GenerateString(InternValidationConstants.JustificationMaxLength + 1);

            var addInternCampaignRequest = new AddStateRequest(
                internId,
                campaignId,
                StatusId.Rejected,
                justificationOutOfRange);

            //Act-Assert
            addStateRequestValidator
                .TestValidate(addInternCampaignRequest)
                .ShouldHaveValidationErrorFor(a => a.Justification);
        }

        [Fact]
        public void Validator_WhenJustificationIsEmpty_ShouldHaveError()
        {
            //Arrange
            var addInternCampaignRequest = new AddStateRequest(
                internId,
                campaignId,
                StatusId.Rejected,
                string.Empty);

            //Act-Assert
            addStateRequestValidator
                .TestValidate(addInternCampaignRequest)
                .ShouldHaveValidationErrorFor(a => a.Justification);
        }
    }
}
