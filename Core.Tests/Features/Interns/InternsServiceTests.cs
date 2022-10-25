using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.ResponseModels;
using Core.Features.Interns;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;
using Core.Features.Interns.Support;
using Core.Features.Persons.Entities;
using Core.Features.Persons.Interfaces;
using Core.Features.Persons.ResponseModels;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly string workEmail = "JohnDoe@endava.com"; 
        private readonly string workEmail2 = "WorkEmailTwo@gmail.com";
        private readonly string displayName1 = "DisplayNameOne";
        private readonly string displayName2 = "DisplayNameTwo";
        private readonly Person internMock;
        private readonly CampaignSummaryResponse campaignSummaryResponseMock;
        private readonly InternsService internsService;
        private readonly InternSummaryResponse internSummaryResponseMock;
        private readonly InternSummaryResponse internSummaryResponseMock2;
        private readonly InternDetailsResponse internDetailsResponseMock;
        private readonly Mock<IInternsRepository> internsRepositoryMock;
        private readonly IdentitySummaryResponse identitySummaryResponseMock;
        private readonly Mock<IIdentityRepository> identityRepositoryMock;
        private readonly Mock<IInternCampaignsService> internCampaignsServiceMock;
        private readonly Mock<ICampaignsService> campaignServiceMock;
        private readonly Mock<ILogger<InternsService>> internsServiceLogger;

        public InternsServiceTests()
        {
            internsRepositoryMock = new Mock<IInternsRepository>();
            identityRepositoryMock = new Mock<IIdentityRepository>();
            internCampaignsServiceMock = new Mock<IInternCampaignsService>();
            campaignServiceMock = new Mock<ICampaignsService>();
            internsServiceLogger = new Mock<ILogger<InternsService>>();

            var createInternRequestValidator = new CreateInternRequestValidator();
            var updateInternRequestValidator = new UpdateInternRequestValidator();
            var addInternCampaignequestValidator = new AddInternCampaignRequestValidator();
            var updateInternCampaignRequestValidator = new UpdateInternCampaignRequestValidator();
            var addStateRequestValidator = new AddStateRequestValidator();
            var inviteInternRequestValidator = new InviteInternRequestValidator();

            var internValidator = new InternValidator(
                createInternRequestValidator, updateInternRequestValidator,
                addInternCampaignequestValidator,
                updateInternCampaignRequestValidator, addStateRequestValidator,
                inviteInternRequestValidator);

            internsService = new InternsService(
                internsRepositoryMock.Object,
                identityRepositoryMock.Object,
                internCampaignsServiceMock.Object,
                campaignServiceMock.Object,
                internsServiceLogger.Object,
                internValidator,
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
                "FirstName LastName",
                "FirstLast@gmail.com");

            campaignSummaryResponseMock = new CampaignSummaryResponse(
                Guid.NewGuid(),
                "Campaign 2022",
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(100),
                true);

            internSummaryResponseMock2 = new InternSummaryResponse(
                internId,
                displayName2,
                workEmail2);

            internDetailsResponseMock = new InternDetailsResponse(
                internId,
                displayName1,
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock,
                MockDataTestHelper.WorkEmailMock,
                new List<InternCampaignSummaryResponse>());

            identitySummaryResponseMock = new IdentitySummaryResponse(workEmail2, displayName2);
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
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock,
                campaignId,
                specialityId,
                justification);

            //Act
            var action = async () => await internsService.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidPersonNames), MemberType = typeof(MockDataTestHelper))]
        public async Task CreateAsync_WhenFirstNameIsInvalid_ShouldThrowException(string invalidFirstName)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                invalidFirstName,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock,
                campaignId,
                specialityId,
                justification);

            //Act
            var action = async () => await internsService.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidPersonNames), MemberType = typeof(MockDataTestHelper))]
        public async Task CreateAsync_WhenLastNameIsInvalid_ShouldThrowException(string invalidLastName)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                MockDataTestHelper.FirstNameMock,
                invalidLastName,
                MockDataTestHelper.PersonalEmailMock,
                campaignId,
                specialityId,
                justification);

            //Act
            var action = async () => await internsService.CreateAsync(createInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidEmails), MemberType = typeof(MockDataTestHelper))]
        public async Task CreateAsync_WhenEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {
            //Arrange
            var createInternRequest = new CreateInternRequest(
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
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
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock,
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
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock,
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
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock,
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
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock,
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
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock,
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
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock);

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
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock);

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
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock);

            //Act
            var action = async () => await internsService.UpdateAsync(updateInternRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidPersonNames), MemberType = typeof(MockDataTestHelper))]
        public async Task UpdateAsync_WhenFirstNameIsInvalid_ShouldThrowException(string invalidFirstName)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(
                internId,
                invalidFirstName,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock);

            //Act
            var action = async () => await internsService.UpdateAsync(updateInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidPersonNames), MemberType = typeof(MockDataTestHelper))]
        public async Task UpdateAsync_WhenLastNameIsInvalid_ShouldThrowException(string invalidLastName)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(
                internId,
                MockDataTestHelper.FirstNameMock,
                invalidLastName,
                MockDataTestHelper.PersonalEmailMock);

            //Act
            var action = async () => await internsService.UpdateAsync(updateInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidEmails), MemberType = typeof(MockDataTestHelper))]
        public async Task UpdateAsync_WhenEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {
            //Arrange
            var updateInternRequest = new UpdateInternRequest(
                internId,
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
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
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock);

            UpdateInternRequest passedRequest = null!;

            var updatedInternSumamryResponseMock = new InternSummaryResponse(
                updateInternRequest.Id,
                updateInternRequest.FirstName + " " + updateInternRequest.LastName,
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
                MockDataTestHelper.FirstNameMock,
                MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock);

            internsRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(internSummaryResponseMock);

            var updatedInternSummaryResponseMock = new InternSummaryResponse(
                internId,
                MockDataTestHelper.FirstNameMock + " " + MockDataTestHelper.LastNameMock,
                MockDataTestHelper.PersonalEmailMock);

            internsRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<UpdateInternRequest>()))
                .ReturnsAsync(updatedInternSummaryResponseMock);

            //Act
            var internSummaryResponse = await internsService.UpdateAsync(updateInternRequest);

            //Assert
            Assert.Equal(updateInternRequest.Id, internSummaryResponse.Id);
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

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            //Act
            var internListingResponses = await internsService.GetAllAsync();

            //Assert
            Assert.Empty(internListingResponses);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectObject()
        {
            //Arrange
            var internListingResponsesMock = new List<InternListingResponse>()
            {
                new InternListingResponse(
                    internId,
                    displayName1,
                    workEmail,
                    new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() })
            };

            internsRepositoryMock
                .Setup(i => i.GetAllAsync())
                .ReturnsAsync(internListingResponsesMock);

            //Act
            var internListingResponses = await internsService.GetAllAsync();

            //Assert

            Assert.Single(internListingResponses);
            Assert.Equal(2, internListingResponses.First().CampaignIds.Count());
        }

        #endregion

        #region GetPaginationAsync

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(-1, -1)]
        public async Task GetPaginationAsync_WhenPageNumIsInvalid_ShouldThrowExceptionAsync(int pageNum, int pageSize)
        {
            //Arrange
            var paginationRequest = new PaginationRequest(pageNum, pageSize);

            //Act
            var action = async () => await internsService.GetPaginationAsync(paginationRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetPaginationAsync_WhenPagePageNumBiggerThanTotalPages_ShouldThrowExceptionAsync()
        {
            //Arrange
            var paginationRequest = new PaginationRequest(10, 20);

            var invalidPaginationResponse = new PaginationResponse<InternListingResponse>(
                new List<InternListingResponse>(),
                10,
                1);

            internsRepositoryMock
                .Setup(x => x.GetPaginationAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(invalidPaginationResponse);

            //Act
            var action = async () => await internsService.GetPaginationAsync(paginationRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task GetPaginationAsync_WhenValid_ShouldPassCorrectObject()
        {
            //Arrange 
            var paginationRequest = new PaginationRequest(pageNum, 10);

            var paginationResponseMock = new PaginationResponse<InternListingResponse>(
                new List<InternListingResponse>(),
                pageNum, 
                5);

            internsRepositoryMock
                .Setup(i => i.GetPaginationAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(paginationResponseMock);

            //Act
            var paginationResponse = await internsService.GetPaginationAsync(paginationRequest);

            //Assert
            Assert.Equal(paginationResponseMock.PageNum, paginationResponse.PageNum);
            Assert.Equal(paginationResponseMock.TotalPages, paginationResponse.TotalPages);
        }

        #endregion

        #region GetPaginationByCampaignIdAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetPaginationByCampaignIdAsync_WhenPageNumIsLessThanMinimum_ShouldThrowExceptionAsync(int pageNum)
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
        public async Task GetPaginationByCampaignIdAsync_WhenPageSizeIsLessThanMinimum_ShouldThrowExceptionAsync(int pageSize)
        {
            //Arrange
            var paginationRequest = new PaginationRequest(1, pageSize);

            //Act
            var action = async () => await internsService.GetAllByCampaignIdAsync(paginationRequest, Guid.NewGuid());

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task GetPaginationByCampaignIdAsync_WhenValid_ShouldPassCorrectObject()
        {
            //Arrange 
            var paginationRequest = new PaginationRequest(pageNum, 10);

            var paginationResponseMock = new PaginationResponse<InternSummaryResponse>(
                new List<InternSummaryResponse>(),
                pageNum,
                5);

            campaignServiceMock
                .Setup(c => c.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaignSummaryResponseMock);

            internsRepositoryMock
                .Setup(i => i.GetPaginationByCampaignIdAsync(It.IsAny<PaginationRequest>(), It.IsAny<Guid>()))
                .ReturnsAsync(paginationResponseMock);

            //Act
            var internsByCampaignPaginationResponse = await internsService.GetAllByCampaignIdAsync(paginationRequest, Guid.NewGuid());

            //Assert
            Assert.Equal(paginationResponseMock.PageNum, internsByCampaignPaginationResponse.PageNum);
            Assert.Equal(paginationResponseMock.TotalPages, internsByCampaignPaginationResponse.TotalPages);
            Assert.NotNull(internsByCampaignPaginationResponse.Content);
        }

        #endregion

        #region InviteAsync

        [Theory]
        [MemberData(nameof(MockDataTestHelper.InvalidEmails), MemberType = typeof(MockDataTestHelper))]
        public async Task InviteAsync_WhenEmailIsInvalid_ShouldThrowException(string invalidEmail)
        {
            //Arrange
            var inviteInternRequest = new InviteInternRequest(
                internId,
                invalidEmail,
                MockDataTestHelper.ApplicationUrlMock);

            //Act
            var action = async () => await internsService.InviteAsync(inviteInternRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task InviteAsync_WhenIdNotFound_ShouldThrowException()
        {
            //Arrange
            var inviteInternRequest = new InviteInternRequest(
                Guid.NewGuid(),
                workEmail,
                MockDataTestHelper.ApplicationUrlMock);

            //Act
            var action = async () => await internsService.InviteAsync(inviteInternRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task InviteAsync_WhenRequestEmailAlreadyAdded_ShouldStopInvitationProcess()
        {
            //Arrange
            var inviteInternRequest = new InviteInternRequest(
                internDetailsResponseMock.Id,
                internDetailsResponseMock.WorkEmail,
                MockDataTestHelper.ApplicationUrlMock);

            internsRepositoryMock
                .Setup(i => i.GetDetailsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(internDetailsResponseMock);

            //Act
            var internSummaryResponse = await internsService.InviteAsync(inviteInternRequest);

            //Assert
            identityRepositoryMock.Verify(i => i.SendUserInviteAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            internsRepositoryMock.Verify(i => i.AddIdentityAsync(It.IsAny<AddInternIdentityRepoRequest>()), Times.Never);

            Assert.Equal(internDetailsResponseMock.WorkEmail, internSummaryResponse.Email);
            Assert.Equal(internDetailsResponseMock.DisplayName, internSummaryResponse.DisplayName);
        }

        [Fact]
        public async Task InviteAsync_WhenEmailIsUsedByOther_ShouldThrowException()
        {
            //Arrange
            var inviteInternRequest = new InviteInternRequest(
                internDetailsResponseMock.Id,
                "AlreadyUsed@gmail.com",
                MockDataTestHelper.ApplicationUrlMock);

            internsRepositoryMock
                .Setup(i => i.GetDetailsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(internDetailsResponseMock);

            internsRepositoryMock
                .Setup(i => i.ExistsByPersonalEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            //Act
            var action = async () => await internsService.InviteAsync(inviteInternRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task InviteAsync_WhenDataIsValid_ShouldOverrideDisplayNameAndEmail()
        {
            //Arrange
            var inviteInternRequest = new InviteInternRequest(
                internDetailsResponseMock.Id,
                workEmail2,
                MockDataTestHelper.ApplicationUrlMock);

            internsRepositoryMock
                .Setup(i => i.GetDetailsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(internDetailsResponseMock);

            internsRepositoryMock
                .Setup(i => i.ExistsByPersonalEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            identityRepositoryMock
                .Setup(i => i.SendUserInviteAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(identitySummaryResponseMock);

            AddInternIdentityRepoRequest addInternIdentityRepoRequest = null!;

            internsRepositoryMock
                .Setup(i => i.AddIdentityAsync(It.IsAny<AddInternIdentityRepoRequest>()))
                .Callback((AddInternIdentityRepoRequest request) => addInternIdentityRepoRequest = request)
                .ReturnsAsync(internSummaryResponseMock2);

            //Act
            var internSummaryResponse = await internsService.InviteAsync(inviteInternRequest);

            //Assert
            Assert.Equal(identitySummaryResponseMock.DisplayName, addInternIdentityRepoRequest.DisplayName);
            Assert.Equal(identitySummaryResponseMock.Email, addInternIdentityRepoRequest.WorkEmail);

            Assert.Equal(internSummaryResponseMock2.DisplayName, internSummaryResponse.DisplayName);
            Assert.Equal(internSummaryResponseMock2.Email, internSummaryResponse.Email);
        }

        #endregion

    }
}
