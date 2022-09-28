using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.RequestModels;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialities.Support;
using Core.Features.Specialties.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Core.Features.Specialities
{
    internal static class Counter
    {
        public static int specialitiesCount = -1;
    }

    public class SpecialitiesService : ISpecialitiesService
    {
        private readonly ISpecialitiesRepository specialitiesRepository;
        private readonly ILogger<SpecialitiesService> specialitiesServiceLogger;
        private readonly IValidator<CreateSpecialityRequest> createSpecialityValidator;
        private readonly IValidator<UpdateSpecialityRequest> updateSpecialityValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;

        public SpecialitiesService(
            ISpecialitiesRepository specialitiesRepository,
            ILogger<SpecialitiesService> specialitiesServiceLogger,
            IValidator<CreateSpecialityRequest> createSpecialityValidator,
            IValidator<UpdateSpecialityRequest> updateSpecialityValidator,
            IValidator<PaginationRequest> paginationRequestValidator)
        {
            this.specialitiesRepository = specialitiesRepository;
            this.specialitiesServiceLogger = specialitiesServiceLogger;
            this.createSpecialityValidator = createSpecialityValidator;
            this.updateSpecialityValidator = updateSpecialityValidator;
            this.paginationRequestValidator = paginationRequestValidator;
        }

        public async Task<SpecialitySummaryResponse> CreateAsync(CreateSpecialityRequest createSpecialityRequest)
        {
            await ValidateCreateSpecialityAsync(createSpecialityRequest);

            var speciality = createSpecialityRequest.ToSpeciality();

            var specialitySummaryResponse = await specialitiesRepository.AddAsync(speciality);

            LogInformation(nameof(CreateAsync), specialitySummaryResponse.Id);

            return specialitySummaryResponse;
        }

        public async Task<SpecialitySummaryResponse> UpdateAsync(UpdateSpecialityRequest updateSpecialityRequest)
        {
            await ValidateUpdateSpecialityAsync(updateSpecialityRequest);

            var speciality = await specialitiesRepository.GetByIdAsync(updateSpecialityRequest.Id);

            if (speciality == null)
            {
                ThrowExceptionIdNotFound(updateSpecialityRequest.Id);
            }

            speciality!.Name = updateSpecialityRequest.Name;

            await specialitiesRepository.SaveTrackingChangesAsync();

            LogInformation(nameof(UpdateAsync), speciality.Id);

            return speciality.ToSpecialitySummaryResponse();
        }

        public async Task<SpecialitySummaryResponse> GetByIdAsync(Guid id)
        {
            var speciality = await specialitiesRepository.GetByIdAsync(id);

            if (speciality == null)
            {
                ThrowExceptionIdNotFound(id);
            }

            LogInformation(nameof(GetByIdAsync), speciality!.Id);

            return speciality.ToSpecialitySummaryResponse();
        }

        public async Task<IEnumerable<SpecialitySummaryResponse>> GetAllAsync()
        {
            var specialities = await specialitiesRepository.GetAllAsync();

            specialitiesServiceLogger.LogInformation($"[{nameof(SpecialitiesService)}] {nameof(GetAllAsync)} successfully executed.");

            return specialities;
        }

        public async Task<PaginationResponse<SpecialitySummaryResponse>> GetPaginationAsync(PaginationRequest filter)
        {
            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            if (Counter.specialitiesCount == -1 || filter.PageNum == 1)
            {
                Counter.specialitiesCount = await specialitiesRepository.GetCountAsync();
            }

            if (Counter.specialitiesCount == 0)
            {
                if (filter.PageNum > PaginationConstants.DefaultPageCount)
                {
                    LogErrorAndThrowExceptionPageCount(PaginationConstants.DefaultPageCount, filter.PageNum.Value);
                }

                var emptyPaginationResponse = new PaginationResponse<SpecialitySummaryResponse>(
                    new List<SpecialitySummaryResponse>(), filter.PageNum.Value, PaginationConstants.DefaultPageCount);

                return emptyPaginationResponse;
            }

            var totalPages = (Counter.specialitiesCount + filter.PageSize.Value - 1) / filter.PageSize.Value;

            if (filter.PageNum > totalPages)
            {
                LogErrorAndThrowExceptionPageCount(totalPages, filter.PageNum.Value);
            }

            var specialities = await specialitiesRepository.GetAllAsync(filter);

            var paginationResponse = new PaginationResponse<SpecialitySummaryResponse>(
                specialities, filter.PageNum.Value, totalPages);

            specialitiesServiceLogger.LogInformation($"[{nameof(SpecialitiesService)}] {nameof(GetAllAsync)} successfully executed.");

            return paginationResponse;
        }

        private async Task ValidateCreateSpecialityAsync(CreateSpecialityRequest createSpecialityRequest)
        {
            await createSpecialityValidator.ValidateAndThrowAsync(createSpecialityRequest);

            var isNameExist = await specialitiesRepository.ExistsByNameAsync(createSpecialityRequest.Name);

            if (isNameExist)
            {
                ThrowExceptionNameExist(createSpecialityRequest.Name);
            }
        }

        private async Task ValidateUpdateSpecialityAsync(UpdateSpecialityRequest updateSpecialityRequest)
        {
            await updateSpecialityValidator.ValidateAndThrowAsync(updateSpecialityRequest);

            var isNameTaken = await specialitiesRepository.IsNameTakenByOtherAsync(updateSpecialityRequest.Name, updateSpecialityRequest.Id);

            if (isNameTaken)
            {
                ThrowExceptionNameExist(updateSpecialityRequest.Name);
            }
        }

        private void ThrowExceptionIdNotFound(Guid id)
        {
            var idNotFoundMessage = $"{nameof(Speciality)} with Id {id} was not found.";

            specialitiesServiceLogger.LogError($"[{nameof(SpecialitiesService)}] {idNotFoundMessage}");

            throw new CoreException(idNotFoundMessage, HttpStatusCode.NotFound);
        }

        private void ThrowExceptionNameExist(string name)
        {
            var message = $"{nameof(Speciality)} with name {name} already exist.";

            specialitiesServiceLogger.LogError(message);

            throw new CoreException(message, HttpStatusCode.BadRequest);
        }

        private void LogInformation(string methodName, Guid id)
        {
            specialitiesServiceLogger.LogInformation($"[{nameof(SpecialitiesService)}] {methodName} successfully executed, entity id: {id}.");
        }

        private void LogErrorAndThrowExceptionPageCount(int totalPages, int pageNum)
        {
            var message = $"Total number of pages is {totalPages} and requested page number is {pageNum}";

            specialitiesServiceLogger.LogError($"[{nameof(SpecialitiesService)}] {message}");

            throw new CoreException(message, HttpStatusCode.BadRequest);
        }
    }
}
