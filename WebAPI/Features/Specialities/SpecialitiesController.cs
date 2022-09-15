using Core.Common.Exceptions;
using Core.Features.Specialities.Interfaces;
using Core.Features.Specialities.RequestModels;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialties.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebAPI.Common.Abstractions;
using WebAPI.Common.ErrorHandling;

namespace WebAPI.Features.Specialities
{
    [Authorize]
    public class SpecialitiesController : ApiControllerBase
    {
        private readonly ISpecialitiesService specialitiesService;
        private readonly ILogger<SpecialitiesController> specialitiesControllerLogger;
        public readonly IValidator<CreateSpecialityRequest> createSpecialityValidator;
        public readonly IValidator<UpdateSpecialityRequest> updateSpecialityValidator;

        public SpecialitiesController(
            ISpecialitiesService specialitiesService,
            ILogger<SpecialitiesController> specialitiesControllerLogger,
            IValidator<CreateSpecialityRequest> createSpecialityValidator,
            IValidator<UpdateSpecialityRequest> updateSpecialityValidator)
        {
            this.specialitiesService = specialitiesService;
            this.specialitiesControllerLogger = specialitiesControllerLogger;
            this.createSpecialityValidator = createSpecialityValidator;
            this.updateSpecialityValidator = updateSpecialityValidator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SpecialitySummaryResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAsync(CreateSpecialityRequest createSpecialityRequest)
        {
            LogInformation(nameof(CreateAsync));

            await createSpecialityValidator.ValidateAndThrowAsync(createSpecialityRequest);

            var specialitySummaryResponse = await specialitiesService.CreateAsync(createSpecialityRequest);

            return Ok(specialitySummaryResponse);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SpecialitySummaryResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateSpecialityRequest updateSpecialityRequest)
        {
            LogInformation(nameof(UpdateAsync), id);

            if (id != updateSpecialityRequest.Id)
            {
                specialitiesControllerLogger.LogError($"[{nameof(SpecialitiesController)}] Invalid {nameof(Speciality)} Id ({id}) in {nameof(UpdateAsync)} method.");

                throw new CoreException($"Invalid {nameof(id)}.", HttpStatusCode.BadRequest);
            }

            await updateSpecialityValidator.ValidateAndThrowAsync(updateSpecialityRequest);

            var specialitySummaryResponse = await specialitiesService.UpdateAsync(updateSpecialityRequest);

            return Ok(specialitySummaryResponse);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SpecialitySummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllAsync()
        {
            LogInformation(nameof(GetAllAsync));

            var specialitySummaries = await specialitiesService.GetAllAsync();

            return Ok(specialitySummaries);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IEnumerable<SpecialitySummaryResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            LogInformation(nameof(GetByIdAsync), id);

            var specialitySummaryResponse = await specialitiesService.GetByIdAsync(id);

            return Ok(specialitySummaryResponse);
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
