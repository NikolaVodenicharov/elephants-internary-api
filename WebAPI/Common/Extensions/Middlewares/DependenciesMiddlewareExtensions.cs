using Core.Features.Campaigns;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.Support;
using FluentValidation;
using Infrastructure.Features.Campaigns;

namespace WebAPI.Common.Extensions.Middlewares
{
    public static class DependenciesMiddlewareExtensions
    {
        public static void CustomizeDependencies(this IServiceCollection services)
        {
            RegisterCampaignDependencies(services);
        }

        private static void RegisterCampaignDependencies(IServiceCollection services)
        {
            services.AddTransient<ICampaignsService, CampaignsService>();

            services.AddTransient<ICampaignsRepository, CampaignsRepository>();

            services.AddTransient<IValidator<CreateCampaign>, CreateCampaignValidator>();

            services.AddTransient<IValidator<UpdateCampaign>, UpdateCampaignValidator>();
        }
    }
}
