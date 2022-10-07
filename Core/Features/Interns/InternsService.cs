using Core.Common;
using Core.Common.Pagination;
using Core.Features.Interns.Entities;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;
using Core.Features.Interns.Support;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Core.Features.Interns
{
    public class InternsService : IInternsService
    {
        private readonly IInternsRepository internsRepository;
        private readonly IInternCampaignsService internCampaignsService;
        private readonly ILogger<InternsService> internsServiceLogger;
        private readonly IValidator<CreateInternRequest> createInternRequestValidator;
        private readonly IValidator<UpdateInternRequest> updateInternRequestValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;

        public InternsService(
            IInternsRepository internsRepository,
            IInternCampaignsService internCampaignsService,
            ILogger<InternsService> internsServiceLogger,
            IValidator<CreateInternRequest> createInternRequestValidator,
            IValidator<UpdateInternRequest> updateInternRequestValidator,
            IValidator<PaginationRequest> paginationRequestValidator)
        {
            this.internsRepository = internsRepository;
            this.internCampaignsService = internCampaignsService;
            this.internsServiceLogger = internsServiceLogger;
            this.createInternRequestValidator = createInternRequestValidator;
            this.updateInternRequestValidator = updateInternRequestValidator;
            this.paginationRequestValidator = paginationRequestValidator;
        }

        public async Task<InternSummaryResponse> CreateAsync(CreateInternRequest createInternRequest)
        {
            await createInternRequestValidator.ValidateAndThrowAsync(createInternRequest);

            await ValidateNoEmailDuplication(createInternRequest.Email);

            var internCampaign = await internCampaignsService.CreateInternCampaignAsync(
                createInternRequest.CampaignId,
                createInternRequest.SpecialityId,
                createInternRequest.Justification);

            var intern = new Intern()
            {
                FirstName = createInternRequest.FirstName,
                LastName = createInternRequest.LastName,
                PersonalEmail = createInternRequest.Email,
                InternCampaigns = new List<InternCampaign>() { internCampaign }
            };

            var internStoredResponse = await internsRepository.AddAsync(intern);

            internsServiceLogger.LogInformationMethod(nameof(InternsService), nameof(CreateAsync), true);

            return internStoredResponse;
        }

        public async Task<InternSummaryResponse> UpdateAsync(UpdateInternRequest updateInternRequest)
        {
            await updateInternRequestValidator.ValidateAndThrowAsync(updateInternRequest);

            var intern = await GetByIdAsync(updateInternRequest.Id);

            await UpdateEmail(updateInternRequest.Email, intern);

            intern.FirstName = updateInternRequest.FirstName;
            intern.LastName = updateInternRequest.LastName;

            await internsRepository.SaveTrackingChangesAsync();

            internsServiceLogger.LogInformationMethod(nameof(InternsService), nameof(UpdateAsync), 
                nameof(Intern), updateInternRequest.Id, true);

            return intern.ToInternSummaryResponse();
        }

        public async Task<PaginationResponse<InternSummaryResponse>> GetAllAsync(PaginationRequest paginationRequest)
        {
            await paginationRequestValidator.ValidateAndThrowAsync(paginationRequest);

            var internPaginationResponse = await internsRepository.GetAllAsync(paginationRequest);

            internsServiceLogger.LogInformationMethod(nameof(InternsService), nameof(GetAllAsync), true);

            return internPaginationResponse;
        }

        public async Task<PaginationResponse<InternByCampaignSummaryResponse>> GetAllByCampaignIdAsync(PaginationRequest paginationRequest, 
            Guid campaignId)
        {
            await paginationRequestValidator.ValidateAndThrowAsync(paginationRequest);

            var internsByCampaignPaginationResponse = await internsRepository.GetAllByCampaignIdAsync(paginationRequest, campaignId);

            internsServiceLogger.LogInformationMethod(nameof(InternsService), nameof(GetAllByCampaignIdAsync), true);

            return internsByCampaignPaginationResponse;
        }

        public async Task<InternDetailsResponse> GetDetailsByIdAsync(Guid id)
        {
            var internDetailsResponse = await internsRepository.GetDetailsByIdAsync(id);

            Guard.EnsureNotNull(internDetailsResponse, internsServiceLogger,
                nameof(InternsService), nameof(Intern), id);

            internsServiceLogger.LogInformationMethod(nameof(InternsService), nameof(GetDetailsByIdAsync),
                nameof(Intern), id, true);

            return internDetailsResponse;
        }

        private async Task<Intern> GetByIdAsync(Guid id)
        {
            var intern = await internsRepository.GetByIdAsync(id);

            Guard.EnsureNotNull(intern, internsServiceLogger, nameof(InternsService),
                nameof(Intern), id);

            return intern;
        }

        private async Task UpdateEmail(string email, Intern intern)
        {
            var isEmailChanged = intern.PersonalEmail != email;

            if (!isEmailChanged)
            {
                return;
            }

            await ValidateNoEmailDuplication(email);

            intern.PersonalEmail = email;
        }

        private async Task ValidateNoEmailDuplication(string email)
        {
            var emailExist = await internsRepository.ExistsByEmailAsync(email);

            if (emailExist)
            {
                internsServiceLogger.LogErrorAndThrowExceptionValueTaken(nameof(InternsService), nameof(Intern),
                    nameof(Intern.PersonalEmail), email);
            }
        }
    }
}
