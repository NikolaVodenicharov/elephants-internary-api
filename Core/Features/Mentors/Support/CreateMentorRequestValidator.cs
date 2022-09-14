using Core.Common;
using Core.Features.Mentors.RequestModels;
using FluentValidation;

namespace Core.Features.Mentors.Support
{
    public class CreateMentorRequestValidator : AbstractValidator<CreateMentorRequest>
    {
        public CreateMentorRequestValidator()
        {
            RuleFor(m => m.FirstName)
                .NotEmpty()
                .MinimumLength(MentorValidationConstraints.NamesMinLength)
                .MaximumLength(MentorValidationConstraints.NamesMaxLength)
                .Matches(RegularExpressionPatterns.PersonNamesPattern);

            RuleFor(m => m.LastName)
                .NotEmpty()
                .MinimumLength(MentorValidationConstraints.NamesMinLength)
                .MaximumLength(MentorValidationConstraints.NamesMaxLength)
                .Matches(RegularExpressionPatterns.PersonNamesPattern);

            RuleFor(m => m.Email)
                .NotEmpty()
                .Matches(RegularExpressionPatterns.EmailPattern)
                .EmailAddress();
        }
    }
}
