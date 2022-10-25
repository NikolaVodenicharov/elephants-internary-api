using Core.Common;
using Core.Common.Pagination;
using Core.Features.Admins.Interfaces;
using Core.Features.Admins.RequestModels;
using Core.Features.Admins.ResponseModels;
using Core.Features.Persons.Entities;
using Core.Features.Persons.Interfaces;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Specialties.Entities;
using Core.Features.Specialities.Interfaces;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace Core.Features.Admins
{
    public class AdminsService : IAdminsService
    {
        private readonly IAdminsRepository adminsRepository;
        private readonly IIdentityRepository identityRepository;
        private readonly IMentorsRepository mentorsRepository;
        private readonly ISpecialitiesRepository specialitiesRepository;
        private readonly ILogger<AdminsService> adminServiceLogger;
        private readonly IAdminValidator adminValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;

        public AdminsService(
            IAdminsRepository adminsRepository,
            IIdentityRepository identityRepository,
            IMentorsRepository mentorsRepository,
            ISpecialitiesRepository specialitiesRepository,
            ILogger<AdminsService> adminServiceLogger,
            IAdminValidator adminValidator,
            IValidator<PaginationRequest> paginationRequestValidator)
        {
            this.adminsRepository = adminsRepository;
            this.identityRepository = identityRepository;
            this.mentorsRepository = mentorsRepository;
            this.adminServiceLogger = adminServiceLogger;
            this.adminValidator = adminValidator;
            this.specialitiesRepository = specialitiesRepository;
            this.paginationRequestValidator = paginationRequestValidator;
        }

        public async Task<AdminSummaryResponse> CreateAsync(CreateAdminRequest request)
        {
            await adminValidator.ValidateAndThrowAsync(request);

            var emailExists = await adminsRepository.ExistsByEmailAsync(request.Email);

            if(emailExists)
            {
                adminServiceLogger.LogErrorAndThrowExceptionValueTaken(nameof(AdminsService), nameof(Person), 
                    nameof(request.Email), request.Email);
            }

            var identitySummaryResponse = await identityRepository.SendUserInviteAsync(request.Email, request.ApplicationUrl);

            var admin = new CreateAdminRepoRequest(
                identitySummaryResponse.DisplayName,
                identitySummaryResponse.Email
            );

            var adminSummaryResponse = await adminsRepository.CreateAsync(admin);

            adminServiceLogger.LogInformationMethod(nameof(AdminsService), nameof(CreateAsync), true);

            return adminSummaryResponse;
        }

        public async Task<AdminSummaryResponse?> GetByIdAsync(Guid id)
        {
            var admin = await adminsRepository.GetByIdAsync(id);

            Guard.EnsureNotNull(admin, adminServiceLogger, nameof(AdminsService), nameof(RoleId.Administrator), id);

            adminServiceLogger.LogInformationMethod(nameof(AdminsService), nameof(GetByIdAsync), true);

            return admin;
        }

        public async Task<PaginationResponse<AdminListingResponse>> GetAllAsync(PaginationRequest filter)
        {
            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            Guard.EnsureNotNullPagination(filter.PageNum, filter.PageSize, adminServiceLogger, nameof(GetAllAsync));

            var adminsCount = await adminsRepository.GetCountAsync();

            var totalPages = PaginationMethods.CalculateTotalPages(adminsCount, filter.PageSize.Value);

            if (filter.PageNum > totalPages)
            {
                adminServiceLogger.LogErrorAndThrowExceptionPageCount(
                    nameof(GetAllAsync), totalPages, filter.PageNum.Value);
            }

            var admins = adminsCount > 0 ?
                await adminsRepository.GetAllAsync(filter) :
                new List<AdminListingResponse>();
            
            var paginationResponse = new PaginationResponse<AdminListingResponse>(
                admins,
                filter.PageNum.Value,
                totalPages
            );

            adminServiceLogger.LogInformationMethod(nameof(AdminsService), nameof(GetAllAsync), true);

            return paginationResponse;
        }

        public async Task<MentorSummaryResponse> AddAsMentorAsync(AddMentorRoleRequest addMentorRoleRequest)
        {
            await adminValidator.ValidateAndThrowAsync(addMentorRoleRequest);

            var isAlreadyMentor = await mentorsRepository.IsMentorByIdAsync(addMentorRoleRequest.Id);

            if(isAlreadyMentor)
            {
                adminServiceLogger.LogErrorAndThrowExceptionValueTaken(nameof(AdminsService), nameof(RoleId.Administrator), 
                    "Role", nameof(RoleId.Mentor));
            }

            var specialities = await ValidateAndGetSpecialities(addMentorRoleRequest.SpecialityIds.Distinct());

            var addMentorRoleRepoRequest = new AddMentorRoleRepoRequest(
                addMentorRoleRequest.Id,
                specialities);
            
            var mentorSummaryResponse = await mentorsRepository.AddMentorRoleByIdAsync(addMentorRoleRepoRequest);

            Guard.EnsureNotNull(
                mentorSummaryResponse, adminServiceLogger, 
                nameof(AdminsService), nameof(RoleId.Administrator), 
                addMentorRoleRequest.Id);

            adminServiceLogger.LogInformationMethod(nameof(AdminsService), nameof(AddAsMentorAsync), true);

            return mentorSummaryResponse;
        }

        private async Task<ICollection<Speciality>> ValidateAndGetSpecialities(IEnumerable<Guid> specialtyIds)
        {
            var specialityList = await specialitiesRepository.GetByIdsAsync(specialtyIds);

            if (specialityList.Count != specialtyIds.Count())
            {
                adminServiceLogger.LogErrorAndThrowExceptionNotAllFound(nameof(AdminsService), nameof(Speciality), specialtyIds);
            }

            return specialityList;
        }
    }
}