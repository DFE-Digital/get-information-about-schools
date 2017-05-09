namespace Edubase.Services
{
    using Exceptions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using StackExchange.Profiling;
    using System;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Texuna;
    using Texuna.Serialization;

    public class HttpClientWrapper
    {
        private readonly HttpClient _httpClient;
        private readonly JsonMediaTypeFormatter _formatter = new JsonMediaTypeFormatter();
        private const string HEADER_SA_USER_ID = "sa_user_id";

        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(20);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "cmVzdC1hcGktdXNlcjp6ITdrVSJYOyVmI0s+I2U7"); // TODO: remove this after auth is done!
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
