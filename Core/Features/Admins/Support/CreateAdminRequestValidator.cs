using Core.Common;
using Core.Features.Admins.RequestModels;
using FluentValidation;

namespace Core.Features.Admins.Support
{
    public class CreateAdminRequestValidator : AbstractValidator<CreateAdminRequest>
    {
        public CreateAdminRequestValidator()
        {
            RuleFor(a => a.Email)
                .NotEmpty()
                .Matches(RegularExpressionPatterns.EmailPattern)
                .EmailAddress();
            
            RuleFor(a => a.ApplicationUrl)
                .NotEmpty();
        }
    }
}