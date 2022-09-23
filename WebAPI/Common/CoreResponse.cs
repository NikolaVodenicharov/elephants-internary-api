using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WebAPI.Common
{
    public class CoreResponse<T>
    {
        public bool Success { get; private set; }

        public T? Data { get; private set; }

        public Error Error { get; private set; }

        public int StatusCode { get; private set; }

        public CoreResponse(T data, int statusCode = 200)
        {
            Data = data;
            Error = new Error();
            Success = true;
            StatusCode = statusCode;
        }

        public CoreResponse(Error error, int statusCode)
        {
            Data = default;
            Error = error;
            Success = false;
            StatusCode = statusCode;
        }
    }

    public static class CoreResult
    {
        public static IActionResult Success<T>(T data, int statusCode = 200)
        {
            var coreResponse = new CoreResponse<T>(data);

            var jsonResult = new JsonResult(coreResponse)
            {
                StatusCode = coreResponse.StatusCode
            };

            return jsonResult;
        }

        public static string Error(Error error, int statusCode)
        {
            var coreResponse = new CoreResponse<Object>(error, statusCode);

            var json = JsonSerializer.Serialize(coreResponse,
                new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return json;
        }
    }

}
