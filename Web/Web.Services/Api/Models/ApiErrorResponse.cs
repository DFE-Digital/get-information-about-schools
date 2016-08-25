using System.Net;

namespace Web.Services.Api.Models
{
    public class ApiErrorResponse
    {
        public string Message { get; }
        public HttpStatusCode StatusCode { get; }

        public ApiErrorResponse(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}