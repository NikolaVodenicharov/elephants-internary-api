namespace WebAPI.Common.Extensions
{
    public static class ConfigureServicesExtension
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection(ApplicationSettings.SectionName));
            builder.Services.Configure<AzureSettings>(builder.Configuration.GetSection(AzureSettings.SectionName));
            builder.Services.Configure<GraphSettings>(builder.Configuration.GetSection(GraphSettings.SectionName));

            var environment = "Default";

            if(builder.Environment.IsProduction())
            {
                environment = builder.Environment.EnvironmentName;
            }

            var configuration = builder.Configuration.GetSection(InvitationUrlSettings.SectionName).GetSection(environment);

            builder.Services.Configure<InvitationUrlSettings>(configuration);
        }
    }
}