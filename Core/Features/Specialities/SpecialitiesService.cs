using Core.Common;
using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.RequestModels;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialities.Support;
using Core.Features.Specialties.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Core.Features.Specialities
{
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

            specialitiesServiceLogger.LogInformationMethod(nameof(SpecialitiesService), nameof(CreateAsync), true);

            return specialitySummaryResponse;
        }

        public async Task<SpecialitySummaryResponse> UpdateAsync(UpdateSpecialityRequest updateSpecialityRequest)
        {
            await ValidateUpdateSpecialityAsync(updateSpecialityRequest);

            var speciality = await specialitiesRepository.GetByIdAsync(updateSpecialityRequest.Id);

            Guard.EnsureNotNull(speciality, specialitiesServiceLogger, nameof(SpecialitiesService),
                nameof(Speciality), updateSpecialityRequest.Id);

            speciality.Name = updateSpecialityRequest.Name;

            await specialitiesRepository.SaveTrackingChangesAsync();

            specialitiesServiceLogger.LogInformationMethod(nameof(SpecialitiesService), nameof(UpdateAsync),
                nameof(Speciality), speciality.Id, true);

            return speciality.ToSpecialitySummaryResponse();
        }

        public async Task<SpecialitySummaryResponse> GetByIdAsync(Guid id)
        {
            var speciality = await specialitiesRepository.GetByIdAsync(id);

            Guard.EnsureNotNull(speciality, specialitiesServiceLogger, nameof(SpecialitiesService),
                nameof(Speciality), id);

            specialitiesServiceLogger.LogInformationMethod(nameof(SpecialitiesService), nameof(GetByIdAsync),
                nameof(Speciality), id, true);

            return speciality.ToSpecialitySummaryResponse();
        }

        public async Task<IEnumerable<SpecialitySummaryResponse>> GetAllAsync()
        {
            var specialities = await specialitiesRepository.GetAllAsync();

            specialitiesServiceLogger.LogInformationMethod(nameof(SpecialitiesService), nameof(GetAllAsync), true);

            return specialities;
        }

        public async Task<PaginationResponse<SpecialitySummaryResponse>> GetPaginationAsync(PaginationRequest filter)
        {
            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            Guard.EnsureNotNullPagination(filter.PageNum, filter.PageSize,
                specialitiesServiceLogger, nameof(SpecialitiesService));

            var specialitiesCount = await specialitiesRepository.GetCountAsync();

            var totalPages = PaginationMethods.CalculateTotalPages(specialitiesCount, filter.PageSize.Value);

            if (filter.PageNum > totalPages)
            {
                specialitiesServiceLogger.LogErrorAndThrowExceptionPageCount(nameof(SpecialitiesService), 
                    totalPages, filter.PageNum.Value);
            }

            var specialities = specialitiesCount > 0 ?
                await specialitiesRepository.GetAllAsync(filter) :
                new List<SpecialitySummaryResponse>();

            var paginationResponse = new PaginationResponse<SpecialitySummaryResponse>(
                specialities, filter.PageNum.Value, totalPages);

            specialitiesServiceLogger.LogInformationMethod(nameof(SpecialitiesService), nameof(GetAllAsync), true);

            return paginationResponse;
        }

        private async Task ValidateCreateSpecialityAsync(CreateSpecialityRequest createSpecialityRequest)
        {
            await createSpecialityValidator.ValidateAndThrowAsync(createSpecialityRequest);

            var isNameExist = await specialitiesRepository.ExistsByNameAsync(createSpecialityRequest.Name);

            if (isNameExist)
            {
                specialitiesServiceLogger.LogErrorAndThrowExceptionValueTaken(nameof(SpecialitiesService), 
                    nameof(Speciality), nameof(Speciality.Name), createSpecialityRequest.Name);
            }
        }

        private async Task ValidateUpdateSpecialityAsync(UpdateSpecialityRequest updateSpecialityRequest)
        {
            await updateSpecialityValidator.ValidateAndThrowAsync(updateSpecialityRequest);

            var isNameTaken = await specialitiesRepository.IsNameTakenByOtherAsync(updateSpecialityRequest.Name, 
                updateSpecialityRequest.Id);

            if (isNameTaken)
            {
                specialitiesServiceLogger.LogErrorAndThrowExceptionValueTaken(nameof(SpecialitiesService), 
                    nameof(Speciality), nameof(Speciality.Name), updateSpecialityRequest.Name);
            }
        }
    }
}
