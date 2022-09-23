using Core.Common.Exceptions;
using Core.Features.LearningTopics.Interfaces;
using Core.Features.LearningTopics.RequestModels;
using Core.Features.LearningTopics.ResponseModels;
using Core.Features.LearningTopics.Support;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialties.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Core.Features.LearningTopics
{
    public class LearningTopicsService : ILearningTopicsService
    {
        private readonly ILearningTopicsRepository learningTopicsRepository;
        private readonly ISpecialitiesRepository specialitiesRepository;
        private readonly ILogger<LearningTopicsService> learningTopicsServiceLogger;
        private readonly IValidator<CreateLearningTopicRequest> createLearningTopicValidator;
        private readonly IValidator<UpdateLearningTopicRequest> updateLearningTopicValidator;

        public LearningTopicsService(
            ILearningTopicsRepository learningTopicsRepository,
            ISpecialitiesRepository specialitiesRepository,
            ILogger<LearningTopicsService> learningTopicsServiceLogger,
            IValidator<CreateLearningTopicRequest> createLearningTopicValidator,
            IValidator<UpdateLearningTopicRequest> updateLearningTopicValidator)
        {
            this.learningTopicsRepository = learningTopicsRepository;
            this.specialitiesRepository = specialitiesRepository;
            this.learningTopicsServiceLogger = learningTopicsServiceLogger;
            this.createLearningTopicValidator = createLearningTopicValidator;
            this.updateLearningTopicValidator = updateLearningTopicValidator;
        }

        public async Task<LearningTopicSummaryResponse> CreateAsync(CreateLearningTopicRequest request)
        {
            await createLearningTopicValidator.ValidateAndThrowAsync(request);

            await ValidateDuplicateNameAsync(request.Name);

            var specialities = await ValidateAndGetSpecialitiesById(request.SpecialityIds);
            
            var learningTopic = request.ToLearningTopic();
            learningTopic.Specialities = specialities;

            var learningTopicResponse = await learningTopicsRepository.AddAsync(learningTopic);

            LogInformation(nameof(CreateAsync), learningTopicResponse.Id);

            return learningTopicResponse.ToLearningTopicSummary();
        }

        public async Task<LearningTopicSummaryResponse> UpdateAsync(UpdateLearningTopicRequest request)
        {
            await updateLearningTopicValidator.ValidateAndThrowAsync(request);

            var existingLearningTopic = await learningTopicsRepository.GetByIdAsync(request.Id);

            if(existingLearningTopic is null)
            {
                ThrowExceptionIdNotFound(request.Id);
            }

            var hasNameChanged = !existingLearningTopic.Name.Equals(request.Name);

            if(hasNameChanged)
            {
                await ValidateDuplicateNameAsync(request.Name);
            }

            var specialities = await ValidateAndGetSpecialitiesById(request.SpecialityIds);

            existingLearningTopic.Name = request.Name;
            existingLearningTopic.Specialities = specialities;

            await learningTopicsRepository.SaveTrackingChangesAsync();

            LogInformation(nameof(UpdateAsync), existingLearningTopic.Id);

            return existingLearningTopic.ToLearningTopicSummary();
        }

        public async Task<LearningTopicSummaryResponse> GetByIdAsync(Guid id)
        {
            var learningTopicResponse = await learningTopicsRepository.GetByIdAsync(id);

            if(learningTopicResponse is null)
            {
                ThrowExceptionIdNotFound(id);
            }

            LogInformation(nameof(GetByIdAsync), learningTopicResponse.Id);

            return learningTopicResponse.ToLearningTopicSummary();
        }

        public async Task<IEnumerable<LearningTopicSummaryResponse>> GetAllAsync()
        {
            var learningTopicsResponse = await learningTopicsRepository.GetAllAsync();

            LogInformation(nameof(GetAllAsync));

            return learningTopicsResponse.ToLearningTopicSummaries();
        }

        private async Task ValidateDuplicateNameAsync(string learningTopicName)
        {
            var nameExists = await learningTopicsRepository.ExistsByNameAsync(learningTopicName);

            if (nameExists)
            {
                ThrowExceptionDuplicateName(learningTopicName);
            }
        }

        private async Task<ICollection<Speciality>> ValidateAndGetSpecialitiesById(ICollection<Guid> specialityIds)
        {
            if(specialityIds.Count() != specialityIds.Distinct().Count())
            {
                ThrowExceptionDuplicateSpecialities(specialityIds);
            }

            var specialities = await specialitiesRepository.GetByIdsAsync(specialityIds);
            
            if(specialities.Count() != specialityIds.Count())
            {
                ThrowExceptionSpecialitiesNotExist(specialityIds);
            }

            return specialities;
        }

        private void ThrowExceptionIdNotFound(Guid id)
        {
            var idNotFoundMessage = "Requested learning topic was not found!";

            learningTopicsServiceLogger.LogError($"[{nameof(LearningTopicsService)}] {idNotFoundMessage} [{id}]");

            throw new CoreException(idNotFoundMessage, HttpStatusCode.NotFound);
        }

        private void ThrowExceptionDuplicateSpecialities(ICollection<Guid> specialityIds)
        {
            var duplicateSpecialitiesMessage = "Learning Topic cannot have duplicate specialities!";

            learningTopicsServiceLogger.LogError($"[{nameof(LearningTopicsService)}] {duplicateSpecialitiesMessage}"
                + $" [{String.Join(",", specialityIds)}]");

            throw new CoreException(duplicateSpecialitiesMessage, HttpStatusCode.BadRequest);
        }

        private void ThrowExceptionSpecialitiesNotExist(ICollection<Guid> specialityIds)
        {
            var specialitiesNotFoundMessage = "Not all selected specialities are found.";

            learningTopicsServiceLogger.LogError($"[{nameof(LearningTopicsService)}] {specialitiesNotFoundMessage}"
                + $" [{String.Join(",", specialityIds)}]");

            throw new CoreException(specialitiesNotFoundMessage, HttpStatusCode.BadRequest);
        }

        private void ThrowExceptionDuplicateName(string name)
        {
            var duplicateNameMessage = "A learning topic with that name already exists.";

            learningTopicsServiceLogger.LogError($"[{nameof(LearningTopicsService)}] {duplicateNameMessage} [{name}]");

            throw new CoreException(duplicateNameMessage, HttpStatusCode.BadRequest);
        }

        private void LogInformation(string methodName, Guid id)
        {
            learningTopicsServiceLogger.LogInformation($"[{nameof(LearningTopicsService)}] {methodName} successfully executed, entity id: {id}.");
        }

        private void LogInformation(string methodName)
        {
            learningTopicsServiceLogger.LogInformation($"[{nameof(LearningTopicsService)}] {methodName} successfully executed.");
        }
    }
}