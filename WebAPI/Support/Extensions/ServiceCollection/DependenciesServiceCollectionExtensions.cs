using Core.Features.Campaigns;
using Core.Features.Campaigns.Interfaces;
using Infrastructure.Features.Campaigns;

namespace WebAPI.Support.Extensions.ServiceCollection
{
    public static class DependenciesServiceCollectionExtensions
    {
        public static void CustomizeDependencies(this IServiceCollection services)
        {
            services.AddScoped<ICampaignsService, CampaignsService>();
            services.AddScoped<ICampaignsRepository, CampaignsRepository>();
        }
    }
}
