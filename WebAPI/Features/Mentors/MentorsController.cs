using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using WebAPI.Common;
using WebAPI.Common.Abstractions;
using Core.Common;
using Core.Features.Mentors.Entities;

namespace WebAPI.Features.Mentors
{
    [Authorize]
    public class MentorsController : ApiControllerBase
    {
        private readonly IMentorsService mentorsService;
        private readonly ILogger<MentorsController> mentorsControllerLogger;
        private readonly IValidator<CreateMentorRequest> createMentorRequestValidator;
        private readonly IValidator<UpdateMentorRequest> updateMentorRequestValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator; 

        public MentorsController(
            IMentorsService mentorsService,
            ILogger<MentorsController> mentorsControllerLogger,
            IValidator<CreateMentorRequest> createMentorRequestValidator,
            IValidator<UpdateMentorRequest> updateMentorRequestValidator,
            IValidator<PaginationRequest> paginationRequestValidator)
        {
            this.mentorsService = mentorsService;
            this.mentorsControllerLogger = mentorsControllerLogger;
            this.createMentorRequestValidator = createMentorRequestValidator;
            this.updateMentorRequestValidator = updateMentorRequestValidator;
            this.paginationRequestValidator = paginationRequestValidator;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<MentorSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> CreateAsync(CreateMentorRequest request)
        {
            mentorsControllerLogger.LogInformationMethod(nameof(MentorsController), nameof(CreateAsync));

            await createMentorRequestValidator.ValidateAndThrowAsync(request);

            var mentor = await mentorsService.CreateAsync(request);

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
