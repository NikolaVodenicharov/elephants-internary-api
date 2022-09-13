using Core.Features.Campaigns.RequestModels;
using Core.Common;
using FluentValidation;

namespace Core.Features.Campaigns.Support
{
    public class CreateCampaignValidator : AbstractValidator<CreateCampaign>
    {
        public CreateCampaignValidator()
        {
            RuleFor(c => c.Name)
                .NotNull()
                .Matches(RegularExpressionPatterns.CampaignNamePattern)
                .MinimumLength(CampaignValidationConstants.NameMinLength)
                .MaximumLength(CampaignValidationConstants.NameMaxLength);

            RuleFor(c => c.StartDate)
                .NotNull()
                .LessThan(c => c.EndDate);

            RuleFor(c => c.EndDate)
                .NotNull()
                .GreaterThanOrEqualTo(DateTime.Today);
        }
    }
}
