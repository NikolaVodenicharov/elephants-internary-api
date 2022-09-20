using Core.Common;
using Core.Features.LearningTopics.RequestModels;
using FluentValidation;

namespace Core.Features.LearningTopics.Support
{
    public class UpdateLearningTopicRequestValidator : AbstractValidator<UpdateLearningTopicRequest>
    {
        public UpdateLearningTopicRequestValidator()
        {
            RuleFor(t => t.Id)
                .NotEqual(Guid.Empty);
                
            RuleFor(t => t.Name)
                .NotNull()
                .MinimumLength(LearniningTopicValidationConstants.NameMinLength)
                .MaximumLength(LearniningTopicValidationConstants.NameMaxLength)
                .Matches(RegularExpressionPatterns.LearningTopicNamePattern);
        }
    }
}
