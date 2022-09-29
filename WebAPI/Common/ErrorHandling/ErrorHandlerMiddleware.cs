using Core.Common;
using Core.Common.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace WebAPI.Common.ErrorHandling
{
    internal sealed class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ErrorHandlerMiddleware> logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var isValidationError = false;

                switch (ex)
                {
                    case CoreException e:
                        context.Response.StatusCode = (int)e.StatusCode;
                        break;

                    case ValidationException e:
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        isValidationError = true;
                        break;

                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                context.Response.Headers.Add("content-type", "application/json");

                var errorMessage = ex.Message ?? string.Empty;

                if (isValidationError)
                {
                    errorMessage = String.Join("",
                        new Regex(RegularExpressionPatterns.ValidationErrorMessageSplitPattern)
                        .Split(errorMessage)
                        .Where(p => p.Trim().Length > 0)
                        ).Trim();
                }

                var error = new Error(errorMessage);

                var coreResponse = CoreResult.Error(error, context.Response.StatusCode);

                logger.LogError(coreResponse);

                await context.Response.WriteAsync(coreResponse);
            }
        }
    }
}
