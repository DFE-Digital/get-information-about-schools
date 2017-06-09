using Edubase.Services.Domain;

namespace Edubase.Services
{
    using Common.IO;
    using Exceptions;
    using Newtonsoft.Json;
    using StackExchange.Profiling;
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Texuna;
    using Texuna.Core;
    using Texuna.Serialization;

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

        public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string uri, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: GET {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Get, uri, principal);
                var result = await SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync<TResponse>(result);
            }
        }

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

        public async Task<ApiResponse<T>> PutAsync<T>(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: PUT {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Put, uri, principal, data);
                var result = await SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync<T>(result);
            }
        }

        public async Task PutAsync(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: PUT {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Put, uri, principal, data);
                var result = await SendAsync(requestMessage);
                await ParseHttpResponseMessageAsync(result);
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

                var jsonPayload = new ObjectContent<object>(data, _formatter);
                jsonPayload.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("form-data") {
                        Name = "payload" // shouldn't be necessary, but it is.
                    };
                
                jsonPayload.Headers.ContentType.MediaType = "text/plain"; // should be application/json, but for some reason we have to use text/plain
                content.Add(jsonPayload);

                requestMessage.Content = content;

                var result = await _httpClient.SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync<T>(result);
            }
        }

        private HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string uri, IPrincipal principal, object requestBodyData = null)
        {
            var requestMessage = new HttpRequestMessage(method, uri);
            requestMessage.Headers.Add(HEADER_SA_USER_ID, principal.GetUserId() ?? string.Empty);
            if (requestBodyData != null) requestMessage.Content = new ObjectContent<object>(requestBodyData, _formatter);
            return requestMessage;
        }

        private async Task<ApiResponse<T>> ParseHttpResponseMessageAsync<T>(HttpResponseMessage message)
        {
            var response = new ApiResponse<T>(message.IsSuccessStatusCode);
            if (message.IsSuccessStatusCode) return await DeserializeResponseAsync(message, response);
            else return await ProcessApiErrorAsync(message, response);
        }

        private async Task<ApiResponse> ParseHttpResponseMessageAsync(HttpResponseMessage message)
        {
            var response = new ApiResponse(message.IsSuccessStatusCode && !(message?.Content?.Headers?.ContentType?.MediaType?.Equals("text/html")).GetValueOrDefault());
            if (message.IsSuccessStatusCode) return response;
            else return await ProcessApiErrorAsync(message, response);
        }

        private static async Task<T> ProcessApiErrorAsync<T>(HttpResponseMessage message, T response) where T : ApiResponse
        {
            if (message.StatusCode == System.Net.HttpStatusCode.Forbidden)
                throw new EduSecurityException("The current principal does not have permission to call this API");

            if (message.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                try
                {
                    try
                    {
                        response.Errors = await message.Content.ReadAsAsync<ApiError[]>();
                    }
                    catch (JsonSerializationException)
                    {
                        response.Errors = new[] { await message.Content.ReadAsAsync<ApiError>() };
                    }
                    return response;
                }
                catch (Exception e)
                {
                    throw new TexunaApiSystemException($"The API returned an error with status code: {message.StatusCode}. (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})", e);
                }
            }
            else if(message.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new TexunaApiSystemException($"The API returned an 'Internal Server Error'. (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
            }
            else
            {
                throw new TexunaApiSystemException($"The API returned an error with status code: {message.StatusCode}. (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
            }
        }


        private async Task<ApiResponse<T>> DeserializeResponseAsync<T>(HttpResponseMessage message, ApiResponse<T> response)
        {
            if (!message.Content.Headers.ContentType.MediaType.Equals("application/json"))
            {
                throw new TexunaApiSystemException(
                    $"The API returned an invalid content type: '{message.Content.Headers.ContentType.MediaType}' (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
            }

            if (typeof(T) == typeof(string))
            {
                response.Response = (T)(object)await message.Content.ReadAsStringAsync();
            }
            else
            {
                response.Response = await message.Content.ReadAsAsync<T>(new[] { _formatter });
            }

            return response;
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
    }
}
