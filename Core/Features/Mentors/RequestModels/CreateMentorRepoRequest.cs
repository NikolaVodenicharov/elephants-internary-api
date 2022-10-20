using Core.Features.Specialties.Entities;

namespace Core.Features.Mentors.RequestModels
{
    public record CreateMentorRepoRequest(
        string DisplayName,
        string WorkEmail,
        ICollection<Speciality> Specialities);
}
