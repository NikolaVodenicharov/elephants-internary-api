using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
using Core.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebAPI.Common.Abstractions;
using WebAPI.Common.ErrorHandling;
using System.Text.Json;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.ResponseModels;
using Core.Common.Pagination;

namespace WebAPI.Features.Campaigns
{
    public class CampaignsController : ApiControllerBase
    {
        private readonly ICampaignsService campaignsService;
        private readonly IMentorsService mentorsService;
        private readonly IValidator<CreateCampaignRequest> createCampaingValidator;
        private readonly IValidator<UpdateCampaignRequest> updateCampaignValidator;
        private readonly IValidator<PaginationFilterRequest> paginationFilterRequestValidator;
        private readonly ILogger<CampaignsController> campaignsControllerLogger;

        public CampaignsController(
            ICampaignsService campaignsService,
            IMentorsService mentorsService,
            IValidator<CreateCampaignRequest> createCampaingValidator, 
            IValidator<UpdateCampaignRequest> updateCampaignValidator,
            IValidator<PaginationFilterRequest> paginationFilterRequestValidator,
            ILogger<CampaignsController> campaignsControllerLogger)
        {
            this.campaignsService = campaignsService;
            this.mentorsService = mentorsService;
            this.createCampaingValidator = createCampaingValidator;
            this.updateCampaignValidator = updateCampaignValidator;
            this.paginationFilterRequestValidator = paginationFilterRequestValidator;
            this.campaignsControllerLogger = campaignsControllerLogger;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CampaignSummaryResponse))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> CreateAsync(CreateCampaignRequest model)
        {
            campaignsControllerLogger.LogInformation($"[CampaignsController] Create campaign: {JsonSerializer.Serialize(model)}");

            await createCampaingValidator.ValidateAndThrowAsync(model);

            var result = await campaignsService.CreateAsync(model);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CampaignSummaryResponse))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateCampaignRequest model)
        {
            if (id != model.Id)
            {
                campaignsControllerLogger.LogError($"[CampaignsController] Invalid Campaign Id ({id}) in Update request data.");
                
                throw new CoreException("Invalid Campaign Id in request data", HttpStatusCode.BadRequest);
            }
            
            await updateCampaignValidator.ValidateAndThrowAsync(model);

            var result = await campaignsService.UpdateAsync(model);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CampaignSummaryResponse))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            campaignsControllerLogger.LogInformation($"[CampaignsController] Get campaign with Id {id}");

            var campaign = await campaignsService.GetByIdAsync(id);

            return Ok(campaign);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PaginationResponse<CampaignSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetPageAsync([Required][FromQuery] int pageNum,
            [Required][FromQuery] int pageSize)
        {
            campaignsControllerLogger.LogInformation($"[CampaignsController] Get {pageSize} campaigns from page {pageNum}");

            var campaignCount = await campaignsService.GetCountAsync();

            if (campaignCount == 0)
            {
                campaignsControllerLogger.LogError("[CampaignsController] No campaigns found");

                throw new CoreException("No campaigns found.", HttpStatusCode.NotFound);
            }

            var toSkip = (pageNum - 1) * pageSize;

            var filter = new PaginationFilterRequest()
            {
                Skip = toSkip,
                Take = pageSize,
                Count = campaignCount
            };

            await paginationFilterRequestValidator.ValidateAndThrowAsync(filter);

            var campaigns = await campaignsService.GetAllAsync(filter);

            var pageCount = (campaignCount + pageSize - 1) / pageSize;

            var paginationReponse = new PaginationResponse<CampaignSummaryResponse>(campaigns, pageNum, pageCount);

            return Ok(paginationReponse);
        }

        [HttpGet("{id}/mentors")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PaginationResponse<MentorSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetMentorsByCampaignIdAsync(Guid id, [Required][FromQuery] int pageNum,
            [Required][FromQuery] int pageSize)
        {
            campaignsControllerLogger.LogInformation($"[CampaignsController] Get {pageSize} mentors from page {pageNum} " +
                $"for campaign with Id {id}");

            await campaignsService.GetByIdAsync(id);

            var mentorCount = await mentorsService.GetCountByCampaignIdAsync(id);

            if (mentorCount == 0)
            {
                campaignsControllerLogger.LogError($"[CampaignsController] No mentors found for campaign with Id {id}");

                throw new CoreException("No mentors found for selected campaign.", HttpStatusCode.NotFound);
            }

            var toSkip = (pageNum - 1) * pageSize;

            var filter = new PaginationFilterRequest()
            {
                Skip = toSkip,
                Take = pageSize,
                Count = mentorCount
            };

            await paginationFilterRequestValidator.ValidateAndThrowAsync(filter);

            var mentors = await mentorsService.GetMentorsByCampaignIdAsync(id, filter);

            var pageCount = (mentorCount + pageSize - 1) / pageSize;

            var paginationReponse = new PaginationResponse<MentorSummaryResponse>(mentors, pageNum, pageCount);

            return Ok(paginationReponse);
        }
    }
}

