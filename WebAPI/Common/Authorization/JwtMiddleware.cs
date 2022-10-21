using Core.Common;
using Core.Features.Persons.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.Common.SettingsModels;

namespace WebAPI.Common.Authorization
{
    internal sealed class JwtMiddleware
    {
        private readonly RequestDelegate next;

        public JwtMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context, IPersonsService personsService, IOptions<ApplicationSettings> applicationSettings)
        {
            var bearerToken = context
                .Request
                .Headers[HeaderNames.Authorization]
                .FirstOrDefault()?
                .Split(" ")
                .Last();
            
            Guard.EnsureNotNullAuthorization(bearerToken, "Token");
            
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var token = tokenHandler.ReadJwtToken(bearerToken);

            var userEmail = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);

            Guard.EnsureNotNullAuthorization(userEmail, "User Email");

            var userName = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name);

            var userAzureGroups = token.Claims
                .Where(c => c.Type == "groups")
                .Select(c => c.Value.ToString());

            var user = await personsService.GetUserRolesByEmailAsync(userEmail.Value);

            var adminGroup = applicationSettings.Value.AdministratorsGroup;
            
            var isUserAdmin = userAzureGroups.Contains(adminGroup);

            if (user == null && isUserAdmin)
            {
                user = await personsService.CreatePersonAsAdminAsync(userEmail.Value, userName!.Value);
            }

            Guard.EnsureNotNullAuthorization(user, "User");
            
            context.Items["User"] = user;

            await next(context);
        }
    }
}