using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using WebAPI.Common;
using WebAPI.Common.Abstractions;

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
            mentorsControllerLogger.LogInformation($"[MentorsController] Create mentor with data: {JsonSerializer.Serialize(request)}");

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
            mentorsControllerLogger.LogInformation($"[MentorsController] Update mentor with Id {request.Id} with data: " +
                $"{JsonSerializer.Serialize(request)}");

            if (id != request.Id)
            {
                mentorsControllerLogger.LogError($"[MentorsController] Invalid mentor Id ({id}) in Update request data");

                throw new CoreException("Invalid mentor Id in request data.", HttpStatusCode.BadRequest);
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
            mentorsControllerLogger.LogInformation($"[MentorsController] Get mentor with Id {id}");

            var mentor = await mentorsService.GetByIdAsync(id);

            return CoreResult.Success(mentor);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<PaginationResponse<MentorSummaryResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetAllAsync([Required][FromQuery] int pageNum,
            [Required][FromQuery] int pageSize)
        {
            mentorsControllerLogger.LogInformation($"[MentorsController] Get {pageSize} mentors from page {pageNum}");

            var filter = new PaginationRequest(pageNum, pageSize);

            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            var paginationReponse = await mentorsService.GetAllAsync(filter);

            return CoreResult.Success(paginationReponse);
        }
    }
}
