using Core.Features.Persons.Entities;

namespace Core.Features.Persons.ResponseModels
{
    public record PersonRolesSummaryResponse(
        Guid Id,
        ICollection<RoleId> Roles
    );
}