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
    public class LearningTopicsService : ILearningTopicsService
    {
        private readonly ILearningTopicsRepository learningTopicsRepository;
        private readonly ISpecialitiesRepository specialitiesRepository;
        private readonly ILogger<LearningTopicsService> learningTopicsServiceLogger;
        private readonly ILearningTopicValidator learningTopicValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;

        public LearningTopicsService(
            ILearningTopicsRepository learningTopicsRepository,
            ISpecialitiesRepository specialitiesRepository,
            ILogger<LearningTopicsService> learningTopicsServiceLogger,
            ILearningTopicValidator learningTopicValidator,
            IValidator<PaginationRequest> paginationRequestValidator)
        {
            this.learningTopicsRepository = learningTopicsRepository;
            this.specialitiesRepository = specialitiesRepository;
            this.learningTopicsServiceLogger = learningTopicsServiceLogger;
            this.learningTopicValidator = learningTopicValidator;
            this.paginationRequestValidator = paginationRequestValidator;
        }

        public async Task<LearningTopicSummaryResponse> CreateAsync(CreateLearningTopicRequest createLearningTopic)
        {
            await learningTopicValidator.ValidateAndThrowAsync(createLearningTopic);

            await ValidateDuplicateNameAsync(createLearningTopic.Name);

            var specialities = await ValidateAndGetSpecialitiesById(createLearningTopic.SpecialityIds.Distinct());
            
            var learningTopic = createLearningTopic.ToLearningTopic();
            learningTopic.Specialities = specialities;

            var learningTopicResponse = await learningTopicsRepository.AddAsync(learningTopic);

            learningTopicsServiceLogger.LogInformationMethod(nameof(LearningTopicsService), nameof(CreateAsync), true);

            return learningTopicResponse.ToLearningTopicSummary();
        }

        public async Task<LearningTopicSummaryResponse> UpdateAsync(UpdateLearningTopicRequest updateLearningTopic)
        {
            await learningTopicValidator.ValidateAndThrowAsync(updateLearningTopic);

            var existingLearningTopic = await learningTopicsRepository.GetByIdAsync(updateLearningTopic.Id);

            Guard.EnsureNotNull(existingLearningTopic, learningTopicsServiceLogger, nameof(LearningTopicsService),
                nameof(LearningTopic), updateLearningTopic.Id);

            var hasNameChanged = !existingLearningTopic.Name.Equals(updateLearningTopic.Name);

            if(hasNameChanged)
            {
                await ValidateDuplicateNameAsync(updateLearningTopic.Name);
            }

            var specialities = await ValidateAndGetSpecialitiesById(updateLearningTopic.SpecialityIds.Distinct());

            existingLearningTopic.Name = updateLearningTopic.Name;
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

            var learningTopicCount = await learningTopicsRepository.GetCountAsync();

            var totalPages = PaginationMethods.CalculateTotalPages(learningTopicCount, filter.PageSize.Value);

            if (filter.PageNum > totalPages)
            {
                learningTopicsServiceLogger.LogErrorAndThrowExceptionPageCount(nameof(LearningTopicsService), 
                    totalPages, filter.PageNum.Value);
            }

            var learningTopics = learningTopicCount > 0 ?
                await learningTopicsRepository.GetAllAsync(filter) :
                new List<LearningTopic>();

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

        private async Task<ICollection<Speciality>> ValidateAndGetSpecialitiesById(IEnumerable<Guid> specialityIds)
        {
            var specialities = await specialitiesRepository.GetByIdsAsync(specialityIds);
            
            if(specialities.Count != specialityIds.Count())
            {
                learningTopicsServiceLogger.LogErrorAndThrowExceptionNotAllFound(nameof(LearningTopicsService), 
                    "specialities", specialityIds);
            }

            return specialities;
        }
    }
}