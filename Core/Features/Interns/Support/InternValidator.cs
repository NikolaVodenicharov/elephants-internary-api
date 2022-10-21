using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using FluentValidation;

namespace Core.Features.Interns.Support
{
    public class InternValidator : IInternValidator
    {
        private readonly IValidator<CreateInternRequest> createInternRequestValidator;
        private readonly IValidator<UpdateInternRequest> updateInternRequestValidator;
        private readonly IValidator<AddInternCampaignRequest> addInternCampaignRequestValidator;
        private readonly IValidator<UpdateInternCampaignRequest> updateInternCampaignRequestValidator;
        private readonly IValidator<AddStateRequest> addStateRequestValidator;

        public InternValidator(
            IValidator<CreateInternRequest> createInternRequestValidator, 
            IValidator<UpdateInternRequest> updateInternRequestValidator,
            IValidator<AddInternCampaignRequest> addInternCampaignRequestValidator,
            IValidator<UpdateInternCampaignRequest> updateInternCampaignRequestValidator,
            IValidator<AddStateRequest> addStateRequestValidator)
        {
            this.createInternRequestValidator = createInternRequestValidator;
            this.updateInternRequestValidator = updateInternRequestValidator;
            this.addInternCampaignRequestValidator = addInternCampaignRequestValidator;
            this.updateInternCampaignRequestValidator = updateInternCampaignRequestValidator;
            this.addStateRequestValidator = addStateRequestValidator;
        }

        public async Task ValidateAndThrowAsync(CreateInternRequest request)
        {
            await createInternRequestValidator.ValidateAndThrowAsync(request);
        }

        public async Task ValidateAndThrowAsync(UpdateInternRequest request)
        {
            await updateInternRequestValidator.ValidateAndThrowAsync(request);
        }

        public async Task ValidateAndThrowAsync(AddInternCampaignRequest request)
        {
            await addInternCampaignRequestValidator.ValidateAndThrowAsync(request);
        }

        public async Task ValidateAndThrowAsync(UpdateInternCampaignRequest request)
        {
            await updateInternCampaignRequestValidator.ValidateAndThrowAsync(request);
        }

        public async Task ValidateAndThrowAsync(AddStateRequest request)
        {
            await addStateRequestValidator.ValidateAndThrowAsync(request);
        }
    }
}
