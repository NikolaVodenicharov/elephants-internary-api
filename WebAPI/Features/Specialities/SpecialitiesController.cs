using Core.Common.Exceptions;
using Core.Common.Pagination;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.RequestModels;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialties.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebAPI.Common;
using WebAPI.Common.Abstractions;

namespace WebAPI.Features.Specialities
{
    [Authorize]
    public class SpecialitiesController : ApiControllerBase
    {
        private readonly ISpecialitiesService specialitiesService;
        private readonly ILogger<SpecialitiesController> specialitiesControllerLogger;
        private readonly IValidator<CreateSpecialityRequest> createSpecialityValidator;
        private readonly IValidator<UpdateSpecialityRequest> updateSpecialityValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;

        public SpecialitiesController(
            ISpecialitiesService specialitiesService,
            ILogger<SpecialitiesController> specialitiesControllerLogger,
            IValidator<CreateSpecialityRequest> createSpecialityValidator,
            IValidator<UpdateSpecialityRequest> updateSpecialityValidator,
            IValidator<PaginationRequest> paginationRequestValidator)
        {
            this.specialitiesService = specialitiesService;
            this.specialitiesControllerLogger = specialitiesControllerLogger;
            this.createSpecialityValidator = createSpecialityValidator;
            this.updateSpecialityValidator = updateSpecialityValidator;
            this.paginationRequestValidator = paginationRequestValidator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CoreResponse<SpecialitySummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAsync(CreateSpecialityRequest createSpecialityRequest)
        {
            LogInformation(nameof(CreateAsync));

            await createSpecialityValidator.ValidateAndThrowAsync(createSpecialityRequest);

            var specialitySummaryResponse = await specialitiesService.CreateAsync(createSpecialityRequest);

            return CoreResult.Success(specialitySummaryResponse);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CoreResponse<SpecialitySummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateSpecialityRequest updateSpecialityRequest)
        {
            LogInformation(nameof(UpdateAsync), id);

            if (id != updateSpecialityRequest.Id)
            {
                specialitiesControllerLogger.LogError($"[{nameof(SpecialitiesController)}] Invalid {nameof(Speciality)} Id ({id}) " +
                    $"in {nameof(UpdateAsync)} method.");

                throw new CoreException($"Invalid {nameof(id)}.", HttpStatusCode.BadRequest);
            }

            await updateSpecialityValidator.ValidateAndThrowAsync(updateSpecialityRequest);

            var specialitySummaryResponse = await specialitiesService.UpdateAsync(updateSpecialityRequest);

            return CoreResult.Success(specialitySummaryResponse);
        }

        [HttpGet]
        [ProducesResponseType(typeof(CoreResponse<IEnumerable<SpecialitySummaryResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<PaginationResponse<SpecialitySummaryResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllAsync(int? pageNum = null, int? pageSize = null)
        {
            if (pageNum == null && pageSize == null)
            {
                LogInformation(nameof(GetAllAsync));

                var specialitySummaries = await specialitiesService.GetAllAsync();

                return CoreResult.Success(specialitySummaries);
            }

            var filter = new PaginationRequest(pageNum, pageSize);

            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            specialitiesControllerLogger.LogInformation($"[{nameof(SpecialitiesController)}] Get {pageSize} " +
                $"specialties from page {pageNum}");

            var paginationResponse  = await specialitiesService.GetPaginationAsync(filter);

            return CoreResult.Success(paginationResponse);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CoreResponse<SpecialitySummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            LogInformation(nameof(GetByIdAsync), id);

            var specialitySummaryResponse = await specialitiesService.GetByIdAsync(id);

            return CoreResult.Success(specialitySummaryResponse);
        }

        private void LogInformation(string methodName)
        {
            specialitiesControllerLogger.LogInformation($"[{nameof(SpecialitiesController)}] {methodName} executing.");
        }

        private void LogInformation(string methodName, Guid id)
        {
            specialitiesControllerLogger.LogInformation($"[{nameof(SpecialitiesController)}] {methodName} executing, entity id: {id}.");
        }
    }
}
