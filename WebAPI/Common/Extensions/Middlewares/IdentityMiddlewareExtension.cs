using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using System.IdentityModel.Tokens.Jwt;

namespace WebAPI.Common.Extensions.Middlewares
{
    public static class IdentityMiddlewareExtension
    {
        public static void CustomizeIdentity(this IServiceCollection services, IConfiguration builderConfiguration)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builderConfiguration);
        }
    }
}