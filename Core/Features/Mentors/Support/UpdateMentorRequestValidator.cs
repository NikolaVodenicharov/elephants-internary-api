using Core.Common;
using Core.Features.Mentors.RequestModels;
using FluentValidation;

namespace Core.Features.Mentors.Support
{
    public class UpdateMentorRequestValidator : AbstractValidator<UpdateMentorRequest>
    {
        public UpdateMentorRequestValidator()
        {
            RuleFor(m => m.Id)
                .NotEqual(Guid.Empty);
            
            RuleFor(m => m.SpecialityIds)
                .NotEmpty();
        }
    }
}
