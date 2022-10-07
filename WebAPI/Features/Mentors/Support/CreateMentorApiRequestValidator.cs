using Core.Common;
using FluentValidation;
using WebAPI.Features.Mentors.ApiRequestModels;

namespace WebAPI.Features.Mentors.Support
{
    public class CreateMentorApiRequestValidator : AbstractValidator<CreateMentorApiRequest>
    {
        public CreateMentorApiRequestValidator()
        {
            RuleFor(c => c.Email)
                .NotEmpty()
                .Matches(RegularExpressionPatterns.EmailPattern)
                .EmailAddress();
            
            RuleFor(c => c.SpecialityIds)
                .NotEmpty();
        }
    }
}