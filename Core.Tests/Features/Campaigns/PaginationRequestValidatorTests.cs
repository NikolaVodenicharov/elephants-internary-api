using Core.Common.Pagination;
using FluentValidation.TestHelper;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Features.Campaigns
{
    public class PaginationRequestValidatorTests
    {
        private readonly PaginationRequestValidator validator = new();        

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Validator_WhenPageNumIsGreaterThanZero_ShouldNotHaveError(int validPageNum)
        {
            var filter = new PaginationRequest(validPageNum, 5);

            validator
                .TestValidate(filter)
                .ShouldNotHaveValidationErrorFor(f => f.PageNum);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task Validator_WhenPageNumIsLessThanOne_ShouldHaveError(int invalidPageNum)
        {
            var filter = new PaginationRequest(invalidPageNum, 5);

            validator
                .TestValidate(filter)
                .ShouldHaveValidationErrorFor(f => f.PageNum);
        }

        [Fact]
        public async Task Validator_WhenPageNumIsNull_ShouldHaveError()
        {
            var filter = new PaginationRequest(null, 1);

            validator
                .TestValidate(filter)
                .ShouldHaveValidationErrorFor(f => f.PageNum);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Validator_WhenPageSizeIsGreaterThanZero_ShouldNotHaveError(int validPageSize)
        {
            var filter = new PaginationRequest(1, validPageSize);

            validator
                .TestValidate(filter)
                .ShouldNotHaveValidationErrorFor(f => f.PageSize);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task Validator_WhenPageSizeIsLessThanOne_ShouldHaveError(int invalidPageSize)
        {
            var filter = new PaginationRequest(1, invalidPageSize);

            validator
                .TestValidate(filter)
                .ShouldHaveValidationErrorFor(f => f.PageSize);
        }

        [Fact]
        public async Task Validator_WhenPageSizeIsNull_ShouldHaveError()
        {
            var filter = new PaginationRequest(1, null);

            validator
                .TestValidate(filter)
                .ShouldHaveValidationErrorFor(f => f.PageSize);
        }

    }
}
