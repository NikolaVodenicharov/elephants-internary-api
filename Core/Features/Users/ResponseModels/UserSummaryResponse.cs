using Core.Features.Users.Entities;

namespace Core.Features.Users.ResponseModels
{
    public record UserSummaryResponse(
        Guid Id,
        string Email,
        RoleEnum Role
    );
}