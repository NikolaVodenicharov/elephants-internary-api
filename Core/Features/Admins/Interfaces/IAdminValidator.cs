using Core.Features.Admins.RequestModels;

namespace Core.Features.Admins.Interfaces
{
    public interface IAdminValidator
    {
        Task ValidateAndThrowAsync(CreateAdminRequest createAdmin);
        
        Task ValidateAndThrowAsync(AddMentorRoleRequest addMentorRole);
    }
}