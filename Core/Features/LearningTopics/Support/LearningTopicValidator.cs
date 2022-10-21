using Core.Features.LearningTopics.Interfaces;
using Core.Features.LearningTopics.RequestModels;
using FluentValidation;

namespace Core.Features.LearningTopics.Support
{
    public class LearningTopicValidator : ILearningTopicValidator
    {
        private readonly IValidator<CreateLearningTopicRequest> createLearningTopicValidator;
        private readonly IValidator<UpdateLearningTopicRequest> updateLearningTopicValidator;

        public LearningTopicValidator(
            IValidator<CreateLearningTopicRequest> createLearningTopicValidator, 
            IValidator<UpdateLearningTopicRequest> updateLearningTopicValidator)
        {
            this.createLearningTopicValidator = createLearningTopicValidator;
            this.updateLearningTopicValidator = updateLearningTopicValidator;
        }

        public async Task ValidateAndThrowAsync(CreateLearningTopicRequest request)
        {
            await createLearningTopicValidator.ValidateAndThrowAsync(request);
        }

        public async Task ValidateAndThrowAsync(UpdateLearningTopicRequest request)
        {
            await updateLearningTopicValidator.ValidateAndThrowAsync(request);
        }
    }
}
