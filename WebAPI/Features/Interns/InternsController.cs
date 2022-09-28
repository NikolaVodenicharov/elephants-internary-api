using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Interns.Entities;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebAPI.Common;
using WebAPI.Common.Abstractions;
using WebAPI.Features.Interns.ApiRequestModels;

namespace WebAPI.Features.Interns
{
    [Authorize]
    public class InternsController : ApiControllerBase
    {
        private readonly IInternsService internsService;
        private readonly IInternCampaignsService internCampaignsService;
        private readonly ILogger<InternsController> internsControllerLogger;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;
        private readonly IValidator<CreateInternRequest> createInternRequestValidator;
        private readonly IValidator<UpdateInternRequest> updateInternRequestValidator;
        private readonly IValidator<AddInternCampaignRequest> addInternCampaignRequestValidator;
        private readonly IValidator<UpdateInternCampaignRequest> updateInternCampaignRequestValidator;
        private readonly IValidator<AddStateRequest> addStateRequestValidator;

        public InternsController(
            IInternsService internsService, 
            IInternCampaignsService internCampaignsService,
            ILogger<InternsController> internsControllerLogger,
            IValidator<PaginationRequest> paginationRequestValidator,
            IValidator<CreateInternRequest> createInternRequestValidator,
            IValidator<UpdateInternRequest> updateInternRequestValidator,
            IValidator<AddInternCampaignRequest> addInternCampaignRequestValidator,
            IValidator<UpdateInternCampaignRequest> updateInternCampaignRequestValidator,
            IValidator<AddStateRequest> addStateRequestValidator)
        {
            this.internsService = internsService;
            this.internCampaignsService = internCampaignsService;
            this.internsControllerLogger = internsControllerLogger;
            this.paginationRequestValidator = paginationRequestValidator;
            this.createInternRequestValidator = createInternRequestValidator;
            this.updateInternRequestValidator = updateInternRequestValidator;
            this.addInternCampaignRequestValidator = addInternCampaignRequestValidator;
            this.updateInternCampaignRequestValidator = updateInternCampaignRequestValidator;
            this.addStateRequestValidator = addStateRequestValidator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CoreResponse<InternSummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAsync(CreateInternRequest createInternRequest)
        {
            LogInformation(nameof(CreateAsync));

            await createInternRequestValidator.ValidateAndThrowAsync(createInternRequest);

            var specialitySummaryResponse = await internsService.CreateAsync(createInternRequest);

            return CoreResult.Success(specialitySummaryResponse);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CoreResponse<InternSummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateInternApiRequest updateInternApiRequest)
        {
            LogInformation(nameof(UpdateAsync));

            var updateInternRequest = new UpdateInternRequest(
                id,
                updateInternApiRequest.FirstName,
                updateInternApiRequest.LastName,
                updateInternApiRequest.Email);

            await updateInternRequestValidator.ValidateAndThrowAsync(updateInternRequest);

            var internSummaryResponse = await internsService.UpdateAsync(updateInternRequest);

            return CoreResult.Success(internSummaryResponse);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CoreResponse<InternDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetDetailsByIdAsync(Guid id)
        {
            LogInformation(nameof(GetDetailsByIdAsync));

            if (id == Guid.Empty)
            {
                internsControllerLogger.LogError($"[{nameof(InternsController)}] Invalid {nameof(Intern)} Id ({id}) in {nameof(GetDetailsByIdAsync)} method.");

                throw new CoreException($"Invalid {nameof(id)}.", HttpStatusCode.BadRequest);
            }

            var internDetailsResponse = await internsService.GetDetailsByIdAsync(id);

            return CoreResult.Success(internDetailsResponse);
        }

        [HttpGet]
        [ProducesResponseType(typeof(CoreResponse<PaginationResponse<InternSummaryResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllAsync([Required][FromQuery] int pageNum, [Required][FromQuery] int pageSize)
        {
            LogInformation(nameof(GetAllAsync));

            var paginationRequest = new PaginationRequest(pageNum, pageSize);

            await paginationRequestValidator.ValidateAndThrowAsync(paginationRequest);

            var paginationResponse = await internsService.GetAllAsync(paginationRequest);

            return CoreResult.Success(paginationResponse);
        }

        [HttpPost("{id}/campaigns/{campaignId}")]
        [ProducesResponseType(typeof(CoreResponse<InternCampaignSummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddInternCampaignAsync(Guid id, Guid campaignId, AddInternCampaignApiRequest addInternCampaignApiRequest)
        {
            LogInformation(nameof(AddInternCampaignAsync));

            var addInternCampaignRequest = new AddInternCampaignRequest(
                id,
                campaignId,
                addInternCampaignApiRequest.SpecialityId,
                addInternCampaignApiRequest.Justification);

            await addInternCampaignRequestValidator.ValidateAndThrowAsync(addInternCampaignRequest);

            var internCampaignResponse = await internCampaignsService.AddInternCampaignAsync(addInternCampaignRequest);

            return CoreResult.Success(internCampaignResponse);
        }

        [HttpPut("{id}/campaigns/{campaignId}")]
        [ProducesResponseType(typeof(CoreResponse<InternCampaignSummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateInternCampaignAsync(Guid id, Guid campaignId, UpdateInternCampaignApiRequest updateInternCampaignApiRequest)
        {
            LogInformation(nameof(UpdateInternCampaignAsync));

            var updateInternCampaignRequest = new UpdateInternCampaignRequest(
                id,
                campaignId,
                updateInternCampaignApiRequest.SpecialityId);

            await updateInternCampaignRequestValidator.ValidateAndThrowAsync(updateInternCampaignRequest);

            var internCampaignResponse = await internCampaignsService.UpdateInternCampaignAsync(updateInternCampaignRequest);

            return CoreResult.Success(internCampaignResponse);
        }

        [HttpPost("{id}/campaigns/{campaignId}/status")]
        [ProducesResponseType(typeof(CoreResponse<StateResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddStateAsync(Guid id, Guid campaignId, AddStateApiRequest addStateApiRequest)
        {
            LogInformation(nameof(AddStateAsync));

            var addStateRequest = new AddStateRequest(
                id,
                campaignId,
                addStateApiRequest.StatusId,
                addStateApiRequest.Justification);

            await addStateRequestValidator.ValidateAndThrowAsync(addStateRequest);

            var stateResponse = await internCampaignsService.AddStateAsync(addStateRequest);

            return CoreResult.Success(stateResponse);
        }


        [HttpGet("status")]
        [ProducesResponseType(typeof(CoreResponse<IEnumerable<StatusResponse>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllStatusAsync()
        {
            LogInformation(nameof(GetAllAsync));

            var statusResponseCollelction = await internCampaignsService.GetAllStatusAsync();

            return CoreResult.Success(statusResponseCollelction);
        }

        private void LogInformation(string methodName)
        {
            internsControllerLogger.LogInformation("[{ControllerName}] {methodName} executing.", nameof(InternsController), methodName);
        }
    }
}
