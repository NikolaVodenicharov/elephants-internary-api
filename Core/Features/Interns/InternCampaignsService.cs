using Core.Common;
using Core.Common.Exceptions;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Interns.Entities;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;
using Core.Features.Interns.Support;
using Core.Features.Persons.Entities;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialties.Entities;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Core.Features.Interns
{
    public class InternCampaignsService : IInternCampaignsService
    {
        private readonly IInternsRepository internsRepository;
        private readonly ICampaignsRepository campaignRepository;
        private readonly ISpecialitiesRepository specialitiesRepository;
        private readonly ILogger<InternCampaignsService> internCampaignsServiceLogger;
        private readonly IInternValidator internValidator;

        public InternCampaignsService(
            IInternsRepository internsRepository,
            ICampaignsRepository campaignRepository,
            ISpecialitiesRepository specialitiesRepository, 
            ILogger<InternCampaignsService> internsServiceLogger,
            IInternValidator internValidator)
        {
            this.internsRepository = internsRepository;
            this.campaignRepository = campaignRepository;
            this.specialitiesRepository = specialitiesRepository;
            this.internCampaignsServiceLogger = internsServiceLogger;
            this.internValidator = internValidator;
        }

        public async Task<InternCampaignSummaryResponse> AddInternCampaignAsync(AddInternCampaignRequest addInternCampaignRequest)
        {
            await AddInternCampaignAsyncValidations(addInternCampaignRequest);

            var internCampaign = await CreateInternCampaignAsync(
                addInternCampaignRequest.CampaignId,
                addInternCampaignRequest.SpecialityId,
                addInternCampaignRequest.Justification);

            var addInternCampaignRepoRequest = new AddInternCampaignRepoRequest(
                addInternCampaignRequest.InternId,
                internCampaign);

            await internsRepository.AddInternCampaignAsync(addInternCampaignRepoRequest);

            internCampaignsServiceLogger.LogInformationMethod(nameof(InternCampaignsService), nameof(AddInternCampaignAsync), true);

            return internCampaign.ToInternCampaignResponse();
        }

        public async Task<StateResponse> AddStateAsync(AddStateRequest addStateRequest)
        {
            await internValidator.ValidateAndThrowAsync(addStateRequest);

            var internCampaign = await GetValidInternCampaignAsync(
                addStateRequest.InternId, 
                addStateRequest.CampaignId);

            var state = CreateState(addStateRequest.StatusId, addStateRequest.Justification);

            internCampaign.States.Add(state);

            await internsRepository.UpdateInternCampaignAsync(internCampaign);

            internCampaignsServiceLogger.LogInformationMethod(nameof(InternCampaignsService), nameof(AddStateAsync), true);

            return state.ToStateResponse();
        }

        public async Task<InternCampaignSummaryResponse> UpdateInternCampaignAsync(UpdateInternCampaignRequest updateInternCampaignRequest)
        {
            await internValidator.ValidateAndThrowAsync(updateInternCampaignRequest);

            var internCampaign = await GetValidInternCampaignAsync(
                updateInternCampaignRequest.InternId,
                updateInternCampaignRequest.CampaignId);

            await UpdateSpecialityForInternCampaign(updateInternCampaignRequest.SpecialityId, internCampaign);

            var internCampaignSummaryResponse = await internsRepository.UpdateInternCampaignAsync(internCampaign);

            internCampaignsServiceLogger.LogInformationMethod(nameof(InternCampaignsService), nameof(UpdateInternCampaignAsync), true);

            return internCampaignSummaryResponse;
        }

        public async Task<InternCampaign> CreateInternCampaignAsync(Guid campaignId, Guid specialityId, string justificaton)
        {
            var campaign = await GetValidCampaignByIdAsync(campaignId);

            var speciality = await GetValidSpecialityByIdAsync(specialityId);

            var state = CreateState(StatusId.Candidate, justificaton);

            var internCampaign = new InternCampaign()
            {
                Campaign = campaign,
                Speciality = speciality,
                States = new List<State>() { state }
            };

            internCampaignsServiceLogger.LogInformationMethod(nameof(InternCampaignsService), nameof(CreateInternCampaignAsync), true);

            return internCampaign;
        }

        public async Task<IEnumerable<StatusResponse>> GetAllStatusAsync()
        {
            var statusResponseCollection = await internsRepository.GetAllStatusAsync();

            internCampaignsServiceLogger.LogInformationMethod(nameof(InternCampaignsService), nameof(GetAllStatusAsync), true);

            return statusResponseCollection;
        }

        private static State CreateState(StatusId statusId, string justification)
        {
            var state = new State()
            {
                StatusId = statusId,
                Justification = justification,
                Created = DateTime.UtcNow,
            };

            return state;
        }

        private async Task<Campaign> GetValidCampaignByIdAsync(Guid id)
        {
            var campaign = await campaignRepository.GetByIdAsync(id);

            Guard.EnsureNotNull(campaign, internCampaignsServiceLogger, nameof(InternCampaignsService),
                nameof(Campaign), id);

            return campaign;
        }

        private async Task<Speciality> GetValidSpecialityByIdAsync(Guid id)
        {
            var speciality = await specialitiesRepository.GetByIdAsync(id);

            Guard.EnsureNotNull(speciality, internCampaignsServiceLogger, nameof(InternCampaignsService),
                nameof(Speciality), id);

            return speciality;
        }

        private async Task<InternCampaign> GetValidInternCampaignAsync(Guid internId, Guid campaignId)
        {
            var internCampaign = await internsRepository.GetInternCampaignByIdsAsync(internId, campaignId);

            Guard.EnsureNotNull(internCampaign, internCampaignsServiceLogger, nameof(InternCampaignsService),
                nameof(InternCampaign));

            return internCampaign;
        }

        private async Task UpdateSpecialityForInternCampaign(Guid specialityId, InternCampaign internCampaign)
        {
            var isSpecialityChanged = internCampaign.SpecialityId != specialityId;

            if (isSpecialityChanged)
            {
                var speciality = await GetValidSpecialityByIdAsync(specialityId);

                internCampaign.Speciality = speciality;
            }
        }

        private async Task ValidateInternIsNotInCampaign(Guid internId, Guid campaignId)
        {
            var internCampaing = await internsRepository.GetInternCampaignByIdsAsync(internId, campaignId);

            if (internCampaing != null)
            {
                internCampaignsServiceLogger.LogError(
                    "[InternCampaignService] Person with id {internId} is already in that campaign with id {campaignId}.", 
                    internId, 
                    campaignId);

                throw new CoreException("Person is already in that campaign.", HttpStatusCode.BadRequest);
            }
        }

        private async Task AddInternCampaignAsyncValidations(AddInternCampaignRequest addInternCampaignRequest)
        {
            await internValidator.ValidateAndThrowAsync(addInternCampaignRequest);

            await ValidateInternIsNotInCampaign(addInternCampaignRequest.InternId, addInternCampaignRequest.CampaignId);

            var internSummaryResponse = await internsRepository.GetByIdAsync(addInternCampaignRequest.InternId);

            Guard.EnsureNotNull(
                internSummaryResponse, 
                internCampaignsServiceLogger, 
                nameof(InternCampaignsService), 
                nameof(Person), 
                addInternCampaignRequest.InternId);
        }
    }
}
