using Core.Common;
using Core.Common.Exceptions;
using Core.Features.Campaigns.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [Fact]
        public void EnsureNotNullPagination_WhenPageNumIsNull_ShouldThrowException()
        {
            //Arrange
            int? pageNum = null, pageSize = 1;

            //Act
            var action = delegate () { Guard.EnsureNotNullPagination(pageNum, pageSize, logger, scopeName); };

            //Assert
            Assert.Throws<CoreException>(action);
        }

        [Fact]
        public void EnsyreNotNullPagination_WhenPageSizeIsNull_ShouldThrowException()
        {
            //Arrange
            int? pageNum = 1, pageSize = null;

            //Act
            var action = delegate () { Guard.EnsureNotNullPagination(pageNum, pageSize, logger, scopeName); };

            //Assert
            Assert.Throws<CoreException>(action);
        }

        [Fact]
        public void EnsureNotNullPagination_WhenPageNummAndPageSizeAreNull_ShouldThrowException()
        {
            //Arrange
            int? pageNum = null, pageSize = null;

            //Act
            var action = delegate () { Guard.EnsureNotNullPagination(pageNum, pageSize, logger, scopeName); };

            //Assert
            Assert.Throws<CoreException>(action);
        }
    }
}
