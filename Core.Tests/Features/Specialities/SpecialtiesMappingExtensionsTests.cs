using Core.Features.Specialities.RequestModels;
using Core.Features.Specialities.Support;
using Core.Features.Specialties.Entities;
using System;
using Xunit;

namespace Core.Tests.Features.Specialities
{
    public class SpecialtiesMappingExtensionsTests
    {
        private readonly Guid Id = Guid.NewGuid();
        private readonly string name = "SpecialityTestName";

        [Fact]
        public void CreateSpeciality_ToSpecialty_ShouldCreateCorrectObject()
        {
            //Arrange
            var createSpecialityRequest = new CreateSpecialityRequest(name);

            //Act
            var specialty = createSpecialityRequest.ToSpeciality();

            //Assert
            Assert.Equal(name, specialty.Name);
        }

        [Fact]
        public void UpdateSpecialityRequest_ToSpecialty_ShouldCreateCorrectObject()
        {
            //Arrange
            var updateSpecialityRequest = new UpdateSpecialityRequest(Guid.NewGuid(), name);

            //Act
            var speciality = updateSpecialityRequest.ToSpeciality();

            //Assert
            Assert.Equal(updateSpecialityRequest.Id, speciality.Id);
            Assert.Equal(updateSpecialityRequest.Name, speciality.Name);
        }

        [Fact]
        public void Specialty_ToSpecialtySummaryResponse_ShouldCreateCorrectObject()
        {
            //Arrange
            var speciality = new Speciality()
            {
                Id = Id,
                Name = name,
            };

            //Act
            var specialitySummary = speciality.ToSpecialitySummaryResponse();

            //Assert
            Assert.Equal(Id, specialitySummary.Id);
            Assert.Equal(name, specialitySummary.Name);

        }
    }
}
