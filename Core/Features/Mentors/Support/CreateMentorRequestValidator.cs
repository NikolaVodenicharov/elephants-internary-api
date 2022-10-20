using Core.Common;
using Core.Features.Mentors.RequestModels;
using FluentValidation;

namespace Core.Features.Mentors.Support
{
    public class CreateMentorRequestValidator : AbstractValidator<CreateMentorRequest>
    {
        public CreateMentorRequestValidator()
        {
            RuleFor(m => m.Email)
                .NotEmpty()
                .Matches(RegularExpressionPatterns.EmailPattern)
                .EmailAddress();

            RuleFor(c => c.SpecialityIds)
                .NotEmpty();
        }
    }
}
