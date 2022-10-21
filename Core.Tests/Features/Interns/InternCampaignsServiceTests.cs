using Core.Common.Exceptions;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Interns;
using Core.Features.Interns.Entities;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;
using Core.Features.Interns.Support;
using Core.Features.Persons.Entities;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialties.Entities;
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
    public class InternCampaignsServiceTests
    {
        private readonly Guid internId = Guid.NewGuid();
        private readonly Guid campaignId = Guid.NewGuid();
        private readonly Guid specialityId = Guid.NewGuid();
        private readonly string justification = "Lorem ipsum.";

        private Person internMock = null!;
        private Campaign campaignMock = null!;
        private Speciality specialityMock = null!;
        private Speciality specialityMock2 = null!;
        private AddInternCampaignRequest addInternCampaignRequestMock = null!;
        private UpdateInternCampaignRequest updateInternCampaignRequestMock = null!;
        private AddStateRequest addStateRequestMock = null!;
        private InternCampaign internCampaignWithStatesMock = null!;
        private State stateMock = null!;
        private InternSummaryResponse internSummaryResponseMock = null!;

        private readonly InternCampaignsService internCampaignsService;
        private readonly Mock<IInternsRepository> internsRepositoryMock;
        private readonly Mock<ICampaignsRepository> campaignsRepositoryMock;
        private readonly Mock<ISpecialitiesRepository> specialitiesRepositoryMock;
        private readonly Mock<ILogger<InternCampaignsService>> internCampaignsServiceLoggerMock;

        public InternCampaignsServiceTests()
        {
            internsRepositoryMock = new Mock<IInternsRepository>();
            campaignsRepositoryMock = new Mock<ICampaignsRepository>();
            specialitiesRepositoryMock = new Mock<ISpecialitiesRepository>();
            internCampaignsServiceLoggerMock = new Mock<ILogger<InternCampaignsService>>();

            var createInternRequestValidator = new CreateInternRequestValidator();
            var updateInternRequestValidator = new UpdateInternRequestValidator();
            var addInternCampaignequestValidator = new AddInternCampaignRequestValidator();
            var updateInternCampaignRequestValidator = new UpdateInternCampaignRequestValidator();
            var addStateRequestValidator = new AddStateRequestValidator();

            var internValidator = new InternValidator(
                createInternRequestValidator, updateInternRequestValidator,
                addInternCampaignequestValidator,
                updateInternCampaignRequestValidator, addStateRequestValidator);

            internCampaignsService = new InternCampaignsService(
                internsRepositoryMock.Object,
                campaignsRepositoryMock.Object,
                specialitiesRepositoryMock.Object,
                internCampaignsServiceLoggerMock.Object,
                internValidator);

            InitializeMockModels();
        }

        public static IEnumerable<object[]> AddStateInvalidJustification =>
            new List<object[]>
            {
                    new object[] { string.Empty },
                    new object[] { TestHelper.GenerateString(InternValidationConstants.JustificationMaxLength + 1) },

            };

        #region AddInternCampaignAsync

        [Fact]
        public async Task AddInternCampaignAsync_WhenInternIdIsEmpty_ShouldThrowException()
        {
            //Arrange
            var addInternCampaignRequest = new AddInternCampaignRequest(
                Guid.Empty,
                campaignId,
                specialityId,
                justification);

            //Act
            var action = async () => await internCampaignsService.AddInternCampaignAsync(addInternCampaignRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task AddInternCampaignAsync_WhenCampaignIdIsEmpty_ShouldThrowException()
        {
            var addInternCampaignRequest = new AddInternCampaignRequest(
                internId,
                Guid.Empty,
                specialityId,
                justification);

            //Act
            var action = async () => await internCampaignsService.AddInternCampaignAsync(addInternCampaignRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task AddInternCampaignAsync_WhenSpecialityIdIsEmpty_ShouldThrowException()
        {
            var addInternCampaignRequest = new AddInternCampaignRequest(
                internId,
                campaignId,
                Guid.Empty,
                justification);

            //Act
            var action = async () => await internCampaignsService.AddInternCampaignAsync(addInternCampaignRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task AddInternCampaignAsync_WhenJustificationIsInvalid_ShouldThrowException()
        {
            var invalidJustification = TestHelper.GenerateString(InternValidationConstants.JustificationMaxLength + 1);

            var addInternCampaignRequest = new AddInternCampaignRequest(
                internId,
                campaignId,
                specialityId,
                invalidJustification);

            //Act
            var action = async () => await internCampaignsService.AddInternCampaignAsync(addInternCampaignRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task AddInternCampaignAsync_WhenInternIsAlreadyInThatCampaing_ShouldThrowException()
        {
            campaignsRepositoryMock
                .Setup(c => c.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaignMock);

            specialitiesRepositoryMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(specialityMock);

            internsRepositoryMock
                .Setup(i => i.GetInternCampaignByIdsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new InternCampaign());

            //Act
            var action = async () => await internCampaignsService.AddInternCampaignAsync(addInternCampaignRequestMock);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task AddInternCampaignAsync_WhenAllValid_ShouldReturnCorrectObject()
        {
            campaignsRepositoryMock
                .Setup(c => c.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaignMock);

            specialitiesRepositoryMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(specialityMock);

            internsRepositoryMock
                .Setup(i => i.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(internSummaryResponseMock);

            //Act
            var internCampaignResponse = await internCampaignsService.AddInternCampaignAsync(addInternCampaignRequestMock);

            //Assert
            Assert.Equal(campaignMock.Id, internCampaignResponse.Campaign.Id);
            Assert.Equal(specialityMock.Id, internCampaignResponse.Speciality.Id);
            Assert.NotNull(internCampaignResponse.StateResponse);
        }

        #endregion

        #region UpdateInternCampaignAsync

        [Fact]
        public async Task UpdateInternCampaignAsync_WhenInternIdIsEmpty_ShouldThrowException()
        {
            //Arrange
            var updateInternCampaignRequest = new UpdateInternCampaignRequest(
                Guid.Empty,
                campaignId,
                specialityId);

            //Act
            var action = async () => await internCampaignsService.UpdateInternCampaignAsync(updateInternCampaignRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateInternCampaignAsync_WhenCampaignIdIsEmpty_ShouldThrowException()
        {
            var updateInternCampaignRequest = new UpdateInternCampaignRequest(
                internId,
                Guid.Empty,
                specialityId);

            //Act
            var action = async () => await internCampaignsService.UpdateInternCampaignAsync(updateInternCampaignRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateInternCampaignAsync_WhenSpecialityIdIsEmpty_ShouldThrowException()
        {
            var updateInternCampaignRequest = new UpdateInternCampaignRequest(
                internId,
                campaignId,
                Guid.Empty);

            //Act
            var action = async () => await internCampaignsService.UpdateInternCampaignAsync(updateInternCampaignRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task UpdateInternCampaignAsync_WhenInternCampaignIdsNotFound_ShouldThrowException()
        {
            var updateInternCampaignRequest = new UpdateInternCampaignRequest(
                internId,
                campaignId,
                specialityId);

            specialitiesRepositoryMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(specialityMock);

            //Act
            var action = async () => await internCampaignsService.UpdateInternCampaignAsync(updateInternCampaignRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task UpdateInternCampaignAsync_WhenSpecialityIdNotFound_ShouldThrowException()
        {
            var updateInternCampaignRequest = new UpdateInternCampaignRequest(
                internId,
                campaignId,
                Guid.NewGuid());

            internsRepositoryMock
                .Setup(s => s.GetInternCampaignByIdsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new InternCampaign());

            //Act
            var action = async () => await internCampaignsService.UpdateInternCampaignAsync(updateInternCampaignRequest);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task UpdateInternCampaignAsync_WhenSpecialityIsNotChanged_ShouldNotGetSpecialityObject()
        {
            internsRepositoryMock
                .Setup(s => s.GetInternCampaignByIdsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(internCampaignWithStatesMock);

            var updateInternCampaignRequest = new UpdateInternCampaignRequest(
                internId,
                campaignId,
                internCampaignWithStatesMock.SpecialityId);

            //Act
            await internCampaignsService.UpdateInternCampaignAsync(updateInternCampaignRequest);

            //Assert
            specialitiesRepositoryMock.Verify(s => s.GetByIdAsync(It.IsAny<Guid>()), Times.Never());
        }

        [Fact]
        public async Task UpdateInternCampaignAsync_WhenSpecialityIsChanged_ShouldPassCorrectObject()
        {
            //Arrange
            internsRepositoryMock
                .Setup(i => i.GetInternCampaignByIdsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(internCampaignWithStatesMock);

            InternCampaign internCampaignCallback = null!;

            internsRepositoryMock
                .Setup(i => i.UpdateInternCampaignAsync(It.IsAny<InternCampaign>()))
                .Callback((InternCampaign ic) => internCampaignCallback = ic);

            specialitiesRepositoryMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(specialityMock2);

            //Act
            await internCampaignsService.UpdateInternCampaignAsync(updateInternCampaignRequestMock);

            //Assert
            Assert.Equal(internCampaignWithStatesMock.Campaign.Name, internCampaignCallback.Campaign.Name);
            Assert.Equal(specialityMock2.Name, internCampaignCallback.Speciality.Name);
        }

        #endregion

        #region CreateInternCampaignAsync

        [Fact]
        public async Task CreateInternCampaignAsync_WhenCampaignIdIsNotFound_ShouldThrowException()
        {
            //Arrange
            specialitiesRepositoryMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(specialityMock);

            //Act
            var action = async () => await internCampaignsService.CreateInternCampaignAsync(Guid.NewGuid(), specialityId, justification);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task CreateInternCampaignAsync_WhenSpecialityIdIsNotFound_ShouldThrowException()
        {
            //Arrange
            campaignsRepositoryMock
                .Setup(c => c.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaignMock);

            //Act
            var action = async () => await internCampaignsService.CreateInternCampaignAsync(campaignId, Guid.NewGuid(), justification);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task CreateInternCampaignAsync_WhenIdsAreFound_ShouldReturnCorrectObject()
        {
            //Arrange
            campaignsRepositoryMock
                .Setup(c => c.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaignMock);

            specialitiesRepositoryMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(specialityMock);

            //Act
            var internCampaign = await internCampaignsService.CreateInternCampaignAsync(campaignMock.Id, specialityMock.Id, justification);

            //Assert
            Assert.Equal(campaignMock.Id, internCampaign.Campaign.Id);
            Assert.Equal(specialityMock.Id, internCampaign.Speciality.Id);
            Assert.Equal(StatusEnum.Candidate, internCampaign.States.First().StatusId);
        }

        #endregion

        #region AddStateAsync

        [Fact]
        public async Task AddStateAsync_WhenInternIdIsEmpty_ShouldThrowExceptionAsync()
        {
            //Arrange
            var addStateRequest = new AddStateRequest(
                Guid.Empty,
                campaignId,
                StatusEnum.Rejected,
                justification);

            //Act
            var action = async () => await internCampaignsService.AddStateAsync(addStateRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task AddStateAsync_WhenCampaignIdIsEmpty_ShouldThrowException()
        {
            //Arrange
            var addStateRequest = new AddStateRequest(
                internId,
                Guid.Empty,
                StatusEnum.Hired,
                justification);

            //Act
            var action = async () => await internCampaignsService.AddStateAsync(addStateRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Theory]
        [MemberData(nameof(AddStateInvalidJustification))]
        public async Task AddStateAsync_WhenJustificationIsInvalid_ShouldThrowException(string justification)
        {
            //Arrange
            var addStateRequest = new AddStateRequest(
                internId,
                campaignId,
                StatusEnum.RejectedToStart,
                justification);

            //Act
            var action = async () => await internCampaignsService.AddStateAsync(addStateRequest);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(action);
        }

        [Fact]
        public async Task AddStateAsync_WhenInternCampaignIdsNotFound_ShouldThrowException()
        {
            //Act
            var action = async () => await internCampaignsService.AddStateAsync(addStateRequestMock);

            //Assert
            await Assert.ThrowsAsync<CoreException>(action);
        }

        [Fact]
        public async Task AddStateAsync_WhenInternCampaingContainsOtherStates_ShouldAddToExistingCollection()
        {
            //Arrange
            internsRepositoryMock
                .Setup(s => s.GetInternCampaignByIdsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(internCampaignWithStatesMock);

            //Act
            await internCampaignsService.AddStateAsync(addStateRequestMock);

            //Assert
            Assert.Equal(2, internCampaignWithStatesMock.States.Count);
        }

        #endregion

        #region GetAllStatusAsync

        [Fact]
        public async Task GetAllStatusAsync_ShouldReturnCorrectNumberOfElements()
        {
            //Arrange
            var statusResponse1 = new StatusResponse((int)StatusEnum.Candidate, StatusEnum.Candidate.ToString());
            var statusResponse2 = new StatusResponse((int)StatusEnum.RejectedToStart, StatusEnum.RejectedToStart.ToString());

            var statusResponseCollectionMock = new List<StatusResponse>() { statusResponse1, statusResponse2 };

            internsRepositoryMock
                .Setup(i => i.GetAllStatusAsync())
                .ReturnsAsync(statusResponseCollectionMock);

            //Act
            var statusResponseCollection = await internCampaignsService.GetAllStatusAsync();

            //Assert
            Assert.Equal(2, statusResponseCollection.Count());
        }

        #endregion

        private void InitializeMockModels()
        {
            internMock = new Person()
            {
                Id = internId,
                FirstName = "FirstName",
                LastName = "LastName",
                PersonalEmail = "FirstLast@gmail.com"
            };

            campaignMock = new Campaign()
            {
                Id = campaignId,
                Name = "CampaignName",
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(50),
                IsActive = true
            };

            specialityMock = new Speciality()
            {
                Id = specialityId,
                Name = "SpecialityName"
            };

            specialityMock2 = new Speciality()
            {
                Id = Guid.NewGuid(),
                Name = "UpdatedName"
            };

            addInternCampaignRequestMock = new AddInternCampaignRequest(
                 internId,
                 campaignId,
                 specialityId,
                 justification);

            updateInternCampaignRequestMock = new UpdateInternCampaignRequest(
                internId,
                campaignId,
                specialityMock2.Id);

            addStateRequestMock = new AddStateRequest(
                internId,
                campaignId,
                StatusEnum.Candidate,
                justification);

            stateMock = new State()
            {
                Justification = justification,
                Created = DateTime.UtcNow,
                StatusId = StatusEnum.Rejected,
                InternId = internId,
                CampaignId = campaignId
            };

            internCampaignWithStatesMock = new InternCampaign()
            {
                Person = internMock,
                Campaign = campaignMock,
                Speciality = specialityMock,
                SpecialityId = specialityMock.Id,
                States = new List<State>() { stateMock }
            };

            internSummaryResponseMock = new InternSummaryResponse(
                internId,
                "FirstName LastName",
                "FirstLast@gmail.com");
        }
    }
}
