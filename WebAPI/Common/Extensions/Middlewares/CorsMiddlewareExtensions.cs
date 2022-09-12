namespace WebAPI.Common.Extensions.Middlewares
{
    public static class CorsMiddlewareExtensions
    {
        public static void CustomizeCorsPolicy(this IServiceCollection services, IConfiguration builderConfiguration)
        {
            var allowedOrigins = builderConfiguration.GetSection("CorsAllowedOrigins").Get<string[]>();

            services.AddCors(options => 
            {
                options.AddDefaultPolicy(policy => policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
            });
        }
    }
}