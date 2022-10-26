using Core.Common;
using Core.Common.Pagination;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.RequestModels;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialties.Entities;
using Core.Features.Persons.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebAPI.Common;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Authorization;


namespace WebAPI.Features.Specialities
{
    [CustomAuthorize(RoleId.Administrator)]
    public class SpecialitiesController : ApiControllerBase
    {
        private readonly ISpecialitiesService specialitiesService;
        private readonly ILogger<SpecialitiesController> specialitiesControllerLogger;
        private readonly ISpecialityValidator specialityValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;

        public SpecialitiesController(
            ISpecialitiesService specialitiesService,
            ILogger<SpecialitiesController> specialitiesControllerLogger,
            ISpecialityValidator specialityValidator,
            IValidator<PaginationRequest> paginationRequestValidator)
        {
            this.specialitiesService = specialitiesService;
            this.specialitiesControllerLogger = specialitiesControllerLogger;
            this.specialityValidator = specialityValidator;
            this.paginationRequestValidator = paginationRequestValidator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CoreResponse<SpecialitySummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAsync(CreateSpecialityRequest createSpecialityRequest)
        {
            specialitiesControllerLogger.LogInformationMethod(nameof(SpecialitiesController), nameof(CreateAsync));

            await specialityValidator.ValidateAndThrowAsync(createSpecialityRequest);

            var specialitySummaryResponse = await specialitiesService.CreateAsync(createSpecialityRequest);

            return CoreResult.Success(specialitySummaryResponse);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CoreResponse<SpecialitySummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateSpecialityRequest updateSpecialityRequest)
        {
            specialitiesControllerLogger.LogInformationMethod(nameof(SpecialitiesController), nameof(UpdateAsync), nameof(Speciality), id);

            if (id != updateSpecialityRequest.Id)
            {
                specialitiesControllerLogger.LogErrorAndThrowExceptionIdMismatch(nameof(SpecialitiesController), 
                    updateSpecialityRequest.Id, id);
            }

            await specialityValidator.ValidateAndThrowAsync(updateSpecialityRequest);

            var specialitySummaryResponse = await specialitiesService.UpdateAsync(updateSpecialityRequest);

            return CoreResult.Success(specialitySummaryResponse);
        }

        [HttpGet]
        [ProducesResponseType(typeof(CoreResponse<IEnumerable<SpecialitySummaryResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<PaginationResponse<SpecialitySummaryResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest filter)
        {
            specialitiesControllerLogger.LogInformationMethod(nameof(SpecialitiesController), nameof(GetAllAsync));

            if (filter.PageNum == null && filter.PageSize == null)
            {
                var specialitySummaries = await specialitiesService.GetAllAsync();

                return CoreResult.Success(specialitySummaries);
            }

            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            var paginationResponse  = await specialitiesService.GetPaginationAsync(filter);

            return CoreResult.Success(paginationResponse);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CoreResponse<SpecialitySummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            specialitiesControllerLogger.LogInformationMethod(nameof(SpecialitiesController), nameof(GetByIdAsync), nameof(Speciality), id);

            var specialitySummaryResponse = await specialitiesService.GetByIdAsync(id);

            return CoreResult.Success(specialitySummaryResponse);
        }
    }
}
