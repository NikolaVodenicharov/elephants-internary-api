using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.RequestModels;
using FluentValidation;

namespace Core.Features.Specialities.Support
{
    public class SpecialityValidator : ISpecialityValidator
    {
        private readonly IValidator<CreateSpecialityRequest> createSpecialityValidator;
        private readonly IValidator<UpdateSpecialityRequest> updateSpecialityValidator;

        public SpecialityValidator(
            IValidator<CreateSpecialityRequest> createSpecialityValidator,
            IValidator<UpdateSpecialityRequest> updateSpecialityValidator)
        {
            this.createSpecialityValidator = createSpecialityValidator;
            this.updateSpecialityValidator = updateSpecialityValidator;
        }

        public async Task ValidateAndThrowAsync(CreateSpecialityRequest request)
        {
            await createSpecialityValidator.ValidateAndThrowAsync(request);
        }

        public async Task ValidateAndThrowAsync(UpdateSpecialityRequest request)
        {
            await updateSpecialityValidator.ValidateAndThrowAsync(request);
        }
    }
}
