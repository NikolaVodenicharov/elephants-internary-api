﻿using Core.Common;
using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Interfaces;
using Core.Features.Mentors.Entities;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Mentors.Support;
using Core.Features.Specialities.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Core.Features.Mentors
{
    internal class Counter
    {
        public static int mentorCount = -1;
        public static Dictionary<Guid, int> countByCampaign = new();
    }
    public class MentorsService : IMentorsService
    {
        private readonly IMentorsRepository mentorsRepository;
        private readonly ICampaignsRepository campaignsRepository;
        private readonly ISpecialitiesRepository specialitiesRepository;
        private readonly ILogger<MentorsService> mentorsServiceLogger;
        private readonly IValidator<CreateMentorRequest> createMentorRequestValidator;
        private readonly IValidator<UpdateMentorRequest> updateMentorRequestValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;

        
        public MentorsService(
            IMentorsRepository mentorsRepository,
            ICampaignsRepository campaignsRepository,
            ISpecialitiesRepository specialitiesRepository,
            ILogger<MentorsService> mentorsServiceLogger,
            IValidator<CreateMentorRequest> createMentorRequestValidator,
            IValidator<UpdateMentorRequest> updateMentorRequestValidator,
            IValidator<PaginationRequest> paginationRequestValidator)
        {
            this.mentorsRepository = mentorsRepository;
            this.campaignsRepository = campaignsRepository;
            this.specialitiesRepository = specialitiesRepository;
            this.mentorsServiceLogger = mentorsServiceLogger;
            this.createMentorRequestValidator = createMentorRequestValidator;
            this.updateMentorRequestValidator = updateMentorRequestValidator;
            this.paginationRequestValidator = paginationRequestValidator;
        }

        public async Task<MentorSummaryResponse> CreateAsync(CreateMentorRequest request)
        {
            await createMentorRequestValidator.ValidateAndThrowAsync(request);

            await CheckEmailAsync(request.Email);

            if (request.SpecialityIds.Count() != request.SpecialityIds.Distinct().Count())
            {
                mentorsServiceLogger.LogErrorAndThrowExceptionDuplicateEntries(nameof(MentorsService), nameof(Mentor),
                    "specialities", request.SpecialityIds);
            }

            var mentorSpecialities = await specialitiesRepository.GetByIdsAsync(request.SpecialityIds);

            if (mentorSpecialities.Count != request.SpecialityIds.Count())
            {
                mentorsServiceLogger.LogErrorAndThrowExceptionNotAllFound(nameof(MentorsService), "specialities", request.SpecialityIds);
            }

            var mentor = request.ToMentor();
            mentor.Specialities = mentorSpecialities;

            var createdMentor = await mentorsRepository.AddAsync(mentor);

            mentorsServiceLogger.LogInformationMethod(nameof(MentorsService), nameof(CreateAsync), true);

            return createdMentor.ToMentorSummaryResponse();
        }

        public async Task<PaginationResponse<MentorSummaryResponse>> GetPaginationAsync(PaginationRequest filter, Guid? campaignId = null)
        {
            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            Guard.EnsureNotNullPagination(filter.PageNum, filter.PageSize, mentorsServiceLogger,
                nameof(MentorsService));

            if (campaignId != null)
            {
                if (!Counter.countByCampaign.ContainsKey(campaignId.Value) || filter.PageNum == 1)
                {
                    Counter.countByCampaign[campaignId.Value] = await GetCountByCampaignIdAsync(campaignId.Value);
                }

                if (Counter.countByCampaign[campaignId.Value] == 0)
                {
                    if (filter.PageNum > PaginationConstants.DefaultPageCount)
                    {
                        mentorsServiceLogger.LogErrorAndThrowExceptionPageCount(nameof(MentorsService), 
                            PaginationConstants.DefaultPageCount, filter.PageNum.Value);
                    }

                    var emptyPaginationResponse = new PaginationResponse<MentorSummaryResponse>(
                        new List<MentorSummaryResponse>(), filter.PageNum.Value, PaginationConstants.DefaultPageCount);

                    mentorsServiceLogger.LogInformationMethod(nameof(MentorsService), nameof(GetPaginationAsync), true);

                    return emptyPaginationResponse;
                }
            }
            else
            {
                if (Counter.mentorCount == -1 || filter.PageNum == 1)
                {
                    Counter.mentorCount = await GetCountAsync();
                }

                if (Counter.mentorCount == 0)
                {
                    if (filter.PageNum > PaginationConstants.DefaultPageCount)
                    {
                        mentorsServiceLogger.LogErrorAndThrowExceptionPageCount(nameof(MentorsService), 
                            PaginationConstants.DefaultPageCount, filter.PageNum.Value);
                    }

                    var emptyPaginationResponse = new PaginationResponse<MentorSummaryResponse>(
                        new List<MentorSummaryResponse>(), filter.PageNum.Value, PaginationConstants.DefaultPageCount);

                    mentorsServiceLogger.LogInformationMethod(nameof(MentorsService), nameof(GetPaginationAsync), true);

                    return emptyPaginationResponse;
                }
            }

            var elementCount = campaignId != null ? Counter.countByCampaign[campaignId.Value] : Counter.mentorCount;

            var totalPages = (elementCount + filter.PageSize.Value - 1) / filter.PageSize.Value;

            if (filter.PageNum > totalPages)
            {
                mentorsServiceLogger.LogErrorAndThrowExceptionPageCount(nameof(MentorsService), totalPages, 
                    filter.PageNum.Value);
            }

            var mentors = campaignId != null ? 
                await mentorsRepository.GetAllAsync(filter, campaignId) : 
                await mentorsRepository.GetAllAsync(filter);

            var paginationResponse = new PaginationResponse<MentorSummaryResponse>(
                mentors.ToMentorSummaryResponses(), filter.PageNum.Value, totalPages);

            mentorsServiceLogger.LogInformationMethod(nameof(MentorsService), nameof(GetPaginationAsync), true);

            return paginationResponse;
        }

        public async Task<IEnumerable<MentorSummaryResponse>> GetAllAsync()
        {
            var mentors = await mentorsRepository.GetAllAsync();

            mentorsServiceLogger.LogInformationMethod(nameof(MentorsService), nameof(GetAllAsync), true);

            return mentors.ToMentorSummaryResponses();
        }

        public async Task<MentorSummaryResponse> GetByIdAsync(Guid id)
        {
            var mentor = await mentorsRepository.GetByIdAsync(id);

            Guard.EnsureNotNull(mentor, mentorsServiceLogger, nameof(MentorsService), nameof(Mentor), id);

            mentorsServiceLogger.LogInformationMethod(nameof(MentorsService), nameof(GetByIdAsync), true);

            return mentor.ToMentorSummaryResponse();
        }

        public async Task<int> GetCountAsync()
        {
            var mentorCount = await mentorsRepository.GetCountAsync();

            return mentorCount;
        }

        public async Task<MentorSummaryResponse> UpdateAsync(UpdateMentorRequest request)
        {
            await updateMentorRequestValidator.ValidateAndThrowAsync(request);

            var existingMentor = await mentorsRepository.GetByIdAsync(request.Id);

            Guard.EnsureNotNull(existingMentor, mentorsServiceLogger, nameof(MentorsService), nameof(Mentor), request.Id);

            var isEmailChanged = !existingMentor.Email.Equals(request.Email);

            if (isEmailChanged)
            {
                await CheckEmailAsync(request.Email);
            }

            if (request.SpecialityIds.Count() != request.SpecialityIds.Distinct().Count())
            {
                mentorsServiceLogger.LogErrorAndThrowExceptionDuplicateEntries(nameof(MentorsService), nameof(Mentor),
                    "specialities", request.SpecialityIds);
            }

            var mentorSpecialities = await specialitiesRepository.GetByIdsAsync(request.SpecialityIds);

            if (mentorSpecialities.Count != request.SpecialityIds.Count())
            {
                mentorsServiceLogger.LogErrorAndThrowExceptionNotAllFound(nameof(MentorsService), "specialities", request.SpecialityIds);
            }

            existingMentor.FirstName = request.FirstName;
            existingMentor.LastName = request.LastName;
            existingMentor.Email = request.Email;
            existingMentor.Specialities = mentorSpecialities;

            await mentorsRepository.SaveTrackingChangesAsync();

            mentorsServiceLogger.LogInformationMethod(nameof(MentorsService), nameof(UpdateAsync), nameof(Mentor), 
                request.Id, true);

            return existingMentor.ToMentorSummaryResponse();
        }  

        public async Task<int> GetCountByCampaignIdAsync(Guid campaignId)
        {
            var count = await mentorsRepository.GetCountByCampaignIdAsync(campaignId);

            return count;
        }

        public async Task AddToCampaignAsync(AddToCampaignRequest request)
        {
            var campaign = await campaignsRepository.GetByIdAsync(request.CampaignId);

            Guard.EnsureNotNull(campaign, mentorsServiceLogger, nameof(MentorsService), nameof(Campaign), request.CampaignId);

            var mentor = await mentorsRepository.GetByIdAsync(request.PersonId);

            Guard.EnsureNotNull(mentor, mentorsServiceLogger, nameof(MentorsService), nameof(Mentor), request.PersonId);

            if (mentor.Campaigns.Contains(campaign))
            {
                mentorsServiceLogger.LogError("[{ServiceName}] Mentor with Id {MentorId} is already assigned to" +
                    " campaign with Id {CampaignId}", nameof(MentorsService), nameof(request.PersonId), nameof(request.CampaignId));

                throw new CoreException($"Mentor {mentor.FirstName} {mentor.LastName} ({mentor.Email}) " +
                    $"is already assigned to campaign '{campaign.Name}'.", HttpStatusCode.BadRequest);
            }

            mentor.Campaigns.Add(campaign);

            await mentorsRepository.SaveTrackingChangesAsync();

            mentorsServiceLogger.LogInformationAddedToEntity(nameof(MentorsService), nameof(Mentor), request.PersonId,
                nameof(Campaign), request.CampaignId);
        }

        private async Task CheckEmailAsync(string email)
        {
            var isEmailUsed = await mentorsRepository.IsEmailUsed(email);

            if (isEmailUsed)
            {
                mentorsServiceLogger.LogErrorAndThrowExceptionValueTaken(nameof(MentorsService), nameof(Mentor), 
                    nameof(Mentor.Email), email);
            }
        }
    }
}
