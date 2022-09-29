using Core.Common.Exceptions;
using Core.Features.LearningTopics.Interfaces;
using Core.Features.LearningTopics.RequestModels;
using Core.Features.LearningTopics.ResponseModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using WebAPI.Common;
using WebAPI.Common.Abstractions;
using Core.Common.Pagination;

namespace WebAPI.Features.LearningTopics
{
    [Authorize]
    public class LearningTopicsController : ApiControllerBase
    {
        private readonly ILearningTopicsService learningTopicsService;
        private readonly IValidator<CreateLearningTopicRequest> createLearningTopicValidator;
        private readonly IValidator<UpdateLearningTopicRequest> updateLearningTopicValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;
        private readonly ILogger<LearningTopicsController> learningTopicsControllerLogger;
        
        public LearningTopicsController(
            ILearningTopicsService learningTopicsService,
            IValidator<CreateLearningTopicRequest> createLearningTopicValidator,
            IValidator<UpdateLearningTopicRequest> updateLearningTopicValidator,
            IValidator<PaginationRequest> paginationRequestValidator,
            ILogger<LearningTopicsController> learningTopicsControllerLogger)
        {
            this.learningTopicsService = learningTopicsService;
            this.createLearningTopicValidator = createLearningTopicValidator;
            this.updateLearningTopicValidator = updateLearningTopicValidator;
            this.paginationRequestValidator = paginationRequestValidator;
            this.learningTopicsControllerLogger = learningTopicsControllerLogger;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<LearningTopicSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> CreateAsync(CreateLearningTopicRequest request)
        {
            LogInformation(nameof(CreateAsync));

            await createLearningTopicValidator.ValidateAndThrowAsync(request);

            var learningTopicResponse = await learningTopicsService.CreateAsync(request);

            return CoreResult.Success(learningTopicResponse);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<LearningTopicSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
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

            return CoreResult.Success(learningTopicResult);
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<LearningTopicSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            LogInformation(nameof(GetByIdAsync), id);

            var learningTopicResult = await learningTopicsService.GetByIdAsync(id);

            return CoreResult.Success(learningTopicResult);
        }

        [HttpGet]
        [ProducesResponseType(typeof(CoreResponse<IEnumerable<LearningTopicSummaryResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<PaginationResponse<LearningTopicSummaryResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllAsync(int? pageNum = null, int? pageSize = null)
        {
            if (pageNum == null && pageSize == null)
            {
                LogInformation(nameof(GetAllAsync));

                var learningTopicSummaries = await learningTopicsService.GetAllAsync();

                return CoreResult.Success(learningTopicSummaries);
            }

            var filter = new PaginationRequest(pageNum, pageSize);

            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            learningTopicsControllerLogger.LogInformation($"[{nameof(LearningTopicsController)}] Get {pageSize} " +
                $"learning topics from page {pageNum}");

            var paginationResponse = await learningTopicsService.GetPaginationAsync(filter);

            return CoreResult.Success(paginationResponse);
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