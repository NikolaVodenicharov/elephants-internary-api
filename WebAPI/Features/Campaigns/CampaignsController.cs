﻿using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
using Core.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebAPI.Common.Abstractions;
using System.Text.Json;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.ResponseModels;
using Core.Common.Pagination;
using WebAPI.Common;

namespace WebAPI.Features.Campaigns
{
    [Authorize]
    public class CampaignsController : ApiControllerBase
    {
        private readonly ICampaignsService campaignsService;
        private readonly IMentorsService mentorsService;
        private readonly IValidator<CreateCampaignRequest> createCampaingValidator;
        private readonly IValidator<UpdateCampaignRequest> updateCampaignValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;
        private readonly ILogger<CampaignsController> campaignsControllerLogger;

        public CampaignsController(
            ICampaignsService campaignsService,
            IMentorsService mentorsService,
            IValidator<CreateCampaignRequest> createCampaingValidator, 
            IValidator<UpdateCampaignRequest> updateCampaignValidator,
            IValidator<PaginationRequest> paginationRequestValidator,
            ILogger<CampaignsController> campaignsControllerLogger)
        {
            this.campaignsService = campaignsService;
            this.mentorsService = mentorsService;
            this.createCampaingValidator = createCampaingValidator;
            this.updateCampaignValidator = updateCampaignValidator;
            this.paginationRequestValidator = paginationRequestValidator;
            this.campaignsControllerLogger = campaignsControllerLogger;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<CampaignSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> CreateAsync(CreateCampaignRequest model)
        {
            campaignsControllerLogger.LogInformation($"[CampaignsController] Create campaign: {JsonSerializer.Serialize(model)}");

            await createCampaingValidator.ValidateAndThrowAsync(model);

            var result = await campaignsService.CreateAsync(model);

            return CoreResult.Success(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<CampaignSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateCampaignRequest model)
        {
            if (id != model.Id)
            {
                campaignsControllerLogger.LogError($"[CampaignsController] Invalid Campaign Id ({id}) in Update request data.");
                
                throw new CoreException("Invalid Campaign Id in request data", HttpStatusCode.BadRequest);
            }
            
            await updateCampaignValidator.ValidateAndThrowAsync(model);

            var result = await campaignsService.UpdateAsync(model);

            return CoreResult.Success(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<CampaignSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            campaignsControllerLogger.LogInformation($"[CampaignsController] Get campaign with Id {id}");

            var campaign = await campaignsService.GetByIdAsync(id);

            return CoreResult.Success(campaign);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<PaginationResponse<CampaignSummaryResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetAllAsync([Required][FromQuery] int pageNum,
            [Required][FromQuery] int pageSize)
        {
            campaignsControllerLogger.LogInformation($"[CampaignsController] Get {pageSize} campaigns from page {pageNum}");

            var filter = new PaginationRequest(pageNum, pageSize);

            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            var paginationReponse = await campaignsService.GetAllAsync(filter);

            return CoreResult.Success(paginationReponse);
        }

        [HttpGet("{id}/mentors")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<PaginationResponse<MentorSummaryResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetMentorsByCampaignIdAsync(Guid id, [Required][FromQuery] int pageNum,
            [Required][FromQuery] int pageSize)
        {
            campaignsControllerLogger.LogInformation($"[CampaignsController] Get {pageSize} mentors from page {pageNum} " +
                $"for campaign with Id {id}");

            await campaignsService.GetByIdAsync(id);

            var filter = new PaginationRequest(pageNum, pageSize);

            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            var paginationResponse = await mentorsService.GetAllAsync(filter, id);

            return CoreResult.Success(paginationResponse);
        }
    }
}

