using Core.Features.Specialties.Entities;

namespace Core.Features.Mentors.RequestModels
{
    public record AddMentorRoleRepoRequest(Guid PersonId, ICollection<Speciality> Specialities);
}