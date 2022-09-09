using Core.Features.Specialities.RequestModels;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialties.Entities;

namespace Core.Features.Specialities.Support
{
    public static class SpecialitiesMappingExtensions
    {
        public static Speciality ToSpeciality(this CreateSpecialityRequest createSpecialityRequest)
        {
            var speciality = new Speciality()
            {
                Name = createSpecialityRequest.Name
            };

            return speciality;
        }

        public static Speciality ToSpeciality(this UpdateSpecialityRequest updateSpecialityRequest)
        {
            var speciality = new Speciality()
            {
                Id = updateSpecialityRequest.Id,
                Name = updateSpecialityRequest.Name
            };

            return speciality;
        }

        public static SpecialitySummaryResponse ToSpecialitySummaryResponse(this Speciality Speciality)
        {
            var specialitySummary = new SpecialitySummaryResponse(Speciality.Id, Speciality.Name);

            return specialitySummary;
        }
    }
}
