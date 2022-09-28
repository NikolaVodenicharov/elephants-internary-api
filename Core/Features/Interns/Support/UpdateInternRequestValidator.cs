using Core.Common;
using Core.Features.Interns.RequestModels;
using FluentValidation;

namespace Core.Features.Interns.Support
{
    public class UpdateInternRequestValidator : AbstractValidator<UpdateInternRequest>
    {
        public UpdateInternRequestValidator()
        {
            RuleFor(u => u.Id)
                .NotEqual(Guid.Empty);

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
        }
    }
}
