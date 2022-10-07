namespace WebAPI.Features.Mentors.ApiRequestModels
{
    public record CreateMentorApiRequest(string Email, IEnumerable<Guid> SpecialityIds);
}