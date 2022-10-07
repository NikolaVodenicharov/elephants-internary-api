using Core.Common;
using Core.Features.Users.RequestModels;
using FluentValidation;

namespace Core.Features.Users.Support
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty()
                .Matches(RegularExpressionPatterns.EmailPattern)
                .EmailAddress();
            
            RuleFor(u => u.RoleId)
                .IsInEnum();
            
            When(u => u.RoleId == Entities.RoleEnum.Mentor, () => {
                RuleFor(us => us.MentorId)
                    .NotEqual(Guid.Empty);
            });
        }
    }
}