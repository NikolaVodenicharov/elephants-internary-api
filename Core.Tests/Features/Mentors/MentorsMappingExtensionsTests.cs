using Core.Features.Mentors.Entities;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.Support;
using Core.Features.Specialties.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Core.Tests.Features.Mentors
{
    public class MentorsMappingExtensionsTests
    {
        private Guid id = Guid.NewGuid();
        private string firstName = "First";
        private string lastName = "Last";
        private string email = "first.last@test.co.uk";
        private List<Speciality> specialities;
        private List<Guid> specialityIds;

        public MentorsMappingExtensionsTests()
        {
            var speciality = new Speciality() { 
                Id = Guid.NewGuid(),
                Name = "Backend"
            };

            specialities = new List<Speciality>() { speciality };

            specialityIds = new List<Guid>() { speciality.Id };
        }

        [Fact]
        public void CreateMentorRequest_ToMentor_CreateCorrectObject()
        {
            //Arrange
            var request = new CreateMentorRequest(firstName, lastName, email, specialityIds);

            //Act
            var mentor = request.ToMentor();

            //Assert
            Assert.Equal(firstName, mentor.FirstName);
            Assert.Equal(lastName, mentor.LastName);
            Assert.Equal(email, mentor.Email);
        }

        [Fact]
        public void Mentor_ToMentorSummaryResponse_CreateCorrectObject()
        {
            //Arrange
            var mentor = new Mentor()
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Specialities = specialities
            };

            //Act
            var response = mentor.ToMentorSummaryResponse();

            //Assert
            Assert.Equal(id, response.Id);
            Assert.Equal(firstName, response.FirstName);
            Assert.Equal(lastName, response.LastName);
            Assert.Equal(email, response.Email);
        }

        [Fact]
        public void IEnumerableOfMentors_ToMentorSummaryResponses_CreateCorrectObject()
        {
            //Arrange
            var mentorList = new List<Mentor>() {
                new Mentor()
                {
                    Id = id,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Specialities = specialities
                }
            };

            //Act
            var responseList = mentorList.ToMentorSummaryResponses().ToList();

            //Assert
            Assert.Equal(mentorList.Count, responseList.Count);
        }
    }
}
