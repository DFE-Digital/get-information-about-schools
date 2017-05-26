namespace Edubase.Services.Domain
{
    public class ApiResponse<T> : ApiResponse
    {
        public T Response { get; set; }

        public ApiResponse()
        {

        }

        public ApiResponse(bool success) : base(success)
        {
        }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public ApiError[] Errors { get; set; }

        public ApiResponse()
        {

        }

        public ApiResponse(bool success)
        {
            Success = success;
        }
    }
}
