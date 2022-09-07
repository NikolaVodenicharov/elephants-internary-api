using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
using Core.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebAPI.Common.Abstractions;
using WebAPI.Common.ExceptionHandling;

namespace WebAPI.Features.Campaigns
{
    public class CampaignsController : ApiControllerBase
    {
        private readonly ICampaignsService campaignsService;
        private readonly IValidator<CreateCampaign> createCampaingValidator;
        private readonly IValidator<UpdateCampaign> updateCampaignValidator;
        private readonly ILogger<CampaignsController> campaignsLogger;

        public CampaignsController(
            ICampaignsService campaignsService, 
            IValidator<CreateCampaign> createCampaingValidator, 
            IValidator<UpdateCampaign> updateCampaignValidator, 
            ILogger<CampaignsController> campaignsLogger)
        {
            this.campaignsService = campaignsService;
            this.createCampaingValidator = createCampaingValidator;
            this.updateCampaignValidator = updateCampaignValidator;
            this.campaignsLogger = campaignsLogger;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CampaignSummary))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> Create(CreateCampaign model)
        {
            await createCampaingValidator.ValidateAndThrowAsync(model);

            var result = await campaignsService.CreateAsync(model);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CampaignSummary))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> Update(Guid id, UpdateCampaign model)
        {
            if (id != model.Id)
            {
                campaignsLogger.LogError($"[{DateTime.UtcNow}] Invalid Campaign Id ({id}) in Update request data.");
                
                throw new CoreException("Invalid Campaign Id in request data", HttpStatusCode.BadRequest);
            }
            
            await updateCampaignValidator.ValidateAndThrowAsync(model);

            var result = await campaignsService.UpdateAsync(model);

            return Ok(result);
        }
    }
}
