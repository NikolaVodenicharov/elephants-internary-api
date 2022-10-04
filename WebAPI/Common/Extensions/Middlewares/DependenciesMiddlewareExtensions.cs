using Core.Common.Pagination;
using Core.Features.Campaigns;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.Support;
using Core.Features.Mentors;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.Support;
using Core.Features.Specialities;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.RequestModels;
using Core.Features.Specialities.Support;
using Core.Features.LearningTopics;
using Core.Features.LearningTopics.Interfaces;
using Core.Features.LearningTopics.RequestModels;
using Core.Features.LearningTopics.Support;
using Core.Features.Interns;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.Support;
using FluentValidation;
using Infrastructure.Features.Campaigns;
using Infrastructure.Features.Mentors;
using Infrastructure.Features.Specialities;
using Infrastructure.Features.LearningTopics;
using Infrastructure.Features.Interns;

namespace WebAPI.Common.Extensions.Middlewares
{
    public static class DependenciesMiddlewareExtensions
    {
        public static void CustomizeDependencies(this IServiceCollection services)
        {
            RegisterCommonDependencies(services);
            RegisterCampaignDependencies(services);
            RegisterMentorDependencies(services);
            RegisterSpecialityDependencies(services);
            RegisterLearningTopicsDependencies(services);
            RegisterInternDependencies(services);
        }

        private static void RegisterCommonDependencies(IServiceCollection services)
        {
            services.AddTransient<IValidator<PaginationRequest>, PaginationRequestValidator>();
        }

        private static void RegisterCampaignDependencies(IServiceCollection services)
        {
            services.AddTransient<ICampaignsService, CampaignsService>();
            services.AddTransient<ICampaignsRepository, CampaignsRepository>();
            services.AddTransient<IValidator<CreateCampaignRequest>, CreateCampaignRequestValidator>();
            services.AddTransient<IValidator<UpdateCampaignRequest>, UpdateCampaignRequestValidator>();
            
        }

        private static void RegisterMentorDependencies(IServiceCollection services)
        {
            services.AddTransient<IMentorsService, MentorsService>();
            services.AddTransient<IMentorsRepository, MentorsRepository>();
            services.AddTransient<IValidator<CreateMentorRequest>, CreateMentorRequestValidator>();
            services.AddTransient<IValidator<UpdateMentorRequest>, UpdateMentorRequestValidator>();
        }

        private static void RegisterSpecialityDependencies(IServiceCollection services)
        {
            services.AddTransient<ISpecialitiesService, SpecialitiesService>();
            services.AddTransient<ISpecialitiesRepository, SpecialitiesRepository>();
            services.AddTransient<IValidator<CreateSpecialityRequest>, CreateSpecialityValidator>();
            services.AddTransient<IValidator<UpdateSpecialityRequest>, UpdateSpecialityValidator>();
        }

        private static void RegisterLearningTopicsDependencies(IServiceCollection services)
        {
            services.AddTransient<ILearningTopicsService, LearningTopicsService>();
            services.AddTransient<ILearningTopicsRepository, LearningTopicsRepository>();
            services.AddTransient<IValidator<CreateLearningTopicRequest>, CreateLearningTopicRequestValidator>();
            services.AddTransient<IValidator<UpdateLearningTopicRequest>, UpdateLearningTopicRequestValidator>();
        }

        private static void RegisterInternDependencies(IServiceCollection services)
        {
            services.AddTransient<IInternCampaignsService, InternCampaignsService>();
            services.AddTransient<IInternsService, InternsService>();
            services.AddTransient<IInternsRepository, InternsRepository>();
            services.AddTransient<IValidator<AddInternCampaignRequest>, AddInternCampaignRequestValidator>();
            services.AddTransient<IValidator<UpdateInternCampaignRequest>, UpdateInternCampaignRequestValidator>();
            services.AddTransient<IValidator<AddStateRequest>, AddStateRequestValidator>();
            services.AddTransient<IValidator<CreateInternRequest>, CreateInternRequestValidator>();
            services.AddTransient<IValidator<UpdateInternRequest>, UpdateInternRequestValidator>();
        }
    }
}
