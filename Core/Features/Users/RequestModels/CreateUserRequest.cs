using Core.Features.Users.Entities;

namespace Core.Features.Users.RequestModels
{
    public record CreateUserRequest(
        string Email,
        RoleEnum RoleId,
        Guid? MentorId
    );
}