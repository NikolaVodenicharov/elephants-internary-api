using Core.Common;
using Core.Features.Interns.RequestModels;
using FluentValidation;

namespace Core.Features.Interns.Support
{
    public class InviteInternRequestValidator : AbstractValidator<InviteInternRequest>
    {
        public InviteInternRequestValidator()
        {
            RuleFor(i => i.Id)
                .NotEqual(Guid.Empty);

            RuleFor(i => i.WorkEmail)
                .NotEmpty()
                .Matches(RegularExpressionPatterns.EmailPattern)
                .EmailAddress();

            RuleFor(i => i.ApplicationUrl)
                .NotEmpty();
        }
    }
}
