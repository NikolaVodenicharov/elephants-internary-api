using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Interns;
using Core.Features.Interns.Entities;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;
using Core.Features.Interns.Support;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Features.Interns
{
    public class InternsServiceTests
    {
        private readonly Guid internId = Guid.NewGuid();
        private readonly Guid specialityId = Guid.NewGuid();
        private readonly Guid campaignId = Guid.NewGuid();
        private readonly string justification = "Lorem ipsum.";
        private readonly int pageNum = 1;

        private readonly Intern internMock;
        private readonly InternsService internsService;
        private readonly Mock<IInternsRepository> internsRepositoryMock;
        private readonly Mock<IInternCampaignsService> internCampaignsServiceMock;
        private readonly Mock<ILogger<InternsService>> internsServiceLogger;

        public InternsServiceTests()
        {
            internsRepositoryMock = new Mock<IInternsRepository>();
            internCampaignsServiceMock = new Mock<IInternCampaignsService>();
            internsServiceLogger = new Mock<ILogger<InternsService>>();

            internsService = new InternsService(
                internsRepositoryMock.Object, 
                internCampaignsServiceMock.Object, 
                internsServiceLogger.Object,
                new CreateInternRequestValidator(),
                new UpdateInternRequestValidator(),
                new PaginationRequestValidator());

            internMock = new Intern()
            {
                Id = internId,
                FirstName = "FirstName",
                LastName = "LastName",
                PersonalEmail = "FirstLast@gmail.com"
            };
        }

        #region CreateAsyncTests

        [Fact]
        public async Task CreateAsync_WhenEmailIsDuplicated_ShouldThrowException()
        {
            //Arrange
            internsRepositoryMock
                .Setup(x => x.ExistsByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var createInternRequest = new CreateInternRequest(
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock,
                campaignId,
                specialityId,
                justification);

            //Act
            var action = async () => await internsService.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Theory]
        [MemberData(nameof(NameEdgeCaseTestHelper.InvalidPersonNames), MemberType = typeof(NameEdgeCaseTestHelper))]
        public async Task CreateAsync_WhenFirstNameIsInvalid_ShouldThrowException(string invalidFirstName)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                invalidFirstName,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock,
                campaignId,
                specialityId,
                justification);

            //Act
            var action = async () => await internsService.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(NameEdgeCaseTestHelper.InvalidPersonNames), MemberType = typeof(NameEdgeCaseTestHelper))]
        public async Task CreateAsync_WhenLastNameIsInvalid_ShouldThrowException(string invalidLastName)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                NameEdgeCaseTestHelper.FirstNameMock,
                invalidLastName,
                NameEdgeCaseTestHelper.EmailMock,
                campaignId,
                specialityId,
                justification);

            //Act
            var action = async () => await internsService.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(NameEdgeCaseTestHelper.InvalidEmails), MemberType = typeof(NameEdgeCaseTestHelper))]
        public async Task CreateAsync_WhenEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                invalidEmail,
                campaignId,
                specialityId,
                justification);

            //Act
            var action = async () => await internsService.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenCampaignIdIsEmpty_ShouldThrowException()
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock,
                Guid.Empty,
                specialityId,
                justification);

            //Act
            var action = async () => await internsService.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenSpecialityIdIsEmpty_ShouldThrowException()
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock,
                campaignId,
                Guid.Empty,
                justification);

            //Act
            var action = async () => await internsService.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenJustificationLengthIsOutOfRange_ShouldThrowException()
        {
            //Arrange
            var justificationOutOfRange = TestHelper.GenerateString(InternValidationConstants.JustificationMaxLength + 1);

            var createInternRequest = new CreateInternRequest(
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock,
                campaignId,
                specialityId,
                justificationOutOfRange);

            //Act
            var action = async () => await internsService.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_WhenRequestModelIsValid_ShouldCallRepository()
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock,
                campaignId,
                specialityId,
                justification);

            //Act
            var internSummaryResponse = await internsService.CreateAsync(createInternRequest);

            //Assert
            internsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Intern>()), Times.Once());
        }

        [Fact]
        public async Task CreateAsync_WhenRequestModelIsValid_ShouldPassCorrectObject()
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock,
                campaignId,
                specialityId,
                justification);

            Intern passedIntern = null;

            internsRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Intern>()))
                .Callback((Intern intern) => passedIntern = intern);

            //Act
            await internsService.CreateAsync(createInternRequest);

            //Assert
            Assert.Equal(createInternRequest.FirstName, passedIntern.FirstName);
            Assert.Equal(createInternRequest.LastName, passedIntern.LastName);
            Assert.Equal(createInternRequest.Email, passedIntern.PersonalEmail);
            Assert.Equal(1, passedIntern.InternCampaigns.Count);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_WhenIdIsEmpty_ShouldThrowException()
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(
                Guid.Empty,
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock);

            //Act
            var action = async () => await internsService.UpdateAsync(updateInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenIdIsNotFound_ShouldThrowException()
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(
                internId,
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock);

            //Act
            var action = async () => await internsService.UpdateAsync(updateInternRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenEmailIsDuplicated_ShouldThrowException()
        {
            //Arrange
            internsRepositoryMock
                .Setup(x => x.ExistsByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var updateInternRequest = new UpdateInternRequest(
                internId,
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock);

            //Act
            var action = async () => await internsService.UpdateAsync(updateInternRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Theory]
        [MemberData(nameof(NameEdgeCaseTestHelper.InvalidPersonNames), MemberType = typeof(NameEdgeCaseTestHelper))]
        public async Task UpdateAsync_WhenFirstNameIsInvalid_ShouldThrowException(string invalidFirstName)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(
                internId,
                invalidFirstName,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock);

            //Act
            var action = async () => await internsService.UpdateAsync(updateInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(NameEdgeCaseTestHelper.InvalidPersonNames), MemberType = typeof(NameEdgeCaseTestHelper))]
        public async Task UpdateAsync_WhenLastNameIsInvalid_ShouldThrowException(string invalidLastName)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(
                internId,
                NameEdgeCaseTestHelper.FirstNameMock,
                invalidLastName,
                NameEdgeCaseTestHelper.EmailMock);

            //Act
            var action = async () => await internsService.UpdateAsync(updateInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(NameEdgeCaseTestHelper.InvalidEmails), MemberType = typeof(NameEdgeCaseTestHelper))]
        public async Task UpdateAsync_WhenEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(
                internId,
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                invalidEmail);

            //Act
            var action = async () => await internsService.UpdateAsync(updateInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateAsync_WhenRequestModelIsValid_ShouldCallRepository()
        {
            //Arrange
            internsRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(internMock);

            var updateInternRequest = new UpdateInternRequest(
                internId,
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock);

            //Act
            await internsService.UpdateAsync(updateInternRequest);

            //Assert
            internsRepositoryMock.Verify(r => r.SaveTrackingChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_WhenRequestModelIsValid_ShouldReturnCorrectObject()
        {
            //Arrange
            internsRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(internMock);

            var updateInternRequest = new UpdateInternRequest(
                internId,
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock);

            //Act
            var internSummaryResponse = await internsService.UpdateAsync(updateInternRequest);

            //Assert
            Assert.Equal(updateInternRequest.Id, internSummaryResponse.Id);
            Assert.Equal(updateInternRequest.FirstName, internSummaryResponse.FirstName);
            Assert.Equal(updateInternRequest.LastName, internSummaryResponse.LastName);
            Assert.Equal(updateInternRequest.Email, internSummaryResponse.Email);
        }

        [Fact]
        public async Task UpdateAsync_WhenEmailsIsNotChanges_ShouldNotCheckForEmailDuplicationInDatabase()
        {
            //Arrange
            var internMock = new Intern()
            {
                Id = internId,
                FirstName = "FirstName",
                LastName = "LastName",
                PersonalEmail = NameEdgeCaseTestHelper.EmailMock
            };

            internsRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(internMock);

            var updateInternRequest = new UpdateInternRequest(
                internId,
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock);

            //Act
            var internSummaryResponse = await internsService.UpdateAsync(updateInternRequest);

            //Assert
            internsRepositoryMock.Verify(r => r.ExistsByEmailAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region GetDetailsByIdAsync

        [Fact]
        public async Task GetDetailsByIdAsync_WhenIdIsNotFound_ShouldThrowException()
        {
            //Act
            var action = async () => await internsService.GetDetailsByIdAsync(Guid.NewGuid());

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetDetailsByIdAsync_WhenIdFound_ShouldReturnCorrectObject()
        {
            //Arrange
            var internDetailsResponseMock = new InternDetailsResponse(
                internId,
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock,
                new List<InternCampaignSummaryResponse>());

            internsRepositoryMock
                .Setup(r => r.GetDetailsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(internDetailsResponseMock);

            //Act
            var internDetailsResponse = await internsService.GetDetailsByIdAsync(internMock.Id);

            //Assert
            Assert.Equal(internDetailsResponseMock, internDetailsResponse);
        }

        #endregion

        #region GetAllAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetAllAsync_WhenPageNumIsLessThanMinimum_ShouldThrowExceptionAsync(int pageNum)
        {
            //Arrange
            var paginationRequest = new PaginationRequest(pageNum, 1);

            //Act
            var action = async () => await internsService.GetAllAsync(paginationRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetAllAsync_WhenPageSizeIsLessThanMinimum_ShouldThrowExceptionAsync(int pageSize)
        {
            //Arrange
            var paginationRequest = new PaginationRequest(1, pageSize);

            //Act
            var action = async () => await internsService.GetAllAsync(paginationRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetAllAsync_WhenValid_ShouldPassCorrectObject()
        {
            //Arrange 
            var paginationRequest = new PaginationRequest(pageNum, 10);

            var paginationResponseMock = new PaginationResponse<InternSummaryResponse>(
                new List<InternSummaryResponse>(),
                pageNum, 
                5);

            internsRepositoryMock
                .Setup(i => i.GetAllAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(paginationResponseMock);

            //Act
            var paginationResponse = await internsService.GetAllAsync(paginationRequest);

            //Assert
            Assert.Equal(paginationResponseMock.PageNum, paginationResponse.PageNum);
            Assert.Equal(paginationResponseMock.TotalPages, paginationResponse.TotalPages);
        }

        #endregion

        #region GetAllByCampaignIdAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetAllByCampaignIdAsync_WhenPageNumIsLessThanMinimum_ShouldThrowExceptionAsync(int pageNum)
        {
            //Arrange
            var paginationRequest = new PaginationRequest(pageNum, 1);

            //Act
            var action = async () => await internsService.GetAllByCampaignIdAsync(paginationRequest, Guid.NewGuid());

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetAllByCampaignIdAsync_WhenPageSizeIsLessThanMinimum_ShouldThrowExceptionAsync(int pageSize)
        {
            //Arrange
            var paginationRequest = new PaginationRequest(1, pageSize);

            //Act
            var action = async () => await internsService.GetAllByCampaignIdAsync(paginationRequest, Guid.NewGuid());

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetAllByCampaignIdAsync_WhenValid_ShouldPassCorrectObject()
        {
            //Arrange 
            var paginationRequest = new PaginationRequest(pageNum, 10);

            var paginationResponseMock = new PaginationResponse<InternByCampaignSummaryResponse>(
                new List<InternByCampaignSummaryResponse>(),
                pageNum,
                5);

            internsRepositoryMock
                .Setup(i => i.GetAllByCampaignIdAsync(It.IsAny<PaginationRequest>(), It.IsAny<Guid>()))
                .ReturnsAsync(paginationResponseMock);

            //Act
            var internsByCampaignPaginationResponse = await internsService.GetAllByCampaignIdAsync(paginationRequest, Guid.NewGuid());

            //Assert
            Assert.Equal(paginationResponseMock.PageNum, internsByCampaignPaginationResponse.PageNum);
            Assert.Equal(paginationResponseMock.TotalPages, internsByCampaignPaginationResponse.TotalPages);
            Assert.NotNull(internsByCampaignPaginationResponse.Content);
        }

        #endregion
    }
}
