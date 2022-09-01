namespace WebAPI.Common.Extensions.Middlewares
{
    public static class RoutingMiddlewareExtensions
    {
        public static void CustomizeRouting(this IServiceCollection services)
        {
            services
                .AddRouting(options => options.LowercaseUrls = true);
        }
    }
}
