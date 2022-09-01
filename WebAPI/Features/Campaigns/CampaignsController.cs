using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
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

        public CampaignsController(ICampaignsService campaignsService, IValidator<CreateCampaign> createCampaingValidator)
        {
            this.campaignsService = campaignsService;
            this.createCampaingValidator = createCampaingValidator;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CampaignSummary))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ErrorResponse))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> Create(CreateCampaign model)
        {
            await createCampaingValidator.ValidateAndThrowAsync(model);

            var result = await campaignsService.CreateAsync(model);

            return Ok(result);
        }
    }
}
