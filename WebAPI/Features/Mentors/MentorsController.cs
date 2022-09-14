using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using WebAPI.Common.Abstractions;
using WebAPI.Common.ErrorHandling;

namespace WebAPI.Features.Mentors
{
    public class MentorsController : ApiControllerBase
    {
        private readonly IMentorsService mentorsService;
        private readonly ILogger<MentorsController> mentorsControllerLogger;
        private readonly IValidator<CreateMentorRequest> createMentorRequestValidator;
        private readonly IValidator<UpdateMentorRequest> updateMentorRequestValidator;
        private readonly IValidator<PaginationFilterRequest> paginationFilterRequestValidator; 

        public MentorsController(
            IMentorsService mentorsService,
            ILogger<MentorsController> mentorsControllerLogger,
            IValidator<CreateMentorRequest> validator,
            IValidator<UpdateMentorRequest> updateMentorRequestValidator,
            IValidator<PaginationFilterRequest> paginationFilterRequestValidator)
        {
            this.mentorsService = mentorsService;
            this.mentorsControllerLogger = mentorsControllerLogger;
            this.createMentorRequestValidator = validator;
            this.updateMentorRequestValidator = updateMentorRequestValidator;
            this.paginationFilterRequestValidator = paginationFilterRequestValidator;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MentorSummaryResponse))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateAsync(CreateMentorRequest request)
        {
            mentorsControllerLogger.LogInformation($"[MentorsController] Create mentor with data: {JsonSerializer.Serialize(request)}");

            await createMentorRequestValidator.ValidateAndThrowAsync(request);

            var mentor = await mentorsService.CreateAsync(request);

            return Ok(mentor);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MentorSummaryResponse))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateMentorRequest request)
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

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MentorSummaryResponse))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            mentorsControllerLogger.LogInformation($"[MentorsController] Get mentor with Id {id}");

            var mentor = await mentorsService.GetByIdAsync(id);

            return Ok(mentor);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PaginationResponse<MentorSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetPageAsync([Required][FromQuery] int pageNum,
            [Required][FromQuery] int pageSize)
        {
            mentorsControllerLogger.LogInformation($"[MentorsController] Get {pageSize} mentors from page {pageNum}");

            var mentorCount = await mentorsService.GetCountAsync();

            if (mentorCount == 0)
            {
                mentorsControllerLogger.LogError($"[MentorsController] No mentors found");

                throw new CoreException("No mentors found.", HttpStatusCode.NotFound);
            }

            var toSkip = (pageNum - 1) * pageSize;

            var filter = new PaginationFilterRequest()
            {
                Skip = toSkip,
                Take = pageSize,
                Count = mentorCount
            };

            await paginationFilterRequestValidator.ValidateAndThrowAsync(filter);

            var mentors = await mentorsService.GetAllAsync(filter);

            var pageCount = (mentorCount + pageSize - 1) / pageSize;

            var paginationReponse = new PaginationResponse<MentorSummaryResponse>(mentors, pageNum, pageCount);

            return Ok(paginationReponse);
        }
    }
}
