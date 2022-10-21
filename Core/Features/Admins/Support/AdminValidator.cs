using Core.Features.Admins.Interfaces;
using Core.Features.Admins.RequestModels;
using FluentValidation;

namespace Core.Features.Admins.Support
{
    public class AdminValidator : IAdminValidator
    {
        private readonly IValidator<CreateAdminRequest> createAdminValidator;
        private readonly IValidator<AddMentorRoleRequest> addMentorRoleValidator;

        public AdminValidator(
            IValidator<CreateAdminRequest> createAdminValidator,
            IValidator<AddMentorRoleRequest> addMentorRoleValidator)
        {
            this.createAdminValidator = createAdminValidator;
            this.addMentorRoleValidator = addMentorRoleValidator;
        }

        public async Task ValidateAndThrowAsync(CreateAdminRequest createAdmin)
        {
            await createAdminValidator.ValidateAndThrowAsync(createAdmin);
        }

        public async Task ValidateAndThrowAsync(AddMentorRoleRequest addMentorRole)
        {
            await addMentorRoleValidator.ValidateAndThrowAsync(addMentorRole);
        }
    }
}