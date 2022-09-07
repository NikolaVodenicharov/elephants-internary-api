using FluentValidation;
using Core.Features.Campaigns.RequestModels;
using Core.Common;

namespace Core.Features.Campaigns.Support
{
    public class UpdateCampaignValidator : AbstractValidator<UpdateCampaign>
    {
        public UpdateCampaignValidator()
        {
            RuleFor(a => a.Id)
                .NotEmpty();
            
            RuleFor(a => a.Name)
                .NotNull()
                .Matches(RegularExpressionPatterns.WordsDigidsDashesAndWhiteSpacePattern)
                .MinimumLength(CampaignValidationConstants.NameMinLength)
                .MaximumLength(CampaignValidationConstants.NameMaxLength);
            
            RuleFor(a => a.StartDate)
                .NotNull();
            
            RuleFor(a => a.EndDate)
                .NotNull()
                .GreaterThan(a => a.StartDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow);
        }
    }
}
