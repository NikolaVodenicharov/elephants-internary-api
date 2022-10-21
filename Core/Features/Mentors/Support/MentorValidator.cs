using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using FluentValidation;

namespace Core.Features.Mentors.Support
{
    public class MentorValidator : IMentorValidator
    {
        private readonly IValidator<CreateMentorRequest> createMentorValidator;
        private readonly IValidator<UpdateMentorRequest> updateMentorValidator;

        public MentorValidator(
            IValidator<CreateMentorRequest> createMentorValidator, 
            IValidator<UpdateMentorRequest> updateMentorValidator)
        {
            this.createMentorValidator = createMentorValidator;
            this.updateMentorValidator = updateMentorValidator;
        }

        public async Task ValidateAndThrowAsync(CreateMentorRequest request)
        {
            await createMentorValidator.ValidateAndThrowAsync(request);
        }

        public async Task ValidateAndThrowAsync(UpdateMentorRequest request)
        {
            await updateMentorValidator.ValidateAndThrowAsync(request);
        }
    }
}
