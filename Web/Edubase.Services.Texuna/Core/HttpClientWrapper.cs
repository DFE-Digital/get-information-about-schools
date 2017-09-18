﻿namespace Edubase.Services
{
    using Common;
    using Common.IO;
    using Core;
    using Data.Repositories;
    using Domain;
    using Exceptions;
    using Microsoft.AspNet.Identity;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web;
    using Texuna;
    using Texuna.Core;
    using Texuna.Glimpse;

    public class HttpClientWrapper
    {
        private readonly HttpClient _httpClient;
        private readonly JsonMediaTypeFormatter _formatter;
        private readonly ApiRecorderSessionItemRepository _apiRecorderSessionItemRepository;
        private const string HEADER_SA_USER_ID = "sa_user_id";
        private const string REQ_BODY_JSON_PAYLOAD = "EdubaseRequestBodyJsonPayload";
        private readonly IClientStorage _clientStorage;

        public HttpClientWrapper(HttpClient httpClient, JsonMediaTypeFormatter formatter, IClientStorage clientStorage, ApiRecorderSessionItemRepository apiRecorderSessionItemRepository)
        {
            _httpClient = httpClient;
            _clientStorage = clientStorage;
            _formatter = formatter;
            _apiRecorderSessionItemRepository = apiRecorderSessionItemRepository;
        }
        
        public HttpClientWrapper(HttpClient httpClient) : this(httpClient, null, null, null)
        {

        }

        public HttpClientWrapper(HttpClient httpClient, JsonMediaTypeFormatter formatter) : this(httpClient, formatter, null, null)
        {

        }

        public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string uri, IPrincipal principal) => await GetAsync<TResponse>(uri, principal, true);

        public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string uri, IPrincipal principal, bool throwOnNotFound)
        {
            var requestMessage = await CreateHttpRequestMessage(HttpMethod.Get, uri, principal);
            var result = await SendAsync(requestMessage);
            return await ParseHttpResponseMessageAsync<TResponse>(uri, result, throwOnNotFound);

        }

        public async Task<ApiResponse> PatchAsync(string uri, object data, IPrincipal principal)
        {
            var requestMessage = await CreateHttpRequestMessage(new HttpMethod("PATCH"), uri, principal, data);
            var result = await SendAsync(requestMessage);
            return await ParseHttpResponseMessageAsync(uri, result);
        }

        public async Task DeleteAsync(string uri, object data, IPrincipal principal)
        {
            var requestMessage = await CreateHttpRequestMessage(HttpMethod.Delete, uri, principal, data);
            var result = await SendAsync(requestMessage);
            await ParseHttpResponseMessageAsync(uri, result);

        }

        #region PUT methods

        public async Task<ApiResponse<T>> PutAsync<T>(string uri, object data, IPrincipal principal)
        {
            var requestMessage = await CreateHttpRequestMessage(HttpMethod.Put, uri, principal, data);
            var result = await SendAsync(requestMessage);
            return await ParseHttpResponseMessageAsync<T>(uri, result);
        }

        public async Task<ApiResponse> PutAsync(string uri, object data, IPrincipal principal)
        {
            var requestMessage = await CreateHttpRequestMessage(HttpMethod.Put, uri, principal, data);
            var result = await SendAsync(requestMessage);
            return await ParseHttpResponseMessageAsync(uri, result);
        }

        #endregion

        #region POST methods

        public async Task<ApiResponse<T>> PostAsync<T>(string uri, object data, IPrincipal principal)
        {
            var requestMessage = await CreateHttpRequestMessage(HttpMethod.Post, uri, principal, data);
            var result = await SendAsync(requestMessage);
            return await ParseHttpResponseMessageAsync<T>(uri, result);

        }

        public async Task<ApiResponse> PostAsync(string uri, object data, IPrincipal principal)
        {

            var requestMessage = await CreateHttpRequestMessage(HttpMethod.Post, uri, principal, data);
            var result = await SendAsync(requestMessage);
            return await ParseHttpResponseMessageAsync(uri, result);

        }

        public async Task<ApiResponse<TSuccess, TValidationEnvelope>> PostAsync<TSuccess, TValidationEnvelope>(string uri, object payload, IPrincipal principal)
            where TValidationEnvelope : class
        {

            var requestMessage = await CreateHttpRequestMessage(HttpMethod.Post, uri, principal, payload);
            var result = await SendAsync(requestMessage);
            return await ParseHttpResponseMessageAsync<TSuccess, TValidationEnvelope>(uri, result);

        }

        public async Task<ApiResponse<TSuccess, TValidationEnvelope>> PutAsync<TSuccess, TValidationEnvelope>(string uri, object payload, IPrincipal principal)
            where TValidationEnvelope : class
        {
            var requestMessage = await CreateHttpRequestMessage(HttpMethod.Put, uri, principal, payload);
            var result = await SendAsync(requestMessage);
            return await ParseHttpResponseMessageAsync<TSuccess, TValidationEnvelope>(uri, result);

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
            var requestMessage = await CreateHttpRequestMessage(HttpMethod.Post, uri, principal);

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
            return await ParseHttpResponseMessageAsync<T>(uri, result);

        }

        #endregion

        #region ParseHttpResponseMessageAsync

        private async Task<ApiResponse<T>> ParseHttpResponseMessageAsync<T>(string requestUrl, HttpResponseMessage message, bool throwOnNotFound = true)
        {
            AssertJsonContentOrEmpty(message);
            var response = new ApiResponse<T>(message.IsSuccessStatusCode);
            if (message.IsSuccessStatusCode) return response.OK(await DeserializeResponseAsync<T>(message, requestUrl));
            else return await ProcessApiErrorAsync(message, response, requestUrl, throwOnNotFound);
        }

        private async Task<ApiResponse> ParseHttpResponseMessageAsync(string requestUrl, HttpResponseMessage message)
        {
            AssertJsonContentOrEmpty(message);
            var response = new ApiResponse(message.IsSuccessStatusCode);
            if (message.IsSuccessStatusCode) return response;
            else return await ProcessApiErrorAsync(message, response, requestUrl);
        }

        private async Task<ApiResponse<TSuccess, TValidationEnvelope>> ParseHttpResponseMessageAsync<TSuccess, TValidationEnvelope>(string requestUrl, HttpResponseMessage message)
            where TValidationEnvelope : class
        {
            AssertJsonContentOrEmpty(message);
            var response = new ApiResponse<TSuccess, TValidationEnvelope>();
            if (message.IsSuccessStatusCode) return response.Success(await DeserializeResponseAsync<TSuccess>(message, requestUrl));
            else return await ProcessApiErrorAsync(message, response, requestUrl);
        }

        #endregion

        #region Error handling

        private async Task<T> ProcessApiErrorAsync<T>(HttpResponseMessage message, T response, string requestUrl, bool throwOnNotFound = true) where T : ApiResponse
        {
            ValidateGenericHttpErrors(message, throwOnNotFound);
            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var json = await message.Content.ReadAsStringAsync();
                var errors = ParseApiErrors(json);
                Guard.IsTrue(errors.Any(), () => new TexunaApiSystemException($"The API gave a 400 Bad Request response; returned JSON.  But the JSON returned was unrecognizable. (Request URI: {requestUrl}) \r\n body:{json}", GetRequestJsonBody(message.RequestMessage)));
                response.Fail(errors);
            }

            if (message.StatusCode == HttpStatusCode.NotFound)
            {
                response.Success = false;
            }

            return response;
        }

        private async Task<ApiResponse<TSuccess, TValidationEnvelope>> ProcessApiErrorAsync<TSuccess, TValidationEnvelope>(HttpResponseMessage message, ApiResponse<TSuccess, TValidationEnvelope> response, string requestUrl)
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
                    Guard.IsTrue(errors.Any(), () => new TexunaApiSystemException($"The API gave a 400 Bad Request response; returned JSON.  But the JSON returned was unrecognizable. (Request URI: {requestUrl}) \r\n body:{json}", GetRequestJsonBody(message.RequestMessage)));
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
                        throw new TexunaApiSystemException($"The API returned an 'Internal Server Error'. (Request URI: {message.RequestMessage?.RequestUri?.PathAndQuery})", GetRequestJsonBody(message.RequestMessage));
                    case HttpStatusCode.NotFound:
                        if (throwOnNotFound)
                            throw new TexunaApiNotFoundException($"The API returned 404 Not Found. (Request URI: {message.RequestMessage?.RequestUri?.PathAndQuery})");
                        break;
                    case HttpStatusCode.Forbidden:
                        throw new EduSecurityException($"The current principal does not have permission to call this API. (Request URI: {message.RequestMessage?.RequestUri?.PathAndQuery})");
                    default:
                        throw new TexunaApiSystemException($"The API returned an error with status code: {message.StatusCode}. (Request URI: {message.RequestMessage?.RequestUri?.PathAndQuery})", GetRequestJsonBody(message.RequestMessage));
                }
            }
        }

        private void AssertJsonContent(HttpResponseMessage message)
        {
            if (message?.Content?.Headers?.ContentType?.MediaType != "application/json")
            {
                throw new TexunaApiSystemException(
                    $"The API returned an invalid content type: '{message?.Content?.Headers?.ContentType?.MediaType}' (Request URI: {message?.RequestMessage?.RequestUri?.PathAndQuery})", GetRequestJsonBody(message?.RequestMessage));
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

        private async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage message, string requestUri)
        {
            AssertJsonContent(message);
            if (typeof(T) == typeof(string)) return (T)(object)await message.Content.ReadAsStringAsync();
            else if (typeof(T) == typeof(int?)) return (T)(object)(await message.Content.ReadAsStringAsync()).ToInteger();
            else
            {
                var errorLogger = new FormatterErrorLogger();
                var retVal = await message.Content.ReadAsAsync<T>(new[] { _formatter }, errorLogger);
                if (errorLogger.Errors.Any()) throw new TexunaApiSystemException($"Error parsing the JSON returned by the API; (Request URI: {requestUri}) details: {errorLogger.Errors.First().ErrorMessage}");
                return retVal;
            }
        }

        private T TryDeserializeAsync<T>(string json) where T : class
        {
            try { return JsonConvert.DeserializeObject<T>(json); }
            catch { return null; }
        }

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            var startTime = DateTime.UtcNow;
            HttpResponseMessage response = null;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                response = await _httpClient.SendAsync(requestMessage);
                return response;
            }
            catch (TaskCanceledException ex) when (!ex.CancellationToken.IsCancellationRequested
            ) // timeout, apparently: ref; https://stackoverflow.com/questions/29179848/httpclient-a-task-was-cancelled
            {
                throw new TexunaApiSystemException(
                    $"The API did not respond in a timely manner (Request URI: {requestMessage.RequestUri.PathAndQuery})",
                    GetRequestJsonBody(requestMessage));
            }
            finally
            {
                stopwatch.Stop();
                string responseMessage = null;
#if DEBUG
                if (response?.Content != null)
                {
                    responseMessage = await response.Content?.ReadAsStringAsync();
                }

                var context = HttpContext.Current;

                var data = new ApiTraceData
                {
                    StartTime = startTime,
                    DurationMillis = (int)Math.Round((DateTime.UtcNow - startTime).TotalMilliseconds, 0),
                    Method = requestMessage.Method.Method,
                    Url = requestMessage.RequestUri.ToString(),
                    Request = $"{requestMessage.Headers}{Environment.NewLine}{GetRequestJsonBody(requestMessage)}",
                    Response = $"{response?.Headers}{Environment.NewLine}{responseMessage}",
                    ResponseCode = response != null ? (int)response.StatusCode : 0,
                    ClientIpAddress = context?.Request.UserHostAddress,
                    UserId = context?.User?.Identity?.GetUserId(),
                    UserName = context?.User?.Identity?.GetUserName()
                };

                ApiTrace.Data.Add(data);
#endif
                
                await LogApiInteraction(requestMessage, response, responseMessage, stopwatch.Elapsed);
            }
        }

        private async Task LogApiInteraction(HttpRequestMessage requestMessage, HttpResponseMessage response, string responseMessage, TimeSpan elapsed)
        {
            var apiSessionId = _clientStorage?.Get("ApiSessionId");
            if (apiSessionId != null && _apiRecorderSessionItemRepository != null)
            {
                if (responseMessage == null && response?.Content != null)
                {
                    responseMessage = await response.Content?.ReadAsStringAsync();
                }

                await _apiRecorderSessionItemRepository.CreateAsync(new Data.Entity.ApiRecorderSessionItem(apiSessionId, requestMessage.RequestUri.AbsolutePath)
                {
                    HttpMethod = requestMessage.Method.ToString(),
                    RawRequestBody = GetRequestJsonBody(requestMessage),
                    RawResponseBody = responseMessage.Ellipsis(32000),
                    RequestHeaders = ToJsonIndented(requestMessage.Headers),
                    ResponseHeaders = ToJsonIndented(response.Headers),
                    ElapsedTimeSpan = elapsed.ToString(),
                    ElapsedMS = elapsed.TotalMilliseconds
                });
            }
        }

        private async Task<HttpRequestMessage> CreateHttpRequestMessage(HttpMethod method, string uri, IPrincipal principal, object requestBodyData = null)
        {
            if (uri.StartsWith("/")) throw new Exception($"{nameof(uri)} parameter value starts with a forward-slash.  You didn't want to do that.");

            var requestMessage = new HttpRequestMessage(method, uri);
            requestMessage.Headers.Add(HEADER_SA_USER_ID, principal.GetUserId() ?? string.Empty);
            if (requestBodyData != null)
            {
                requestMessage.Content = new ObjectContent<object>(requestBodyData, _formatter);
                requestMessage.Properties[REQ_BODY_JSON_PAYLOAD] = await requestMessage.Content.ReadAsStringAsync();
            }
            else requestMessage.Properties[REQ_BODY_JSON_PAYLOAD] = string.Empty;

            return requestMessage;
        }

        private string GetRequestJsonBody(HttpRequestMessage msg) => msg != null && msg.Properties.ContainsKey(REQ_BODY_JSON_PAYLOAD) ? msg.Properties[REQ_BODY_JSON_PAYLOAD]?.ToString() ?? string.Empty : string.Empty;

        public string ToJsonIndented(HttpHeaders headers)
        {
            var dict = ToDictionary(headers);
            return JsonConvert.SerializeObject(dict, Formatting.Indented);
        }

        private static Dictionary<string, string> ToDictionary(HttpHeaders headers)
        {
            var dict = new Dictionary<string, string>();
            foreach (var item in headers.ToList())
            {
                if (item.Value != null)
                {
                    var header = item.Value.Aggregate(string.Empty, (current, value) => $"{current}{value} ");
                    header = header.TrimEnd(" ".ToCharArray());
                    dict.Add(item.Key, header);
                }
            }
            return dict;
        }

        #endregion
    }
}
