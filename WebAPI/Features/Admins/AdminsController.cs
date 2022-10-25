using Core.Common;
using Core.Common.Pagination;
using Core.Features.Admins.Interfaces;
using Core.Features.Admins.RequestModels;
using Core.Features.Admins.ResponseModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Persons.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebAPI.Common;
using WebAPI.Common.Authorization;
using WebAPI.Common.Abstractions;
using WebAPI.Common.SettingsModels;

namespace WebAPI.Features.Admins
{
    [CustomAuthorize(RoleId.Administrator)]
    public class AdminsController : ApiControllerBase
    {
        private readonly IAdminsService adminsService;
        private readonly IAdminValidator adminValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;
        private readonly ILogger<AdminsController> adminsControllerLogger;
        private readonly InvitationUrlSettings invitationUrls;
        
        public AdminsController(
            IAdminsService adminsService,
            IAdminValidator adminValidator,
            IValidator<PaginationRequest> paginationRequestValidator,
            ILogger<AdminsController> adminsControllerLogger,
            IOptions<InvitationUrlSettings> invitationUrlSettings)
        {
            this.adminsService = adminsService;
            this.adminValidator = adminValidator;
            this.paginationRequestValidator = paginationRequestValidator;
            this.adminsControllerLogger = adminsControllerLogger;
            this.invitationUrls = invitationUrlSettings.Value;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<AdminSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> CreateAsync(string email)
        {
            adminsControllerLogger.LogInformationMethod(nameof(AdminsController), nameof(CreateAsync));

            var createAdminRequest = new CreateAdminRequest(
                email,
                invitationUrls.BackOfficeUrl);
            
            await adminValidator.ValidateAndThrowAsync(createAdminRequest);
            
            var createAdminResponse = await adminsService.CreateAsync(createAdminRequest);

            return CoreResult.Success(createAdminResponse);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<AdminSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            adminsControllerLogger.LogInformationMethod(nameof(AdminsController), nameof(GetByIdAsync));

            var adminResponse = await adminsService.GetByIdAsync(id);

            return CoreResult.Success(adminResponse);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<PaginationResponse<AdminListingResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetAllAsync([Required][FromQuery] int pageNum,
            [Required][FromQuery] int pageSize)
        {
            adminsControllerLogger.LogInformationMethod(nameof(AdminsController), nameof(GetAllAsync));
            
            var filter = new PaginationRequest(pageNum, pageSize);

            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            var admins = await adminsService.GetAllAsync(filter);

            return CoreResult.Success(admins);
        }

        [HttpPost("{id}/mentor")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<MentorSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> AddMentorRoleAsync(Guid id, AddMentorRoleRequest request)
        {
            adminsControllerLogger.LogInformationMethod(nameof(AdminsController), nameof(AddMentorRoleAsync));

            if (id != request.Id)
            {
                adminsControllerLogger.LogErrorAndThrowExceptionIdMismatch(nameof(AdminsController), 
                    request.Id, id);
            }

            await adminValidator.ValidateAndThrowAsync(request);

            var mentorSummaryResponse = await adminsService.AddAsMentorAsync(request);

            return CoreResult.Success(mentorSummaryResponse);
        }
    }
}