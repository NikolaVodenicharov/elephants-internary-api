using Core.Common;
using Core.Features.Specialities.RequestModels;
using FluentValidation;

namespace Core.Features.Specialities.Support
{
    public class CreateSpecialityValidator : AbstractValidator<CreateSpecialityRequest>
    {
        public CreateSpecialityValidator()
        {
            RuleFor(c => c.Name)
                .NotNull()
                .MinimumLength(SpecialityValidationConstants.NameMinLength)
                .MaximumLength(SpecialityValidationConstants.NameMaxLength)
                .Matches(RegularExpressionPatterns.LettersDotsNumberSignsAndExclamationMarks);
        }
    }
}
