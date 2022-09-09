using Core.Features.Campaigns;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.Support;
using Core.Features.Specialities;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.RequestModels;
using Core.Features.Specialities.Support;
using FluentValidation;
using Infrastructure.Features.Campaigns;
using Infrastructure.Features.Specialities;

namespace WebAPI.Common.Extensions.Middlewares
{
    public static class DependenciesMiddlewareExtensions
    {
        public static void CustomizeDependencies(this IServiceCollection services)
        {
            RegisterCampaignDependencies(services);
            RegisterSpecialityDependencies(services);
        }

        private static void RegisterCampaignDependencies(IServiceCollection services)
        {
            services.AddTransient<ICampaignsService, CampaignsService>();
            services.AddTransient<ICampaignsRepository, CampaignsRepository>();
            services.AddTransient<IValidator<CreateCampaign>, CreateCampaignValidator>();
            services.AddTransient<IValidator<UpdateCampaign>, UpdateCampaignValidator>();
            services.AddTransient<IValidator<PaginationFilterRequest>, PaginationFilterRequestValidator>();
        }

        private static void RegisterSpecialityDependencies(IServiceCollection services)
        {
            services.AddTransient<ISpecialitiesService, SpecialitiesService>();
            services.AddTransient<ISpecialitiesRepository, SpecialitiesRepository>();
            services.AddTransient<IValidator<CreateSpecialityRequest>, CreateSpecialityValidator>();
            services.AddTransient<IValidator<UpdateSpecialityRequest>, UpdateSpecialityValidator>();
        }
    }
}
