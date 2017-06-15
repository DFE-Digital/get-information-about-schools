using Edubase.Common;
using System;
using System.Linq;

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

        public ApiResponse<T> OK(T response)
        {
            Success = true;
            Response = response;
            return this;
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

        public ApiResponse Fail(ApiError[] errors)
        {
            Guard.IsNotNull(errors, () => new Exception("The supplied API errors array was null"));
            Success = false;
            Errors = errors;
            return this;
        }

        public ApiResponse Fail(ApiError error)
        {
            Guard.IsNotNull(error, () => new Exception("The supplied API error was null"));
            Success = false;
            Errors = new[] { error };
            return this;
        }

        public ApiResponse Fail(ApiError error, ApiError[] errors)
        {
            Guard.IsTrue(error != null || (errors != null && errors.Any()), () => new Exception("The supplied API error(s) parameter was null"));
            if (error != null) Fail(error);
            else if (errors != null && errors.Any()) Fail(errors);
            return this;
        }

        public bool HasErrors => (Errors != null && Errors.Any());
    }


    public class ApiResponse<TSuccess, TValidationEnvelope> where TValidationEnvelope : class
    {
        public ApiError[] Errors { get; set; }

        public TValidationEnvelope ValidationEnvelope { get; set; }

        public TSuccess Response { get; set; }

        public bool Successful { get; set; }

        public bool HasErrors => (Errors != null && Errors.Any()) || ValidationEnvelope != null;

        public ApiResponse()
        {

        }

        public ApiResponse(bool successful)
        {
            Successful = successful;
        }

        public ApiResponse<TSuccess, TValidationEnvelope> Success(TSuccess response)
        {
            Successful = true;
            Response = response;
            return this;
        }

        public ApiResponse<TSuccess, TValidationEnvelope> Fail(ApiError[] errors)
        {
            Guard.IsNotNull(errors, () => new Exception("The supplied API errors array was null"));
            Successful = false;
            Errors = errors;
            return this;
        }

        public ApiResponse<TSuccess, TValidationEnvelope> Fail(ApiError error)
        {
            Guard.IsNotNull(error, () => new Exception("The supplied API error was null"));
            Successful = false;
            Errors = new[] { error };
            return this;
        }

        public ApiResponse<TSuccess, TValidationEnvelope> Fail(ApiError error, ApiError[] errors)
        {
            Guard.IsTrue(error != null || (errors != null && errors.Any()), () => new Exception("The supplied API error(s) parameter was null"));
            if (error != null) Fail(error);
            else if (errors!=null && errors.Any()) Fail(errors);
            return this;
        }



        /// <summary>
        /// Returns the response or raises an exception if the request was unsuccessful.
        /// (Just using .Response may return simply NULL and cause an Obj Null Ref Ex upstream, which will be unhelpful.)
        /// </summary>
        /// <returns></returns>
        public TSuccess GetResponse()
        {
            if (Successful && Response != null) return Response;
            else if (Successful && Response == null) throw new Exception("The response is empty but the API call was successful.");
            else throw new Exception($"The API was not successful. There were {Errors?.Length} errors");
        }
    }
}
