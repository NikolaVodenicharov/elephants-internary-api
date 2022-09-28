using Core.Features.Interns.RequestModels;
using FluentValidation;

namespace Core.Features.Interns.Support
{
    public class AddInternCampaignRequestValidator : AbstractValidator<AddInternCampaignRequest>
    {
        public AddInternCampaignRequestValidator()
        {
            RuleFor(a => a.InternId)
                .NotEqual(Guid.Empty);

            RuleFor(a => a.CampaignId)
                .NotEqual(Guid.Empty);

            RuleFor(a => a.SpecialityId)
                .NotEqual(Guid.Empty);

            RuleFor(a => a.InternId)
                .NotEqual(Guid.Empty);

            RuleFor(i => i.Justification)
                .MaximumLength(InternValidationConstants.JustificationMaxLength);
        }
    }
}
