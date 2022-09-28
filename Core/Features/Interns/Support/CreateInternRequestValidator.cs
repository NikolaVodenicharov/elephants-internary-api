using Core.Common;
using Core.Features.Interns.RequestModels;
using FluentValidation;

namespace Core.Features.Interns.Support
{
    public class CreateInternRequestValidator : AbstractValidator<CreateInternRequest>
    {
        public CreateInternRequestValidator()
        {
            RuleFor(i => i.FirstName)
                .NotEmpty()
                .MinimumLength(InternValidationConstants.InternNameMinLength)
                .MaximumLength(InternValidationConstants.InternNameMaxLength)
                .Matches(RegularExpressionPatterns.PersonNamesPattern);

            RuleFor(i => i.LastName)
                .NotEmpty()
                .MinimumLength(InternValidationConstants.InternNameMinLength)
                .MaximumLength(InternValidationConstants.InternNameMaxLength)
                .Matches(RegularExpressionPatterns.PersonNamesPattern);

            RuleFor(i => i.Email)
                .NotEmpty()
                .Matches(RegularExpressionPatterns.EmailPattern)
                .EmailAddress();

            RuleFor(i => i.CampaignId)
                .NotEqual(Guid.Empty);

            RuleFor(i => i.SpecialityId)
                .NotEqual(Guid.Empty);

            RuleFor(i => i.Justification)
                .MaximumLength(InternValidationConstants.JustificationMaxLength);
        }
    }
}
