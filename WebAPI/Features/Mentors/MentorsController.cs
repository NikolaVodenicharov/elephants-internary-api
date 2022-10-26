using Core.Common;
using Core.Common.Pagination;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Persons.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using WebAPI.Common;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Authorization;
using WebAPI.Common.SettingsModels;
using WebAPI.Features.Mentors.ApiRequestModels;

namespace WebAPI.Features.Mentors
{
    [CustomAuthorize(RoleId.Administrator)]
    public class MentorsController : ApiControllerBase
    {
        private readonly IMentorsService mentorsService;
        private readonly ILogger<MentorsController> mentorsControllerLogger;
        private readonly IMentorValidator mentorValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator; 
        private readonly InvitationUrlSettings invitationUrls;

        public MentorsController(
            IMentorsService mentorsService,
            ILogger<MentorsController> mentorsControllerLogger,
            IMentorValidator mentorValidator,
            IValidator<PaginationRequest> paginationRequestValidator,
            IOptions<InvitationUrlSettings> invitationUrlSettings)
        {
            this.mentorsService = mentorsService;
            this.mentorsControllerLogger = mentorsControllerLogger;
            this.mentorValidator = mentorValidator;
            this.paginationRequestValidator = paginationRequestValidator;
            this.invitationUrls = invitationUrlSettings.Value;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<MentorSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> CreateAsync(CreateMentorApiRequest createMentorApiRequest)
        {
            var createMentorRequest = new CreateMentorRequest(
                createMentorApiRequest.Email,
                createMentorApiRequest.SpecialityIds,
                invitationUrls.BackOfficeUrl);

            await mentorValidator.ValidateAndThrowAsync(createMentorRequest);

            var mentorSummaryResponse = await mentorsService.CreateAsync(createMentorRequest);

            mentorsControllerLogger.LogInformationMethod(nameof(MentorsController), nameof(CreateAsync), nameof(Person), mentorSummaryResponse.Id);

            return CoreResult.Success(mentorSummaryResponse);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<MentorDetailsResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateMentorRequest request)
        {
            mentorsControllerLogger.LogInformationMethod(nameof(MentorsController), nameof(UpdateAsync), nameof(Person), id);

            if (id != request.Id)
            {
                mentorsControllerLogger.LogErrorAndThrowExceptionIdMismatch(nameof(MentorsController), 
                    request.Id, id);
            }

            await mentorValidator.ValidateAndThrowAsync(request);

            var result = await mentorsService.UpdateAsync(request);

            return CoreResult.Success(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<MentorDetailsResponse>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            mentorsControllerLogger.LogInformationMethod(nameof(MentorsController), nameof(GetByIdAsync), nameof(Person), id);

            var mentor = await mentorsService.GetByIdAsync(id);

            return CoreResult.Success(mentor);
        }

        [HttpGet]
        [ProducesResponseType(typeof(CoreResponse<PaginationResponse<MentorPaginationResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<IEnumerable<MentorPaginationResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest filter)
        {
            mentorsControllerLogger.LogInformationMethod(nameof(MentorsController), nameof(GetAllAsync));

            if (filter.PageNum == null && filter.PageSize == null)
            {
                var mentors = await mentorsService.GetAllAsync();

                return CoreResult.Success(mentors);
            }

            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            var paginationResponse = await mentorsService.GetPaginationAsync(filter);

            return CoreResult.Success(paginationResponse);
        }
    }
}
