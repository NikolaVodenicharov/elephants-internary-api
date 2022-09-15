using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Common.Extensions
{
    public static class DatabaseSetupExtension
    {
        public static void ConfigureAndMigrateDatabase(this IServiceCollection services, IConfiguration builderConfiguration)
        {
            services.AddDbContextPool<InternaryContext>(options =>
                options.UseSqlServer(builderConfiguration.GetConnectionString("InternaryDatabaseConnection")));
            
            var internaryContext = services
                .BuildServiceProvider()
                .GetRequiredService<InternaryContext>();
            
            internaryContext.Database.Migrate();
        }
    }
}