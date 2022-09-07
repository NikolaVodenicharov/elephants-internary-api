using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
using Core.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using DA = System.ComponentModel.DataAnnotations;
using System.Net;
using WebAPI.Common.Abstractions;
using WebAPI.Common.ErrorHandling;
using System.Text.Json;

namespace WebAPI.Features.Campaigns
{
    public class CampaignsController : ApiControllerBase
    {
        private readonly ICampaignsService campaignsService;
        private readonly IValidator<CreateCampaign> createCampaingValidator;
        private readonly IValidator<UpdateCampaign> updateCampaignValidator;
        private readonly IValidator<PaginationFilterRequest> paginationFilterRequestValidator;
        private readonly ILogger<CampaignsController> campaignsControllerLogger;

        public CampaignsController(
            ICampaignsService campaignsService, 
            IValidator<CreateCampaign> createCampaingValidator, 
            IValidator<UpdateCampaign> updateCampaignValidator,
            IValidator<PaginationFilterRequest> paginationFilterRequestValidator,
            ILogger<CampaignsController> campaignsControllerLogger)
        {
            this.campaignsService = campaignsService;
            this.createCampaingValidator = createCampaingValidator;
            this.updateCampaignValidator = updateCampaignValidator;
            this.paginationFilterRequestValidator = paginationFilterRequestValidator;
            this.campaignsControllerLogger = campaignsControllerLogger;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CampaignSummaryResponse))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> Create(CreateCampaign model)
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
        public async Task<ActionResult> Update(Guid id, UpdateCampaign model)
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
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetPageAsync([DA.Required][FromQuery] int pageNum,
            [DA.Required][FromQuery] int pageSize)
        {
            campaignsControllerLogger.LogInformation($"[CampaignsController] Get {pageSize} campaigns from page {pageNum}");

            var campaignCount = await campaignsService.GetCountAsync();

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
    }
}

