using Core.Features.Interns.RequestModels;
using FluentValidation;

namespace Core.Features.Interns.Support
{
    public class UpdateInternCampaignRequestValidator : AbstractValidator<UpdateInternCampaignRequest>
    {
        public UpdateInternCampaignRequestValidator()
        {
            RuleFor(a => a.InternId)
                .NotEqual(Guid.Empty);

            RuleFor(a => a.CampaignId)
                .NotEqual(Guid.Empty);

            RuleFor(a => a.SpecialityId)
                .NotEqual(Guid.Empty);
        }
    }
}
