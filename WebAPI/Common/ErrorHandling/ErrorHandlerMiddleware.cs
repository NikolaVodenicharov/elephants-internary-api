using Core.Common.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

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
                switch (ex)
                {
                    case CoreException e:
                        context.Response.StatusCode = (int)e.StatusCode;
                        break;

                    case ValidationException e:
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                context.Response.Headers.Add("content-type", "application/json");

                var errorResponse = CreateErrorResponse(ex);
                var json = JsonSerializer.Serialize(errorResponse);

                logger.LogError(json);

                await context.Response.WriteAsync(json);
            }
        }

        private ErrorResponse CreateErrorResponse(Exception ex)
        {
            var error = GetSnakeCaseErrorName(ex);
            var message = ex.Message ?? string.Empty;

            var errorResponse = new ErrorResponse(error, message);
            return errorResponse;
        }

        private string GetSnakeCaseErrorName(Exception ex)
        {
            var errorWithoutExceptionSuffix = ex
                .GetType()
                .Name
                .Replace(nameof(Exception), string.Empty);

            var snakeCaseError = ToUnderscoreCase(errorWithoutExceptionSuffix);

            return snakeCaseError;
        }

        private string ToUnderscoreCase(string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }
    }
}
