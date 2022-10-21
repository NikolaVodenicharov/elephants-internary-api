using Core.Features.Admins.RequestModels;
using FluentValidation;

namespace Core.Features.Admins.Support
{
    public class AddMentorRoleRequestValidator : AbstractValidator<AddMentorRoleRequest>
    {
        public AddMentorRoleRequestValidator()
        {
            RuleFor(a => a.Id)
                .NotEqual(Guid.Empty);
            
            RuleFor(a => a.SpecialityIds)
                .NotEmpty();
        }
    }
}