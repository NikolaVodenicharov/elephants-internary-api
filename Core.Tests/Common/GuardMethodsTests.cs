using Core.Common;
using Core.Common.Exceptions;
using Core.Features.Campaigns.Entities;
using Core.Features.Persons.ResponseModels;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Core.Tests.Common
{
    public class GuardMethodsTests
    {
        private readonly ILogger logger;
        private readonly string scopeName = "TestService";
        private readonly string entityName = "Test";
        private readonly Guid entityId = Guid.NewGuid();

        public GuardMethodsTests()
        {
            logger = new Mock<ILogger>().Object;
        }

        [Fact]
        public void EnsureNotNull_WhenEntityIsNull_ShouldThrowException()
        {
            //Arrange
            Campaign? nullCampaign = null;

            //Act
            var action = delegate() { Guard.EnsureNotNull(nullCampaign, logger, scopeName, entityName, entityId); };

            //Assert
            Assert.Throws<CoreException>(action);
        }

        [Theory]
        [InlineData(null, 1)]
        [InlineData(1, null)]
        [InlineData(null, null)]
        public void EnsureNotNullPagination_WhenPageNumIsNull_ShouldThrowException2(int? pageNum, int? pageSize)
        {
            ////Arrange
            //int? pageNum = null, pageSize = 1;

            //Act
            var action = delegate () { Guard.EnsureNotNullPagination(pageNum, pageSize, logger, scopeName); };

            //Assert
            Assert.Throws<CoreException>(action);
        }

        [Fact]
        public void EnsureNotNullAuthorization_WhenEntityIsNull_ShouldThrowException()
        {
            // Arrange
            PersonRolesSummaryResponse? nullPerson = null;

            // Act
            var action = delegate () { Guard.EnsureNotNullAuthorization(nullPerson, entityName); };
            
            // Assert
            Assert.Throws<CoreException>(action);
        }
    }
}
