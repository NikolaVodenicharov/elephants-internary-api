using Core.Features.Specialties.Entities;

namespace Core.Features.Mentors.RequestModels
{
    public record UpdateMentorRepoRequest(
        Guid Id,
        ICollection<Speciality> Specialities);
}
