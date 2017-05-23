namespace Edubase.Services.Domain
{
    public class ApiResponse<T>
    {
        public T Response { get; set; }
        public bool Success { get; set; }
        public ApiError[] Errors { get; set; }
    }
}
