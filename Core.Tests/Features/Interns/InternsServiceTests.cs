using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.ResponseModels;
using Core.Features.Interns;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;
using Core.Features.Interns.Support;
using Core.Features.Persons.Entities;
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

        private readonly Person internMock;
        private readonly CampaignSummaryResponse campaignSummaryResponse;
        private readonly InternsService internsService;
        private readonly InternSummaryResponse internSummaryResponseMock;
        private readonly Mock<IInternsRepository> internsRepositoryMock;
        private readonly Mock<IInternCampaignsService> internCampaignsServiceMock;
        private readonly Mock<ICampaignsService> campaignServiceMock;
        private readonly Mock<ILogger<InternsService>> internsServiceLogger;

        public InternsServiceTests()
        {
            internsRepositoryMock = new Mock<IInternsRepository>();
            internCampaignsServiceMock = new Mock<IInternCampaignsService>();
            campaignServiceMock = new Mock<ICampaignsService>();
            internsServiceLogger = new Mock<ILogger<InternsService>>();

            internsService = new InternsService(
                internsRepositoryMock.Object, 
                internCampaignsServiceMock.Object,
                campaignServiceMock.Object,
                internsServiceLogger.Object,
                new CreateInternRequestValidator(),
                new UpdateInternRequestValidator(),
                new PaginationRequestValidator());

            internMock = new Person()
            {
                Id = internId,
                FirstName = "FirstName",
                LastName = "LastName",
                PersonalEmail = "FirstLast@gmail.com"
            };

            internSummaryResponseMock = new InternSummaryResponse(
                internId,
                "FirstName",
                "LastName",
                "FirstLast@gmail.com");

            campaignSummaryResponse = new CampaignSummaryResponse(
                Guid.NewGuid(),
                "Campaign 2022",
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(100),
                true);
        }

        #region CreateAsyncTests

        [Fact]
        public async Task CreateAsync_WhenEmailIsDuplicated_ShouldThrowException()
        {
            //Arrange
            internsRepositoryMock
                .Setup(x => x.ExistsByPersonalEmailAsync(It.IsAny<string>()))
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
            await internsService.CreateAsync(createInternRequest);

            //Assert
            internsRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<CreateInternRepoRequest>()), Times.Once());
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

            CreateInternRepoRequest passedRequest = null!;

            internsRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<CreateInternRepoRequest>()))
                .Callback((CreateInternRepoRequest request) => passedRequest = request);

            //Act
            await internsService.CreateAsync(createInternRequest);

            //Assert
            Assert.Equal(createInternRequest.FirstName, passedRequest.FirstName);
            Assert.Equal(createInternRequest.LastName, passedRequest.LastName);
            Assert.Equal(createInternRequest.Email, passedRequest.Email);
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
                .Setup(x => x.ExistsByPersonalEmailAsync(It.IsAny<string>()))
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
                .ReturnsAsync(internSummaryResponseMock);

            var updateInternRequest = new UpdateInternRequest(
                internId,
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock);

            UpdateInternRequest passedRequest = null!;

            var updatedInternSumamryResponseMock = new InternSummaryResponse(
                updateInternRequest.Id,
                updateInternRequest.FirstName,
                updateInternRequest.LastName,
                updateInternRequest.Email);

            internsRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<UpdateInternRequest>()))
                .Callback((UpdateInternRequest request) => passedRequest = request)
                .ReturnsAsync(updatedInternSumamryResponseMock);

            //Act
            var updatedInternSumamryResponse = await internsService.UpdateAsync(updateInternRequest);

            //Assert
            Assert.Equal(updateInternRequest, passedRequest);
            Assert.Equal(updatedInternSumamryResponseMock, updatedInternSumamryResponse);
        }

        [Fact]
        public async Task UpdateAsync_WhenRequestModelIsValid_ShouldReturnCorrectObject()
        {
            //Arrange

            var updateInternRequest = new UpdateInternRequest(
                internId,
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock);

            internsRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(internSummaryResponseMock);

            var updatedInternSummaryResponseMock = new InternSummaryResponse(
                internId,
                NameEdgeCaseTestHelper.FirstNameMock,
                NameEdgeCaseTestHelper.LastNameMock,
                NameEdgeCaseTestHelper.EmailMock);

            internsRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<UpdateInternRequest>()))
                .ReturnsAsync(updatedInternSummaryResponseMock);

            //Act
            var internSummaryResponse = await internsService.UpdateAsync(updateInternRequest);

            //Assert
            Assert.Equal(updateInternRequest.Id, internSummaryResponse.Id);
            Assert.Equal(updateInternRequest.FirstName, internSummaryResponse.FirstName);
            Assert.Equal(updateInternRequest.LastName, internSummaryResponse.LastName);
            Assert.Equal(updateInternRequest.Email, internSummaryResponse.Email);
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
        [InlineData(0, 0)]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(-1, -1)]
        public async Task GetAllAsync_WhenPageNumIsInvalid_ShouldThrowExceptionAsync(int pageNum, int pageSize)
        {
            //Arrange
            var paginationRequest = new PaginationRequest(pageNum, pageSize);

            //Act
            var action = async () => await internsService.GetAllAsync(paginationRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetAllAsync_WhenPagePageNumBiggerThanTotalPages_ShouldThrowExceptionAsync()
        {
            //Arrange
            var paginationRequest = new PaginationRequest(10, 20);

            var invalidPaginationResponse = new PaginationResponse<InternSummaryResponse>(
                new List<InternSummaryResponse>(),
                10,
                1);

            internsRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(invalidPaginationResponse);

            //Act
            var action = async () => await internsService.GetAllAsync(paginationRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
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

            campaignServiceMock
                .Setup(c => c.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaignSummaryResponse);

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
