using Core.Features.Users.Entities;
using Core.Features.Users.RequestModels;
using Core.Features.Users.ResponseModels;

namespace Core.Features.Users.Support
{
    public static class UsersMappingExtensions
    {
        public static User ToUser(this CreateUserRequest request)
        {
            var user = new User()
            {
                Email = request.Email,
                RoleId = request.RoleId,
                MentorId = request.MentorId
            };

            return user;
        }

        public static UserSummaryResponse ToUserSummary(this User user)
        {
            var userSummaryResponse = new UserSummaryResponse(
                user.Id,
                user.Email,
                user.RoleId
            );

            return userSummaryResponse;
        }
    }
}