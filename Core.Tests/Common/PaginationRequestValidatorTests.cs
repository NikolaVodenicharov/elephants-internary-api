using Core.Common.Pagination;
using FluentValidation.TestHelper;
using Xunit;

namespace Core.Tests.Common
{
    public class PaginationRequestValidatorTests
    {
        private readonly PaginationRequestValidator paginationRequestValidator = new();

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validator_WhenPageNumIsLessThanMinimum_ShouldHaveError(int pageNum)
        {
            //Arrange
            var paginationRequest = new PaginationRequest(pageNum, 1);

            //Act-Assert
            paginationRequestValidator
                .TestValidate(paginationRequest)
                .ShouldHaveValidationErrorFor(p => p.PageNum);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validator_WhenPageSizeIsLessThanMinimum_ShouldHaveError(int pageSize)
        {
            //Arrange
            var paginationRequest = new PaginationRequest(1, pageSize);

            //Act-Assert
            paginationRequestValidator
                .TestValidate(paginationRequest)
                .ShouldHaveValidationErrorFor(p => p.PageSize);
        }
    }
}
