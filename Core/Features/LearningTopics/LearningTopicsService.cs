using Core.Common;
using Core.Common.Pagination;
using Core.Features.LearningTopics.Entities;
using Core.Features.LearningTopics.Interfaces;
using Core.Features.LearningTopics.RequestModels;
using Core.Features.LearningTopics.ResponseModels;
using Core.Features.LearningTopics.Support;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialties.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Core.Features.LearningTopics
{
    internal static class Counter
    {
        public static int learningTopicCount = -1;
    }

    public class LearningTopicsService : ILearningTopicsService
    {
        private readonly ILearningTopicsRepository learningTopicsRepository;
        private readonly ISpecialitiesRepository specialitiesRepository;
        private readonly ILogger<LearningTopicsService> learningTopicsServiceLogger;
        private readonly IValidator<CreateLearningTopicRequest> createLearningTopicValidator;
        private readonly IValidator<UpdateLearningTopicRequest> updateLearningTopicValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;

        public LearningTopicsService(
            ILearningTopicsRepository learningTopicsRepository,
            ISpecialitiesRepository specialitiesRepository,
            ILogger<LearningTopicsService> learningTopicsServiceLogger,
            IValidator<CreateLearningTopicRequest> createLearningTopicValidator,
            IValidator<UpdateLearningTopicRequest> updateLearningTopicValidator,
            IValidator<PaginationRequest> paginationRequestValidator)
        {
            this.learningTopicsRepository = learningTopicsRepository;
            this.specialitiesRepository = specialitiesRepository;
            this.learningTopicsServiceLogger = learningTopicsServiceLogger;
            this.createLearningTopicValidator = createLearningTopicValidator;
            this.updateLearningTopicValidator = updateLearningTopicValidator;
            this.paginationRequestValidator = paginationRequestValidator;
        }

        public async Task<LearningTopicSummaryResponse> CreateAsync(CreateLearningTopicRequest request)
        {
            await createLearningTopicValidator.ValidateAndThrowAsync(request);

            await ValidateDuplicateNameAsync(request.Name);

            var specialities = await ValidateAndGetSpecialitiesById(request.SpecialityIds);
            
            var learningTopic = request.ToLearningTopic();
            learningTopic.Specialities = specialities;

            var learningTopicResponse = await learningTopicsRepository.AddAsync(learningTopic);

            learningTopicsServiceLogger.LogInformationMethod(nameof(LearningTopicsService), nameof(CreateAsync), true);

            return learningTopicResponse.ToLearningTopicSummary();
        }

        public async Task<LearningTopicSummaryResponse> UpdateAsync(UpdateLearningTopicRequest request)
        {
            await updateLearningTopicValidator.ValidateAndThrowAsync(request);

            var existingLearningTopic = await learningTopicsRepository.GetByIdAsync(request.Id);

            Guard.EnsureNotNull(existingLearningTopic, learningTopicsServiceLogger, nameof(LearningTopicsService),
                nameof(LearningTopic), request.Id);

            var hasNameChanged = !existingLearningTopic.Name.Equals(request.Name);

            if(hasNameChanged)
            {
                await ValidateDuplicateNameAsync(request.Name);
            }

            var specialities = await ValidateAndGetSpecialitiesById(request.SpecialityIds);

            existingLearningTopic.Name = request.Name;
            existingLearningTopic.Specialities = specialities;

            await learningTopicsRepository.SaveTrackingChangesAsync();

            learningTopicsServiceLogger.LogInformationMethod(nameof(LearningTopicsService), nameof(UpdateAsync), 
                nameof(LearningTopic), existingLearningTopic.Id, true);

            return existingLearningTopic.ToLearningTopicSummary();
        }

        public async Task<LearningTopicSummaryResponse> GetByIdAsync(Guid id)
        {
            var learningTopicResponse = await learningTopicsRepository.GetByIdAsync(id);

            Guard.EnsureNotNull(learningTopicResponse, learningTopicsServiceLogger,
                nameof(LearningTopicsService), nameof(LearningTopic), id);

            learningTopicsServiceLogger.LogInformationMethod(nameof(LearningTopicsService), nameof(GetByIdAsync), 
                nameof(LearningTopic), id, true);

            return learningTopicResponse.ToLearningTopicSummary();
        }

        public async Task<IEnumerable<LearningTopicSummaryResponse>> GetAllAsync()
        {
            var learningTopicsResponse = await learningTopicsRepository.GetAllAsync();

            learningTopicsServiceLogger.LogInformationMethod(nameof(LearningTopicsService), nameof(GetAllAsync), true);

            return learningTopicsResponse.ToLearningTopicSummaries();
        }

        public async Task<PaginationResponse<LearningTopicSummaryResponse>> GetPaginationAsync(PaginationRequest filter)
        {
            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            Guard.EnsureNotNullPagination(filter.PageNum, filter.PageSize, learningTopicsServiceLogger,
                nameof(LearningTopicsService));

            if (Counter.learningTopicCount == -1 || filter.PageNum == 1)
            {
                Counter.learningTopicCount = await learningTopicsRepository.GetCountAsync();
            }

            if (Counter.learningTopicCount == 0)
            {
                if (filter.PageNum > PaginationConstants.DefaultPageCount)
                {
                    learningTopicsServiceLogger.LogErrorAndThrowExceptionPageCount(nameof(LearningTopicsService), 
                        PaginationConstants.DefaultPageCount, filter.PageNum.Value);
                }

                var emptyPaginationResponse = new PaginationResponse<LearningTopicSummaryResponse>(
                    new List<LearningTopicSummaryResponse>(), filter.PageNum.Value, PaginationConstants.DefaultPageCount);

                return emptyPaginationResponse;
            }

            var totalPages = (Counter.learningTopicCount + filter.PageSize.Value - 1) / filter.PageSize.Value;

            if (filter.PageNum > totalPages)
            {
                learningTopicsServiceLogger.LogErrorAndThrowExceptionPageCount(nameof(LearningTopicsService), 
                    totalPages, filter.PageNum.Value);
            }

            var learningTopics = await learningTopicsRepository.GetAllAsync(filter);

            var paginationResponse = new PaginationResponse<LearningTopicSummaryResponse>(
                learningTopics.ToLearningTopicSummaries(), filter.PageNum.Value, totalPages);

            learningTopicsServiceLogger.LogInformationMethod(nameof(LearningTopicsService), nameof(GetPaginationAsync), true);

            return paginationResponse;
        }

        private async Task ValidateDuplicateNameAsync(string learningTopicName)
        {
            var nameExists = await learningTopicsRepository.ExistsByNameAsync(learningTopicName);

            if (nameExists)
            {
                learningTopicsServiceLogger.LogErrorAndThrowExceptionValueTaken(nameof(LearningTopicsService), nameof(LearningTopic),
                    nameof(LearningTopic.Name), learningTopicName);
            }
        }

        private async Task<ICollection<Speciality>> ValidateAndGetSpecialitiesById(ICollection<Guid> specialityIds)
        {
            if(specialityIds.Count() != specialityIds.Distinct().Count())
            {
                learningTopicsServiceLogger.LogErrorAndThrowExceptionDuplicateEntries(nameof(LearningTopicsService), nameof(LearningTopic),
                    "specialities", specialityIds);
            }

            var specialities = await specialitiesRepository.GetByIdsAsync(specialityIds);
            
            if(specialities.Count() != specialityIds.Count())
            {
                learningTopicsServiceLogger.LogErrorAndThrowExceptionNotAllFound(nameof(LearningTopicsService), 
                    "specialities", specialityIds);
            }

            return specialities;
        }
    }
}