using System;

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

        /// <summary>
        /// Returns the response or raises an exception if the request was unsuccessful.
        /// (Just using .Response may return simply NULL and cause an Obj Null Ref Ex upstream, which will be unhelpful.)
        /// </summary>
        /// <returns></returns>
        public T GetResponse()
        {
            if (Success && Response != null) return Response;
            else if (Success && Response == null) throw new Exception("The response is empty but the API call was successful.");
            else throw new Exception($"The API was not successful. There were {Errors?.Length} errors");
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
