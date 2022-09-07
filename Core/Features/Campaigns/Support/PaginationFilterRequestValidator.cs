using Core.Features.Campaigns.RequestModels;
using FluentValidation;

namespace Core.Features.Campaigns.Support
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
