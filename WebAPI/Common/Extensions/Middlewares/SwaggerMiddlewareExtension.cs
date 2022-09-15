using Microsoft.OpenApi.Models;

namespace WebAPI.Common.Extensions.Middlewares
{
    public static class SwaggerMiddlewareExtension
    {
        private const string openAPISchemeName = "oauth2";

        public static void CustomizeSwagger(this IServiceCollection services, ApplicationSettings appSettings)
        {
            services.AddSwaggerGen(configuration => 
            {
                configuration.AddSecurityDefinition(openAPISchemeName, new OpenApiSecurityScheme()
                {
                    Name = "Oauth2.0 Auth Code with PKCE",
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(appSettings.AuthorizationUrl),
                            TokenUrl = new Uri(appSettings.TokenUrl),
                            Scopes = new Dictionary<string, string>
                            {
                                { appSettings.ApiScope, appSettings.ApiScopeDescription }
                            }
                        }
                    }
                });

                configuration.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = openAPISchemeName}
                        },
                        new[] { appSettings.ApiScope }
                    }
                });

            });
        }
    }
}