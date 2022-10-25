using Core.Common;
using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Interns.Entities;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;
using Core.Features.Persons.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using WebAPI.Common;
using WebAPI.Common.SettingsModels;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Authorization;
using WebAPI.Features.Interns.ApiRequestModels;

namespace WebAPI.Features.Interns
{
    [CustomAuthorize(RoleId.Administrator)]
    public class InternsController : ApiControllerBase
    {
        private readonly IInternsService internsService;
        private readonly IInternCampaignsService internCampaignsService;
        private readonly ILogger<InternsController> internsControllerLogger;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;
        private readonly IInternValidator internValidator;
        private readonly InvitationUrlSettings invitationUrls;

        public InternsController(
            IInternsService internsService, 
            IInternCampaignsService internCampaignsService,
            ILogger<InternsController> internsControllerLogger,
            IValidator<PaginationRequest> paginationRequestValidator,
            IInternValidator internValidator,
            IOptions<InvitationUrlSettings> invitationUrlSettings)
        {
            this.internsService = internsService;
            this.internCampaignsService = internCampaignsService;
            this.internsControllerLogger = internsControllerLogger;
            this.paginationRequestValidator = paginationRequestValidator;
            this.internValidator = internValidator;
            this.invitationUrls = invitationUrlSettings.Value;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CoreResponse<InternSummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAsync(CreateInternRequest createInternRequest)
        {
            internsControllerLogger.LogInformationMethod(nameof(InternsController), nameof(CreateAsync));

            await internValidator.ValidateAndThrowAsync(createInternRequest);

            var specialitySummaryResponse = await internsService.CreateAsync(createInternRequest);

            return CoreResult.Success(specialitySummaryResponse);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CoreResponse<InternSummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateInternApiRequest updateInternApiRequest)
        {
            internsControllerLogger.LogInformationMethod(nameof(InternsController), nameof(UpdateAsync));

            var updateInternRequest = new UpdateInternRequest(
                id,
                updateInternApiRequest.FirstName,
                updateInternApiRequest.LastName,
                updateInternApiRequest.Email);

            await internValidator.ValidateAndThrowAsync(updateInternRequest);

            var internSummaryResponse = await internsService.UpdateAsync(updateInternRequest);

            return CoreResult.Success(internSummaryResponse);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CoreResponse<InternDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetDetailsByIdAsync(Guid id)
        {
            internsControllerLogger.LogInformationMethod(nameof(InternsController), nameof(GetDetailsByIdAsync));

            var internDetailsResponse = await internsService.GetDetailsByIdAsync(id);

            return CoreResult.Success(internDetailsResponse);
        }

        [HttpGet]
        [ProducesResponseType(typeof(CoreResponse<IEnumerable<InternListingResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<PaginationResponse<InternListingResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllAsync(int? pageNum = null, int? pageSize = null)
        {
            internsControllerLogger.LogInformationMethod(nameof(InternsController), nameof(GetAllAsync));

            if (pageNum == null && pageSize == null)
            {
                var internListingResponses = await internsService.GetAllAsync();

                return CoreResult.Success(internListingResponses);
            }

            var paginationRequest = new PaginationRequest(pageNum, pageSize);

            await paginationRequestValidator.ValidateAndThrowAsync(paginationRequest);

            var paginationResponse = await internsService.GetPaginationAsync(paginationRequest);

            return CoreResult.Success(paginationResponse);
        }

        [HttpPost("{id}/campaigns/{campaignId}")]
        [ProducesResponseType(typeof(CoreResponse<InternCampaignSummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddInternCampaignAsync(Guid id, Guid campaignId, AddInternCampaignApiRequest addInternCampaignApiRequest)
        {
            internsControllerLogger.LogInformationMethod(nameof(InternsController), nameof(AddInternCampaignAsync));

            var addInternCampaignRequest = new AddInternCampaignRequest(
                id,
                campaignId,
                addInternCampaignApiRequest.SpecialityId,
                addInternCampaignApiRequest.Justification);

            await internValidator.ValidateAndThrowAsync(addInternCampaignRequest);

            var internCampaignResponse = await internCampaignsService.AddInternCampaignAsync(addInternCampaignRequest);

            return CoreResult.Success(internCampaignResponse);
        }

        [HttpPut("{id}/campaigns/{campaignId}")]
        [ProducesResponseType(typeof(CoreResponse<InternCampaignSummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateInternCampaignAsync(Guid id, Guid campaignId, UpdateInternCampaignApiRequest updateInternCampaignApiRequest)
        {
            internsControllerLogger.LogInformationMethod(nameof(InternsController), nameof(UpdateInternCampaignAsync));

            var updateInternCampaignRequest = new UpdateInternCampaignRequest(
                id,
                campaignId,
                updateInternCampaignApiRequest.SpecialityId);

            await internValidator.ValidateAndThrowAsync(updateInternCampaignRequest);

            var internCampaignResponse = await internCampaignsService.UpdateInternCampaignAsync(updateInternCampaignRequest);

            return CoreResult.Success(internCampaignResponse);
        }

        [HttpPost("{id}/campaigns/{campaignId}/status")]
        [ProducesResponseType(typeof(CoreResponse<StateResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddStateAsync(Guid id, Guid campaignId, AddStateApiRequest addStateApiRequest)
        {
            internsControllerLogger.LogInformationMethod(nameof(InternsController), nameof(AddStateAsync));

            await InviteInternIfStatusIntern(id, addStateApiRequest);

            var addStateRequest = new AddStateRequest(
                id,
                campaignId,
                addStateApiRequest.StatusId,
                addStateApiRequest.Justification);

            await internValidator.ValidateAndThrowAsync(addStateRequest);

            var stateResponse = await internCampaignsService.AddStateAsync(addStateRequest);

            return CoreResult.Success(stateResponse);
        }   

        [HttpGet("status")]
        [ProducesResponseType(typeof(CoreResponse<IEnumerable<StatusResponse>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllStatusAsync()
        {
            internsControllerLogger.LogInformationMethod(nameof(InternsController), nameof(GetAllAsync));

            var statusResponseCollelction = await internCampaignsService.GetAllStatusAsync();

            return CoreResult.Success(statusResponseCollelction);
        }

        private async Task InviteInternIfStatusIntern(Guid id, AddStateApiRequest addStateApiRequest)
        {
            if (addStateApiRequest.StatusId != StatusId.Intern)
            {
                return;
            }

            var inviteInternRequest = new InviteInternRequest(
                id,
                addStateApiRequest.WorkEmail!,
                invitationUrls.BackOfficeUrl);

            await internValidator.ValidateAndThrowAsync(inviteInternRequest);

            await internsService.InviteAsync(inviteInternRequest);
        }
    }
}
