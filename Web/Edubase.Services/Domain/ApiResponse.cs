namespace Edubase.Services.Domain
{
    public class ApiResponse<T> : ApiResponse
    {
        public T Response { get; set; }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public ApiError[] Errors { get; set; }
    }
}
