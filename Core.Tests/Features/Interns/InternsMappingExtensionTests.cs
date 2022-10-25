using Core.Features.Campaigns.Entities;
using Core.Features.Interns.Entities;
using Core.Features.Interns.Support;
using Core.Features.Specialties.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Core.Tests.Features.Interns
{
    public class InternsMappingExtensionTests
    {
        [Fact]
        public void State_ToStateResponse_ShouldCreateCorrectObject()
        {
            //Arrange
            var state = new State()
            {
                StatusId = StatusId.Candidate,
                Created = DateTime.UtcNow,
                Justification = "Lorem ipsum."
            };

            //Act
            var stateResponse = state.ToStateResponse();

            //Assert
            Assert.Equal(state.StatusId.ToString(), stateResponse.Status);
            Assert.Equal(state.Created, stateResponse.Created);
            Assert.Equal(state.Justification, stateResponse.Justification);
        }

        [Fact]
        public void States_ToInternStatesResponse_ShouldCreateCorrectObject()
        {
            //Arrange
            var state1 = new State()
            {
                StatusId = StatusId.Candidate,
                Created = DateTime.UtcNow,
                Justification = "Lorem ipsum."
            };

            var state2 = new State()
            {
                StatusId = StatusId.Rejected,
                Created = DateTime.UtcNow.AddDays(1),
                Justification = "Dolor sit amet."
            };

            var states = new List<State>() { state1, state2 };

            //Act
            var internStatesResponse = states.ToInternStateResponses();

            //Assert
            Assert.Equal(states.Count, internStatesResponse.Count());
        }

        [Fact]
        public void InternCampaign_ToInternCampaignResponse_ShouldCreateCorrectObject()
        {
            //Arrange
            var campaign = new Campaign()
            {
                Id = Guid.NewGuid(),
                Name = "Campaign 2022",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(50),
                IsActive = true
            };

            var speciality = new Speciality
            {
                Id = Guid.NewGuid(),
                Name = "SpecialityName"
            };

            var state = new State()
            {
                StatusId = StatusId.Candidate,
                Created = DateTime.UtcNow,
                Justification = "Lorem ipsum."
            };

            var internCamapign = new InternCampaign()
            {
                Campaign = campaign,
                Speciality = speciality,
                States = new List<State>() { state }
            };

            //Act
            var internCamapignResponse = internCamapign.ToInternCampaignResponse();

            //Assert
            Assert.NotNull(internCamapignResponse.Campaign);
            Assert.NotNull(internCamapignResponse.Speciality);
            Assert.NotNull(internCamapignResponse.StateResponse);
        }

        [Fact]
        public void StatusResponse_ToStatusResponse_ShouldCreateCorrectObject()
        {
            //Arrange
            var status = new Status()
            {
                StatusId = StatusId.Candidate,
                Name = StatusId.Candidate.ToString()
            };

            //Act
            var statusResponse = status.ToStatusResponse();

            //Assert
            Assert.Equal((int)status.StatusId, statusResponse.Id);
            Assert.Equal(status.Name, statusResponse.Name);
        }
    }
}
