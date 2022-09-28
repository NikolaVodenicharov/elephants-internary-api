using FluentValidation;

namespace Core.Common.Pagination
{
    public class PaginationRequestValidator : AbstractValidator<PaginationRequest>
    {
        public PaginationRequestValidator()
        {
            RuleFor(c => c.PageNum)
                .NotNull()
                .WithMessage("{PropertyName} is required")
                .GreaterThan(0)
                .WithMessage("{PropertyName} must be greater than zero");

            RuleFor(c => c.PageSize)
                .NotNull()
                .WithMessage("{PropertyName} is required")
                .GreaterThan(0)
                .WithMessage("{PropertyName} must be greater than zero");
        }
    }
}
