using Core.Features.Identity.Interfaces;
using Core.Features.Identity.ResponseModels;
using Core.Features.Users.Interfaces;
using Core.Features.Users.RequestModels;
using Core.Features.Users.ResponseModels;
using Core.Features.Users.Support;
using FluentValidation;

namespace Core.Features.Users
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository usersRepository;
        private readonly IIdentityRepository identityRepository;
        private readonly IValidator<CreateUserRequest> createUserRequestValidator;

        public UsersService(
            IUsersRepository usersRepository, 
            IIdentityRepository identityRepository, 
            IValidator<CreateUserRequest> createUserRequestValidator)
        {
            this.usersRepository = usersRepository;
            this.identityRepository = identityRepository;
            this.createUserRequestValidator = createUserRequestValidator;
        }

        public async Task<UserSummaryResponse> CreateAsync(CreateUserRequest request)
        {
            await createUserRequestValidator.ValidateAndThrowAsync(request);

            var user = request.ToUser();

            var createdUser = await usersRepository.AddAsync(user);

            return createdUser.ToUserSummary();
        }
        
        public async Task<IdentitySummaryResponse> SendInvitationByEmailAsync(string email, string applicationUrl)
        {
            var invitationSummary = await identityRepository.SendUserInviteAsync(email, applicationUrl);

            return invitationSummary;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var userExists = await usersRepository.ExistsByEmailAsync(email);

            return userExists;
        }
    }
}