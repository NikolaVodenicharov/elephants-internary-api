using Core.Common;
using Core.Features.Specialities.RequestModels;
using FluentValidation;

namespace Core.Features.Specialities.Support
{
    public class UpdateSpecialityValidator : AbstractValidator<UpdateSpecialityRequest>
    {
        public UpdateSpecialityValidator()
        {
            RuleFor(u => u.Id)
                .NotEqual(Guid.Empty);

            RuleFor(u => u.Name)
                .NotEmpty()
                .MinimumLength(SpecialityValidationConstants.NameMinLength)
                .MaximumLength(SpecialityValidationConstants.NameMaxLength)
                .Matches(RegularExpressionPatterns.SpecialityNamePattern);
        }
    }
}
