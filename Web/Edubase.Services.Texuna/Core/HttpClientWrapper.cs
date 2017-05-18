namespace Edubase.Services
{
    using Common.IO;
    using Exceptions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
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
            _httpClient.Timeout = TimeSpan.FromSeconds(20);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", new BasicAuthCredentials(ApiUsername, ApiPassword).ToString()); 
            _formatter.SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new TexunaCamelCasePropertyNamesContractResolver()
            };
        }

        public async Task<T> GetAsync<T>(string uri, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: GET {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Get, uri, principal);
                var result = await _httpClient.SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync<T>(result);
            }
        }
        
        public async Task<T> PostAsync<T>(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: POST {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, uri, principal, data);
                var result = await _httpClient.SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync<T>(result);
            }
        }
        
        public async Task PostAsync(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: POST {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, uri, principal, data);
                var result = await _httpClient.SendAsync(requestMessage);
                Validate(result);
            }
        }

        public async Task<T> PutAsync<T>(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: PUT {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Put, uri, principal, data);
                var result = await _httpClient.SendAsync(requestMessage);
                return await ParseHttpResponseMessageAsync<T>(result);
            }
        }

        public async Task PutAsync(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: PUT {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Put, uri, principal, data);
                var result = await _httpClient.SendAsync(requestMessage);
                Validate(result);
            }
        }

        public async Task PatchAsync(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: PUT {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(new HttpMethod("PATCH"), uri, principal, data);
                var result = await _httpClient.SendAsync(requestMessage);
                Validate(result);
            }
        }

        public async Task DeleteAsync(string uri, object data, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: DELETE {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Delete, uri, principal, data);
                var result = await _httpClient.SendAsync(requestMessage);
                Validate(result);
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
        public async Task<T> PostMultipartAsync<T>(string uri, object data, string fileName, IPrincipal principal)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: POST (multipart) {uri}"))
            {
                var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, uri, principal);

                var content = new MultipartContent("form-data", Guid.NewGuid().ToString());

                var fileContent = new ByteArrayContent(File.ReadAllBytes(fileName));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(new FileHelper().GetMimeType(fileName));
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                fileContent.Headers.ContentDisposition.Name = "bulkfile"; // shouldn't be necessary, but it is.
                fileContent.Headers.ContentDisposition.FileName = Path.GetFileName(fileName); // shouldn't be necessary
                content.Add(fileContent);

                var jsonPayload = new ObjectContent<object>(data, _formatter);
                jsonPayload.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                jsonPayload.Headers.ContentDisposition.Name = "payload"; // shouldn't be necessary, but it is.
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

        private async Task<T> ParseHttpResponseMessageAsync<T>(HttpResponseMessage message)
        {
            if (message.IsSuccessStatusCode)
            {
                if (!message.Content.Headers.ContentType.MediaType.Equals("application/json"))
                    throw new TexunaApiSystemException($"The TEX-API returned an invalid content type: '{message.Content.Headers.ContentType.MediaType}' (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
                return await message.Content.ReadAsAsync<T>(new[] { _formatter });
            }
            else throw new TexunaApiSystemException($"The TEX-API returned an error with status code: {message.StatusCode}. (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
        }

        private void Validate(HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode) throw new TexunaApiSystemException($"The TEX-API returned an error with status code: {message.StatusCode}. (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
        }
    }
}
