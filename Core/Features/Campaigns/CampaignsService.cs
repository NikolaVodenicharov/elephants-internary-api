using Core.Common.Exceptions;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Campaigns.RequestModels;
using Core.Features.Campaigns.ResponseModels;
using Core.Features.Campaigns.Support;
using FluentValidation;
using System.Net;

namespace Core.Features.Campaigns
{
    public class CampaignsService : ICampaignsService
    {
        private readonly ICampaignsRepository repository;

        public CampaignsService(ICampaignsRepository repository)
        {
            this.repository = repository;
        }

        public async Task<CampaignSummary> CreateAsync(CreateCampaign model)
        {
            var validator = new CreateCampaignValidator();
            await validator.ValidateAndThrowAsync(model);

            var isExist = await repository.ExistsByNameAsync(model.Name);

            if (isExist)
            {
                throw new CoreException($"Campaign with name {model.Name} already exist.", HttpStatusCode.BadRequest);
            }

            var campaign = model.ToCampaign();

            var createdCampaign = await repository.AddAsync(campaign);

            return createdCampaign.ToCampaignSummary();
        }
    }
}
