namespace Core.Features.Mentors.RequestModels
{
    public record CreateMentorRequest(string Email, IEnumerable<Guid> SpecialityIds, string ApplicationUrl);
}
