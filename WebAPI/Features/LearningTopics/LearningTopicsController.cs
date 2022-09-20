using Core.Common.Exceptions;
using Core.Features.LearningTopics.Interfaces;
using Core.Features.LearningTopics.RequestModels;
using Core.Features.LearningTopics.ResponseModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using WebAPI.Common.Abstractions;
using WebAPI.Common.ErrorHandling;

namespace WebAPI.Features.LearningTopics
{
    [Authorize]
    public class LearningTopicsController : ApiControllerBase
    {
        private readonly ILearningTopicsService learningTopicsService;
        private readonly IValidator<CreateLearningTopicRequest> createLearningTopicValidator;
        private readonly IValidator<UpdateLearningTopicRequest> updateLearningTopicValidator;
        private readonly ILogger<LearningTopicsController> learningTopicsControllerLogger;
        
        public LearningTopicsController(
            ILearningTopicsService learningTopicsService,
            IValidator<CreateLearningTopicRequest> createLearningTopicValidator,
            IValidator<UpdateLearningTopicRequest> updateLearningTopicValidator,
            ILogger<LearningTopicsController> learningTopicsControllerLogger)
        {
            this.learningTopicsService = learningTopicsService;
            this.createLearningTopicValidator = createLearningTopicValidator;
            this.updateLearningTopicValidator = updateLearningTopicValidator;
            this.learningTopicsControllerLogger = learningTopicsControllerLogger;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LearningTopicSummaryResponse))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateAsync(CreateLearningTopicRequest request)
        {
            LogInformation(nameof(CreateAsync));

            await createLearningTopicValidator.ValidateAndThrowAsync(request);

            var learningTopicResponse = await learningTopicsService.CreateAsync(request);

            return Ok(learningTopicResponse);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LearningTopicSummaryResponse))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateLearningTopicRequest request)
        {
            LogInformation(nameof(UpdateAsync), request.Id);

            if (id != request.Id)
            {
                learningTopicsControllerLogger.LogError($"[{nameof(LearningTopicsController)}] Invalid Learning Topic Id ({id}) in {nameof(UpdateAsync)} method.");

                throw new CoreException($"Invalid Id of Learning topic provided in request data.", HttpStatusCode.BadRequest);
            }

            await updateLearningTopicValidator.ValidateAndThrowAsync(request);

            var learningTopicResult = await learningTopicsService.UpdateAsync(request);

            return Ok(learningTopicResult);
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LearningTopicSummaryResponse))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            LogInformation(nameof(GetByIdAsync), id);

            var learningTopicResult = await learningTopicsService.GetByIdAsync(id);

            return Ok(learningTopicResult);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<LearningTopicSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetAllAsync()
        {
            LogInformation(nameof(GetAllAsync));

            var learningTopicsResult = await learningTopicsService.GetAllAsync();

            return Ok(learningTopicsResult);
        }
        
        private void LogInformation(string methodName, Guid id)
        {
            learningTopicsControllerLogger.LogInformation($"[{nameof(LearningTopicsController)}] {methodName} executing, entity id: {id}.");
        }

        private void LogInformation(string methodName)
        {
            learningTopicsControllerLogger.LogInformation($"[{nameof(LearningTopicsController)}] {methodName} executing.");
        }
    }
}