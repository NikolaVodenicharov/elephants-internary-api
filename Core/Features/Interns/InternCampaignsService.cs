using Core.Common;
using Core.Common.Exceptions;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Interns.Entities;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;
using Core.Features.Interns.Support;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialties.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Core.Features.Interns
{
    public class InternCampaignsService : IInternCampaignsService
    {
        private readonly IInternsRepository internsRepository;
        private readonly ICampaignsRepository campaignRepository;
        private readonly ISpecialitiesRepository specialitiesRepository;
        private readonly ILogger<InternCampaignsService> internCampaignsServiceLogger;
        private readonly IValidator<AddInternCampaignRequest> addInternCampaignRequestValidator;
        private readonly IValidator<UpdateInternCampaignRequest> updateInternCampaignRequestValidator;
        private readonly IValidator<AddStateRequest> addStateRequestValidator;

        public InternCampaignsService(
            IInternsRepository internsRepository,
            ICampaignsRepository campaignRepository,
            ISpecialitiesRepository specialitiesRepository, 
            ILogger<InternCampaignsService> internsServiceLogger,
            IValidator<AddInternCampaignRequest> addInternCampaignRequestValidator,
            IValidator<UpdateInternCampaignRequest> updateInternCampaignRequestValidator,
            IValidator<AddStateRequest> addStateRequestValidator)
        {
            this.internsRepository = internsRepository;
            this.campaignRepository = campaignRepository;
            this.specialitiesRepository = specialitiesRepository;
            this.internCampaignsServiceLogger = internsServiceLogger;
            this.addInternCampaignRequestValidator = addInternCampaignRequestValidator;
            this.updateInternCampaignRequestValidator = updateInternCampaignRequestValidator;
            this.addStateRequestValidator = addStateRequestValidator;
        }

        public async Task<InternCampaignSummaryResponse> AddInternCampaignAsync(AddInternCampaignRequest addInternCampaignRequest)
        {
            await addInternCampaignRequestValidator.ValidateAndThrowAsync(addInternCampaignRequest);

            await ValidateInternIsNotInCampaign(addInternCampaignRequest.InternId, addInternCampaignRequest.CampaignId);

            var intern = await GetValidInternByIdAsync(addInternCampaignRequest.InternId);

            var internCampaign = await CreateInternCampaignAsync(
                addInternCampaignRequest.CampaignId,
                addInternCampaignRequest.SpecialityId,
                addInternCampaignRequest.Justification);

            AddInternCampaingToIntern(intern, internCampaign);

            await internsRepository.SaveTrackingChangesAsync();

            internCampaignsServiceLogger.LogInformationMethod(nameof(InternCampaignsService), nameof(AddInternCampaignAsync), true);

            return internCampaign.ToInternCampaignResponse();
        }

        public async Task<StateResponse> AddStateAsync(AddStateRequest addStateRequest)
        {
            await addStateRequestValidator.ValidateAndThrowAsync(addStateRequest);

            var internCampaign = await GetValidInternCampaignAsync(addStateRequest.InternId, addStateRequest.CampaignId);

            var state = CreateState(addStateRequest.StatusId, addStateRequest.Justification);

            internCampaign.States.Add(state);

            await internsRepository.SaveTrackingChangesAsync();

            internCampaignsServiceLogger.LogInformationMethod(nameof(InternCampaignsService), nameof(AddStateAsync), true);

            return state.ToStateResponse();
        }

        public async Task<InternCampaignSummaryResponse> UpdateInternCampaignAsync(UpdateInternCampaignRequest updateInternCampaignRequest)
        {
            await updateInternCampaignRequestValidator.ValidateAndThrowAsync(updateInternCampaignRequest);

            var internCampaign = await GetValidInternCampaignAsync(
                updateInternCampaignRequest.InternId,
                updateInternCampaignRequest.CampaignId);

            await UpdateSpecialityForInternCampaign(updateInternCampaignRequest.SpecialityId, internCampaign);

            await internsRepository.SaveTrackingChangesAsync();

            internCampaignsServiceLogger.LogInformationMethod(nameof(InternCampaignsService), nameof(UpdateInternCampaignAsync), true);

            return internCampaign.ToInternCampaignResponse();
        }

        public async Task<InternCampaign> CreateInternCampaignAsync(Guid campaignId, Guid specialityId, string justificaton)
        {
            var campaign = await GetValidCampaignByIdAsync(campaignId);

            var speciality = await GetValidSpecialityByIdAsync(specialityId);

            var state = CreateState(StatusEnum.Candidate, justificaton);

            var internIntersection = new InternCampaign()
            {
                Campaign = campaign,
                Speciality = speciality,
                States = new List<State>() { state }
            };

            internCampaignsServiceLogger.LogInformationMethod(nameof(InternCampaignsService), nameof(CreateInternCampaignAsync), true);

            return internIntersection;
        }

        public async Task<IEnumerable<StatusResponse>> GetAllStatusAsync()
        {
            var statusResponseCollection = await internsRepository.GetAllStatusAsync();

            internCampaignsServiceLogger.LogInformationMethod(nameof(InternCampaignsService), nameof(GetAllStatusAsync), true);

            return statusResponseCollection;
        }

        private static State CreateState(StatusEnum statusId, string justification)
        {
            var state = new State()
            {
                StatusId = statusId,
                Justification = justification,
                Created = DateTime.UtcNow,
            };

            return state;
        }

        private async Task<Intern> GetValidInternByIdAsync(Guid id)
        {
            var intern = await internsRepository.GetByIdAsync(id);

            Guard.EnsureNotNull(intern, internCampaignsServiceLogger, nameof(InternCampaignsService), 
                nameof(Intern), id);

            return intern;
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

        private static void AddInternCampaingToIntern(Intern intern, InternCampaign internCampaign)
        {
            if (intern.InternCampaigns == null)
            {
                intern.InternCampaigns = new List<InternCampaign>();
            }

            intern.InternCampaigns.Add(internCampaign);
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
                var internInCampaignMessage = $"{nameof(Intern)} is already in that campaign.";

                LogError(internInCampaignMessage);

                throw new CoreException(internInCampaignMessage, HttpStatusCode.BadRequest);
            }
        }

        private void LogError(string message)
        {
            internCampaignsServiceLogger.LogError("[{ServiceName}] {message}", nameof(InternsService), message);
        }
    }
}
