using Core.Features.Interns.RequestModels;
using FluentValidation;

namespace Core.Features.Interns.Support
{
    public class AddStateRequestValidator : AbstractValidator<AddStateRequest>
    {
        public AddStateRequestValidator()
        {
            RuleFor(a => a.InternId)
                .NotEqual(Guid.Empty);

            RuleFor(a => a.CampaignId)
                .NotEqual(Guid.Empty);

            RuleFor(a => a.Justification)
                .NotEmpty()
                .MinimumLength(InternValidationConstants.JustificationMinLength)
                .MaximumLength(InternValidationConstants.JustificationMaxLength);

            RuleFor(a => a.StatusId)
                .IsInEnum();
        }
    }
}
