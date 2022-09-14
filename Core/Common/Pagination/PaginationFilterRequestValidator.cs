using FluentValidation;

namespace Core.Common.Pagination
{
    public class PaginationFilterRequestValidator : AbstractValidator<PaginationFilterRequest>
    {
        public PaginationFilterRequestValidator()
        {
            RuleFor(c => c.Skip)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .LessThan(c => c.Count);

            RuleFor(c => c.Take)
                .NotNull()
                .GreaterThan(0);
        }
    }
}
