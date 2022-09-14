using Core.Common.Pagination;
using FluentValidation.TestHelper;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Features.Campaigns
{
    public class PaginationFilterRequestValidatorTests
    {
        private readonly PaginationFilterRequestValidator validator = new();        

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Validator_WhenSkipIsValid_ShouldNotHaveError(int validSkip)
        {
            var filter = new PaginationFilterRequest()
            {
                Skip = validSkip,
                Take = 5,
                Count = 15
            };

            validator
                .TestValidate(filter)
                .ShouldNotHaveValidationErrorFor(f => f.Skip);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(15)]
        [InlineData(20)]
        public async Task Validator_WhenSkipIsInvalid_ShouldHaveError(int invalidSkip)
        {
            var filter = new PaginationFilterRequest()
            {
                Skip = invalidSkip,
                Take = 10,
                Count = 15
            };

            validator
                .TestValidate(filter)
                .ShouldHaveValidationErrorFor(f => f.Skip);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Validator_WhenTakeIsGreaterThanOrEqualToOne_ShouldNotHaveError(int take)
        {
            var filter = new PaginationFilterRequest()
            {
                Skip = 0,
                Take = take,
                Count = 10
            };

            validator
                .TestValidate(filter)
                .ShouldNotHaveValidationErrorFor(f => f.Take);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task Validator_WhenTakeIsLessThanOne_ShouldHaveError(int take)
        {
            var filter = new PaginationFilterRequest()
            {
                Skip = 0,
                Take = take,
                Count = 10
            };

            validator
                .TestValidate(filter)
                .ShouldHaveValidationErrorFor(f => f.Take);
        }

    }
}
