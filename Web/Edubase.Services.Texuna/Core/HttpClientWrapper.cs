
namespace Edubase.Services
{
    using Common.IO;
    using Exceptions;
    using Newtonsoft.Json;
    using StackExchange.Profiling;
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Texuna;
    using Texuna.Core;
    using Texuna.Serialization;
    using Domain;
    using System.Linq;
    using Edubase.Common;

    public class HttpClientWrapper
    {
        private readonly HttpClient _httpClient;
        private readonly JsonMediaTypeFormatter _formatter = new JsonMediaTypeFormatter();
        private const string HEADER_SA_USER_ID = "sa_user_id";

        private string ApiUsername => ConfigurationManager.AppSettings["api:Username"];
        private string ApiPassword => ConfigurationManager.AppSettings["api:Password"];

        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(180); // API is slooooow
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", new BasicAuthCredentials(ApiUsername, ApiPassword).ToString()); 
            _formatter.SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new TexunaCamelCasePropertyNamesContractResolver()
            };
        }

        //TODO: tidy up the "throwOnNotFound" stuff
        public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string uri, IPrincipal principal, bool throwOnNotFound = true)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: GET {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Get, uri, principal);
                var result = await SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync<TResponse>(result, throwOnNotFound);
            }
        }

        public async Task<ApiResponse> PatchAsync(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: PUT {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(new HttpMethod("PATCH"), uri, principal, data);
                var result = await SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync(result);
            }
        }

        public async Task DeleteAsync(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: DELETE {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Delete, uri, principal, data);
                var result = await SendAsync(requestMessage);
                await ParseHttpResponseMessageAsync(result);
            }
        }

        #region PUT methods

        public async Task<ApiResponse<T>> PutAsync<T>(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: PUT {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Put, uri, principal, data);
                var result = await SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync<T>(result);
            }
        }

        public async Task<ApiResponse> PutAsync(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: PUT {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Put, uri, principal, data);
                var result = await SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync(result);
            }
        }

        #endregion

        #region POST methods

        public async Task<ApiResponse<T>> PostAsync<T>(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: POST {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, uri, principal, data);
                var result = await SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync<T>(result);
            }
        }

        public async Task<ApiResponse> PostAsync(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: POST {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, uri, principal, data);
                var result = await SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync(result);
            }
        }

        public async Task<ApiResponse<TSuccess, TValidationEnvelope>> PostAsync<TSuccess, TValidationEnvelope>(string uri, object payload, IPrincipal principal) 
            where TValidationEnvelope : class
        {
            using (MiniProfiler.Current.Step($"TEXAPI: POST {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, uri, principal, payload);
                var result = await SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync<TSuccess, TValidationEnvelope>(result);
            }
        }

        public async Task<ApiResponse<TSuccess, TValidationEnvelope>> PutAsync<TSuccess, TValidationEnvelope>(string uri, object payload, IPrincipal principal)
            where TValidationEnvelope : class
        {
            using (MiniProfiler.Current.Step($"TEXAPI: POST {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Put, uri, principal, payload);
                var result = await SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync<TSuccess, TValidationEnvelope>(result);
            }
        }

        /// <summary>
        /// Posts a multipart request to the API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Note:  This method contains a few hacks as the API isn't great at supporting multipart at the point of writing.
        /// </remarks>
        public async Task<ApiResponse<T>> PostMultipartAsync<T>(string uri, object data, string fileName, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: POST (multipart) {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, uri, principal);

                var content = new MultipartContent("form-data", Guid.NewGuid().ToString());

                var fileContent = new ByteArrayContent(File.ReadAllBytes(fileName));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(new FileHelper().GetMimeType(fileName));
                fileContent.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "bulkfile", // shouldn't be necessary, but it is.
                        FileName = Path.GetFileName(fileName) // shouldn't be necessary
                    };
               
                content.Add(fileContent);

                if (data != null)
                {
                    var jsonPayload = new ObjectContent<object>(data, _formatter);
                    jsonPayload.Headers.ContentDisposition =
                        new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "payload" // shouldn't be necessary, but it is.
                    };

                    jsonPayload.Headers.ContentType.MediaType = "text/plain"; // should be application/json, but for some reason we have to use text/plain
                    content.Add(jsonPayload);
                }

                requestMessage.Content = content;

                var result = await _httpClient.SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync<T>(result);
            }
        }
        
        #endregion

        #region ParseHttpResponseMessageAsync

        private async Task<ApiResponse<T>> ParseHttpResponseMessageAsync<T>(HttpResponseMessage message, bool throwOnNotFound = true)
        {
            AssertJsonContentOrEmpty(message);
            var response = new ApiResponse<T>(message.IsSuccessStatusCode);
            if (message.IsSuccessStatusCode) return response.OK(await DeserializeResponseAsync<T>(message));
            else return await ProcessApiErrorAsync(message, response, throwOnNotFound);
        }

        private async Task<ApiResponse> ParseHttpResponseMessageAsync(HttpResponseMessage message)
        {
            AssertJsonContentOrEmpty(message);
            var response = new ApiResponse(message.IsSuccessStatusCode);
            if (message.IsSuccessStatusCode) return response;
            else return await ProcessApiErrorAsync(message, response);
        }
        
        private async Task<ApiResponse<TSuccess, TValidationEnvelope>> ParseHttpResponseMessageAsync<TSuccess, TValidationEnvelope>(HttpResponseMessage message)
            where TValidationEnvelope : class
        {
            AssertJsonContentOrEmpty(message);
            var response = new ApiResponse<TSuccess, TValidationEnvelope>();
            if (message.IsSuccessStatusCode) return response.Success(await DeserializeResponseAsync<TSuccess>(message));
            else return await ProcessApiErrorAsync(message, response);
        }

        #endregion

        #region Error handling

        private async Task<T> ProcessApiErrorAsync<T>(HttpResponseMessage message, T response, bool throwOnNotFound = true) where T : ApiResponse
        {
            ValidateGenericHttpErrors(message, throwOnNotFound);
            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var json = await message.Content.ReadAsStringAsync();
                var errors = ParseApiErrors(json);
                Guard.IsTrue(errors.Any(), () => new TexunaApiSystemException("The API gave a 400 Bad Request response; returned JSON.  But the JSON returned was unrecognizable. " + json));
                response.Fail(errors);
            }

            if (message.StatusCode == HttpStatusCode.NotFound)
            {
                response.Success = false;
            }

            return response;
        }

        private async Task<ApiResponse<TSuccess, TValidationEnvelope>> ProcessApiErrorAsync<TSuccess, TValidationEnvelope>(HttpResponseMessage message, ApiResponse<TSuccess, TValidationEnvelope> response, bool throwOnNotFound = true) 
            where TValidationEnvelope : class
        {
            ValidateGenericHttpErrors(message);

            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                response.Successful = false;
                var json = await message.Content.ReadAsStringAsync();
                response.ValidationEnvelope = TryDeserializeAsync<TValidationEnvelope>(json);
                if (response.ValidationEnvelope == null)
                {
                    var errors = ParseApiErrors(json);
                    Guard.IsTrue(errors.Any(), () => new TexunaApiSystemException("The API gave a 400 Bad Request response; returned JSON.  But the JSON returned was unrecognizable. " + json));
                    response.Fail(errors);
                }
            }

            if (message.StatusCode == HttpStatusCode.NotFound)
            {
                response.Successful = false;
                response.Response = default(TSuccess);
            }

            return response;
        }

        private ApiError[] ParseApiErrors(string json)
        {
            var error = TryDeserializeAsync<ApiError>(json);
            if (error != null && !error.IsEmpty) return new[] { error };
            
            var errors = TryDeserializeAsync<ApiError[]>(json);
            if (errors != null) return errors.Where(x => !x.IsEmpty).ToArray();

            return new ApiError[0];
        }

        private void ValidateGenericHttpErrors(HttpResponseMessage message, bool throwOnNotFound = true)
        {
            if (!message.IsSuccessStatusCode)
            {
                switch (message.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        AssertJsonContent(message);
                        break;
                    case HttpStatusCode.InternalServerError:
                        throw new TexunaApiSystemException($"The API returned an 'Internal Server Error'. (Request URI: {message?.RequestMessage?.RequestUri?.PathAndQuery})");
                    case HttpStatusCode.NotFound:
                        if (throwOnNotFound)
                            throw new TexunaApiNotFoundException($"The API returned 404 Not Found. (Request URI: {message?.RequestMessage?.RequestUri?.PathAndQuery})");
                        break;
                    case HttpStatusCode.Forbidden:
                        throw new EduSecurityException("The current principal does not have permission to call this API");
                    default:
                        throw new TexunaApiSystemException($"The API returned an error with status code: {message.StatusCode}. (Request URI: {message?.RequestMessage?.RequestUri?.PathAndQuery})");
                }
            }
        }
        
        private void AssertJsonContent(HttpResponseMessage message)
        {
            if (message?.Content?.Headers?.ContentType?.MediaType != "application/json")
            {
                throw new TexunaApiSystemException(
                    $"The API returned an invalid content type: '{message?.Content?.Headers?.ContentType?.MediaType}' (Request URI: {message?.RequestMessage?.RequestUri?.PathAndQuery})");
            }
        }

        /// <summary>
        /// Validates that the response message is empty or contains JSON
        /// </summary>
        /// <param name="message"></param>
        private void AssertJsonContentOrEmpty(HttpResponseMessage message)
        {
            var type = message?.Content?.Headers?.ContentType?.MediaType;
            if (type != null) AssertJsonContent(message);
        }

        #endregion

        #region Helper methods

        private async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage message)
        {
            AssertJsonContent(message);
            if (typeof(T) == typeof(string)) return (T)(object)await message.Content.ReadAsStringAsync();
            else return await message.Content.ReadAsAsync<T>(new[] { _formatter });
        }
        
        private T TryDeserializeAsync<T>(string json) where T : class
        {
            try { return JsonConvert.DeserializeObject<T>(json); }
            catch { return null; }
        }

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            try
            {
                return await _httpClient.SendAsync(requestMessage);
            }
            catch (TaskCanceledException ex) when (!ex.CancellationToken.IsCancellationRequested) // timeout, apparently: ref; https://stackoverflow.com/questions/29179848/httpclient-a-task-was-cancelled
            {
                throw new TexunaApiSystemException(
                    $"The API did not respond in a timely manner (Request URI: {requestMessage.RequestUri.PathAndQuery})");
            }
        }

        private HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string uri, IPrincipal principal, object requestBodyData = null)
        {
            var requestMessage = new HttpRequestMessage(method, uri);
            requestMessage.Headers.Add(HEADER_SA_USER_ID, principal.GetUserId() ?? string.Empty);
            if (requestBodyData != null) requestMessage.Content = new ObjectContent<object>(requestBodyData, _formatter);
            return requestMessage;
        }

        #endregion
    }
}
