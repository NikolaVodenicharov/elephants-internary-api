using FluentValidation;
using Core.Features.Campaigns.RequestModels;
using Core.Common;

namespace Core.Features.Campaigns.Support
{
    public class UpdateCampaignRequestValidator : AbstractValidator<UpdateCampaignRequest>
    {
        public UpdateCampaignRequestValidator()
        {
            RuleFor(a => a.Id)
                .NotEqual(Guid.Empty);

            RuleFor(a => a.Name)
                .NotEmpty()
                .Matches(RegularExpressionPatterns.CampaignNamePattern)
                .MinimumLength(CampaignValidationConstants.NameMinLength)
                .MaximumLength(CampaignValidationConstants.NameMaxLength);

            RuleFor(a => a.StartDate)
                .NotNull();

            RuleFor(a => a.EndDate)
                .NotNull()
                .GreaterThan(a => a.StartDate)
                .GreaterThanOrEqualTo(DateTime.Today);
        }
    }
}
