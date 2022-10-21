namespace Core.Features.Admins.RequestModels
{
    public record AddMentorRoleRequest(Guid Id, IEnumerable<Guid> SpecialityIds);
}