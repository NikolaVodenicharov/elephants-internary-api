using Core.Common.Exceptions;
using Core.Common;
using Core.Common.Pagination;
using Core.Features.LearningTopics.Entities;
using Core.Features.LearningTopics.Interfaces;
using Core.Features.LearningTopics.RequestModels;
using Core.Features.LearningTopics.ResponseModels;
using Core.Features.Persons.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebAPI.Common;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Authorization;

namespace WebAPI.Features.LearningTopics
{
    [CustomAuthorize(RoleId.Administrator)]
    public class LearningTopicsController : ApiControllerBase
    {
        private readonly ILearningTopicsService learningTopicsService;
        private readonly ILearningTopicValidator learningTopicValidator;
        private readonly IValidator<PaginationRequest> paginationRequestValidator;
        private readonly ILogger<LearningTopicsController> learningTopicsControllerLogger;
        
        public LearningTopicsController(
            ILearningTopicsService learningTopicsService,
            ILearningTopicValidator learningTopicValidator,
            IValidator<PaginationRequest> paginationRequestValidator,
            ILogger<LearningTopicsController> learningTopicsControllerLogger)
        {
            this.learningTopicsService = learningTopicsService;
            this.learningTopicValidator = learningTopicValidator;
            this.paginationRequestValidator = paginationRequestValidator;
            this.learningTopicsControllerLogger = learningTopicsControllerLogger;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<LearningTopicSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> CreateAsync(CreateLearningTopicRequest request)
        {
            learningTopicsControllerLogger.LogInformationMethod(nameof(LearningTopicsController), nameof(CreateAsync));

            await learningTopicValidator.ValidateAndThrowAsync(request);

            var learningTopicResponse = await learningTopicsService.CreateAsync(request);

            return CoreResult.Success(learningTopicResponse);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<LearningTopicSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(CoreResponse<Object>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateLearningTopicRequest request)
        {
            learningTopicsControllerLogger.LogInformationMethod(nameof(LearningTopicsController), nameof(UpdateAsync), nameof(LearningTopic), request.Id);

            if (id != request.Id)
            {
                learningTopicsControllerLogger.LogErrorAndThrowExceptionIdMismatch(nameof(LearningTopicsController), 
                    request.Id, id);
            }

            await learningTopicValidator.ValidateAndThrowAsync(request);

            var learningTopicResult = await learningTopicsService.UpdateAsync(request);

            return CoreResult.Success(learningTopicResult);
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CoreResponse<LearningTopicSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(CoreResponse<Object>))]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            learningTopicsControllerLogger.LogInformationMethod(nameof(LearningTopicsController), nameof(GetByIdAsync), nameof(LearningTopic), id);

            var learningTopicResult = await learningTopicsService.GetByIdAsync(id);

            return CoreResult.Success(learningTopicResult);
        }

        [HttpGet]
        [ProducesResponseType(typeof(CoreResponse<IEnumerable<LearningTopicSummaryResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<PaginationResponse<LearningTopicSummaryResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CoreResponse<Object>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest filter)
        {
            learningTopicsControllerLogger.LogInformationMethod(nameof(LearningTopicsController), nameof(GetAllAsync));

            if (filter.PageNum == null && filter.PageSize == null)
            {     
                var learningTopicSummaries = await learningTopicsService.GetAllAsync();

                return CoreResult.Success(learningTopicSummaries);
            }
            
            await paginationRequestValidator.ValidateAndThrowAsync(filter);

            var paginationResponse = await learningTopicsService.GetPaginationAsync(filter);

            return CoreResult.Success(paginationResponse);
        }
    }
}