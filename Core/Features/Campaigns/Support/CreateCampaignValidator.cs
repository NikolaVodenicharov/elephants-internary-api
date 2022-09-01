using Core.Features.Campaigns.RequestModels;
using FluentValidation;

namespace Core.Features.Campaigns.Support
{
    public class CreateCampaignValidator : AbstractValidator<CreateCampaign>
    {
        public CreateCampaignValidator()
        {
            RuleFor(c => c.Name)
                .NotNull()
                .MinimumLength(CampaignValidationConstants.NameMinLength)
                .MaximumLength(CampaignValidationConstants.NameMaxLength);

            RuleFor(c => c.StartDate)
                .NotNull()
                .GreaterThan(DateTime.UtcNow)
                .LessThan(c => c.EndDate);

            RuleFor(c => c.EndDate)
                .NotNull()
                .GreaterThan(DateTime.UtcNow);
        }
    }
}
