using Core.Common.Pagination;
using Core.Features.Admins;
using Core.Features.Admins.Interfaces;
using Core.Features.Admins.RequestModels;
using Core.Features.Admins.Support;
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
using Core.Features.Persons;
using FluentValidation;
using Infrastructure.Features.Admins;
using Infrastructure.Features.Campaigns;
using Infrastructure.Features.Mentors;
using Infrastructure.Features.Specialities;
using Infrastructure.Features.LearningTopics;
using Infrastructure.Features.Interns;
using Core.Features.Persons.Interfaces;
using Infrastructure.Features.Persons;
using WebAPI.Features.Mentors.ApiRequestModels;
using WebAPI.Features.Mentors.Support;

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
            RegisterUsersAndIdentityDependencies(services);
            RegisterAdminDependencies(services);
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
            services.AddTransient<ICampaignValidator, CampaignValidator>();
        }

        private static void RegisterMentorDependencies(IServiceCollection services)
        {
            services.AddTransient<IMentorsService, MentorsService>();
            services.AddTransient<IMentorsRepository, MentorsRepository>();
            services.AddTransient<IValidator<CreateMentorRequest>, CreateMentorRequestValidator>();
            services.AddTransient<IValidator<UpdateMentorRequest>, UpdateMentorRequestValidator>();
            services.AddTransient<IValidator<CreateMentorApiRequest>, CreateMentorApiRequestValidator>();
            services.AddTransient<IMentorValidator, MentorValidator>();
        }

        private static void RegisterSpecialityDependencies(IServiceCollection services)
        {
            services.AddTransient<ISpecialitiesService, SpecialitiesService>();
            services.AddTransient<ISpecialitiesRepository, SpecialitiesRepository>();
            services.AddTransient<IValidator<CreateSpecialityRequest>, CreateSpecialityValidator>();
            services.AddTransient<IValidator<UpdateSpecialityRequest>, UpdateSpecialityValidator>();
            services.AddTransient<ISpecialityValidator, SpecialityValidator>();
        }

        private static void RegisterLearningTopicsDependencies(IServiceCollection services)
        {
            services.AddTransient<ILearningTopicsService, LearningTopicsService>();
            services.AddTransient<ILearningTopicsRepository, LearningTopicsRepository>();
            services.AddTransient<IValidator<CreateLearningTopicRequest>, CreateLearningTopicRequestValidator>();
            services.AddTransient<IValidator<UpdateLearningTopicRequest>, UpdateLearningTopicRequestValidator>();
            services.AddTransient<ILearningTopicValidator, LearningTopicValidator>();
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
            services.AddTransient<IValidator<InviteInternRequest>, InviteInternRequestValidator>();
            services.AddTransient<IInternValidator, InternValidator>();
        }

        private static void RegisterUsersAndIdentityDependencies(IServiceCollection services)
        {
            services.AddTransient<IIdentityRepository, IdentityRepository>();
            services.AddTransient<IPersonsRepository, PersonsRepository>();
            services.AddTransient<IPersonsService, PersonsService>();
        }

        private static void RegisterAdminDependencies(IServiceCollection services)
        {
            services.AddTransient<IAdminsService, AdminsService>();
            services.AddTransient<IAdminsRepository, AdminsRepository>();
            services.AddTransient<IValidator<CreateAdminRequest>, CreateAdminRequestValidator>();
            services.AddTransient<IValidator<AddMentorRoleRequest>, AddMentorRoleRequestValidator>();
            services.AddTransient<IAdminValidator, AdminValidator>();
        }
    }
}
