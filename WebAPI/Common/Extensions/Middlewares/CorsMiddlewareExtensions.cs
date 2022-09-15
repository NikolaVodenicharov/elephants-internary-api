namespace WebAPI.Common.Extensions.Middlewares
{
    public static class CorsMiddlewareExtensions
    {
        public static void CustomizeCorsPolicy(this IServiceCollection services, ApplicationSettings appSettings)
        {
            services.AddCors(options => 
            {
                options.AddDefaultPolicy(policy => policy
                    .WithOrigins(appSettings.CorsAllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
            });
        }
    }
}