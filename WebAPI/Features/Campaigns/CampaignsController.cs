﻿using Core.Common;
using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.ResponseModels;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Persons.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebAPI.Common;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Authorization;

namespace WebAPI.Features.Campaigns
{
    [CustomAuthorize(RoleId.Administrator)]
    public class CampaignsController : ApiControllerBase
    {
        private readonly ICampaignsService campaignsService;
        private readonly IMentorsService mentorsService;
        private readonly IInternsService internService;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;
        private readonly ICampaignValidator campaignValidator;
        private readonly ILogger<CampaignsController> campaignsControllerLogger;

        public CampaignsController(
            ICampaignsService campaignsService,
            IMentorsService mentorsService,
            IInternsService internService,
            IValidator<PaginationRequest> paginationRequestValidator,
            ICampaignValidator campaignValidator,
            ILogger<CampaignsController> campaignsControllerLogger)
        {
            this.campaignsService = campaignsService;
            this.mentorsService = mentorsService;
            this.internService = internService;
            this.paginationRequestValidator = paginationRequestValidator;
            this.campaignValidator = campaignValidator;
            this.campaignsControllerLogger = campaignsControllerLogger;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<CampaignSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> CreateAsync(CreateCampaignRequest model)
        {
            campaignsControllerLogger.LogInformation(nameof(CampaignsController), nameof(CreateAsync));

            await campaignValidator.ValidateAndThrowAsync(model);

            var result = await campaignsService.CreateAsync(model);

            return CoreResult.Success(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<CampaignSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateCampaignRequest model)
        {
            campaignsControllerLogger.LogInformationMethod(nameof(CampaignsController), nameof(UpdateAsync), nameof(Campaign), id);

            if (id != model.Id)
            {
                campaignsControllerLogger.LogErrorAndThrowExceptionIdMismatch(nameof(CampaignsController), model.Id, id);
            }
            
            await campaignValidator.ValidateAndThrowAsync(model);

            var result = await campaignsService.UpdateAsync(model);

            return CoreResult.Success(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<CampaignSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            campaignsControllerLogger.LogInformationMethod(nameof(CampaignsController), nameof(GetByIdAsync), nameof(Campaign), id);

            var campaign = await campaignsService.GetByIdAsync(id);

            return CoreResult.Success(campaign);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<PaginationResponse<CampaignSummaryResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest filter)
        {
            campaignsControllerLogger.LogInformationMethod(nameof(CampaignsController), nameof(GetAllAsync));

            if (filter.PageNum == null && filter.PageSize == null)
            {
                var campaigns = await campaignsService.GetAllAsync();

                return CoreResult.Success(campaigns);
            }

            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            var paginationReponse = await campaignsService.GetPaginationAsync(filter);

            return CoreResult.Success(paginationReponse);
        }

        [HttpGet("{id}/mentors")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<PaginationResponse<MentorPaginationResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetMentorsByCampaignIdAsync(Guid id, [FromQuery] PaginationRequest filter)
        {
            campaignsControllerLogger.LogInformationMethod(nameof(CampaignsController), nameof(GetMentorsByCampaignIdAsync), nameof(Campaign), id);

            await campaignsService.GetByIdAsync(id);

            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            var paginationResponse = await mentorsService.GetPaginationAsync(filter, id);

            return CoreResult.Success(paginationResponse);
        }

        [HttpGet("{id}/interns")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<PaginationResponse<InternSummaryResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetInternsByCampaignAsync(Guid id, [FromQuery] PaginationRequest filter)
        {
            campaignsControllerLogger.LogInformationMethod(
                nameof(CampaignsController), 
                nameof(GetInternsByCampaignAsync), 
                nameof(Campaign), 
                id);

            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            var InternSummaryResponseCollection = await internService.GetAllByCampaignIdAsync(filter, id);

            return CoreResult.Success(InternSummaryResponseCollection);
        }

        [HttpPost("{id}/mentors")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<bool>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> AddMentorAsync(Guid id, AddMentorToCampaignRequest request)
        {
            if (id != request.CampaignId)
            {
                campaignsControllerLogger.LogErrorAndThrowExceptionIdMismatch(nameof(CampaignsController),
                    request.CampaignId, id);
            }

            await mentorsService.AddToCampaignAsync(request);

            return CoreResult.Success(true);
        }

        [HttpDelete("{id}/mentors/{mentorId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<bool>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> RemoveMentorAsync(Guid id, Guid mentorId)
        {
            campaignsControllerLogger.LogInformationMethod(nameof(CampaignsController), nameof(RemoveMentorAsync));

            var isRemoved = await mentorsService.RemoveFromCampaignAsync(id, mentorId);

            return CoreResult.Success(isRemoved);
        }
    }
}

