using Core.Common;
using Core.Common.Exceptions;
using Core.Features.Persons.Entities;
using Core.Features.Persons.ResponseModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace WebAPI.Common.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IEnumerable<RoleId> roles;

        public CustomAuthorizeAttribute(params RoleId[] roles)
        {
            this.roles = roles ?? Array.Empty<RoleId>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var contextUser = context.HttpContext.Items["User"];
            
            Guard.EnsureNotNullAuthorization(contextUser, "User");

            var user = (PersonRolesSummaryResponse)contextUser;

            if ( user == null || 
                (roles.Any() && !roles.Any(r => user.Roles.Contains(r))) )
            {
                var message = $"Unauthorized. Required Role was not found";

                throw new CoreException(message, HttpStatusCode.Unauthorized);
            }
        }
    } 
}