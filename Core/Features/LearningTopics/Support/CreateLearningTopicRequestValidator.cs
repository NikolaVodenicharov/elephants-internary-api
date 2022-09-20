using Core.Common;
using Core.Features.LearningTopics.RequestModels;
using FluentValidation;

namespace Core.Features.LearningTopics.Support
{
    public class CreateLearningTopicRequestValidator : AbstractValidator<CreateLearningTopicRequest>
    {
        public CreateLearningTopicRequestValidator()
        {
            RuleFor(t => t.Name)
                .NotNull()
                .MinimumLength(LearniningTopicValidationConstants.NameMinLength)
                .MaximumLength(LearniningTopicValidationConstants.NameMaxLength)
                .Matches(RegularExpressionPatterns.LearningTopicNamePattern);
        }
    }
}