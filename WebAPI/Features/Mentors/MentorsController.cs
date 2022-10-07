using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Users.Entities;
using Core.Features.Users.Interfaces;
using Core.Features.Users.RequestModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Net;
using WebAPI.Common;
using WebAPI.Common.Abstractions;
using WebAPI.Features.Mentors.ApiRequestModels;
using Core.Common;
using Core.Features.Mentors.Entities;

namespace WebAPI.Features.Mentors
{
    [Authorize]
    public class MentorsController : ApiControllerBase
    {
        private readonly IMentorsService mentorsService;
        private readonly IUsersService usersService;
        private readonly ILogger<MentorsController> mentorsControllerLogger;
        private readonly IValidator<CreateMentorRequest> createMentorRequestValidator;
        private readonly IValidator<UpdateMentorRequest> updateMentorRequestValidator;
        private readonly IValidator<CreateMentorApiRequest> createMentorApiRequestValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator; 
        private readonly InvitationUrlSettings invitationUrls;

        public MentorsController(
            IMentorsService mentorsService,
            IUsersService usersService,
            ILogger<MentorsController> mentorsControllerLogger,
            IValidator<CreateMentorRequest> createMentorRequestValidator,
            IValidator<UpdateMentorRequest> updateMentorRequestValidator,
            IValidator<CreateMentorApiRequest> createMentorApiRequestValidator,
            IValidator<PaginationRequest> paginationRequestValidator,
            IOptions<InvitationUrlSettings> invitationUrlSettings)
        {
            this.mentorsService = mentorsService;
            this.usersService = usersService;
            this.mentorsControllerLogger = mentorsControllerLogger;
            this.createMentorRequestValidator = createMentorRequestValidator;
            this.updateMentorRequestValidator = updateMentorRequestValidator;
            this.createMentorApiRequestValidator = createMentorApiRequestValidator;
            this.paginationRequestValidator = paginationRequestValidator;
            this.invitationUrls = invitationUrlSettings.Value;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<MentorSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> CreateAsync(CreateMentorApiRequest request)
        {
            mentorsControllerLogger.LogInformationMethod(nameof(MentorsController), nameof(CreateAsync));

            await createMentorApiRequestValidator.ValidateAndThrowAsync(request);

            var userExists = await usersService.ExistsByEmailAsync(request.Email);

            if(userExists)
            {
                mentorsControllerLogger.LogErrorAndThrowExceptionValueTaken(nameof(MentorsController), 
                    nameof(Core.Features.Users.Entities.User),
                    nameof(Core.Features.Users.Entities.User.Email), 
                    request.Email);
            }

            var invitationResponse = await usersService.SendInvitationByEmailAsync(request.Email, invitationUrls.BackOfficeUrl);

            var mentorRequest = new CreateMentorRequest(
                invitationResponse.DisplayName, 
                request.Email, 
                request.SpecialityIds);

            await createMentorRequestValidator.ValidateAndThrowAsync(mentorRequest);

            var mentor = await mentorsService.CreateAsync(mentorRequest);

            var userRequest = new CreateUserRequest(mentor.Email, RoleEnum.Mentor, mentor.Id);
            
            await usersService.CreateAsync(userRequest);

            return CoreResult.Success(mentor);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<MentorSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateMentorRequest request)
        {
            mentorsControllerLogger.LogInformationMethod(nameof(MentorsController), nameof(UpdateAsync), nameof(Mentor), id);

            if (id != request.Id)
            {
                mentorsControllerLogger.LogErrorAndThrowExceptionIdMismatch(nameof(MentorsController), 
                    request.Id, id);
            }

            await updateMentorRequestValidator.ValidateAndThrowAsync(request);

            var result = await mentorsService.UpdateAsync(request);

            return CoreResult.Success(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<MentorSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            mentorsControllerLogger.LogInformationMethod(nameof(MentorsController), nameof(GetByIdAsync), nameof(Mentor), id);

            var mentor = await mentorsService.GetByIdAsync(id);

            return CoreResult.Success(mentor);
        }

        [HttpGet]
        [ProducesResponseType(typeof(CoreResponse<PaginationResponse<MentorSummaryResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<IEnumerable<MentorSummaryResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllAsync(int? pageNum = null, int? pageSize = null)
        {
            mentorsControllerLogger.LogInformationMethod(nameof(MentorsController), nameof(GetAllAsync));

            if (pageNum == null && pageSize == null)
            {
                var mentors = await mentorsService.GetAllAsync();

                return CoreResult.Success(mentors);
            }

            var filter = new PaginationRequest(pageNum, pageSize);

            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            var paginationResponse = await mentorsService.GetPaginationAsync(filter);

            return CoreResult.Success(paginationResponse);
        }
    }
}
