namespace Edubase.Services
{
    using Exceptions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using StackExchange.Profiling;
    using System;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;
    using Texuna.Serialization;

    public class HttpClientWrapper
    {
        private readonly HttpClient _httpClient;
        private readonly JsonMediaTypeFormatter _formatter = new JsonMediaTypeFormatter();

        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(20);
            _httpClient.DefaultRequestHeaders.Add("sa_user_id", "3600026"); // TODO: remove this after auth is done!
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "cmVzdC1hcGktdXNlcjp6ITdrVSJYOyVmI0s+I2U7"); // TODO: remove this after auth is done!
            _formatter.SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new TexunaCamelCasePropertyNamesContractResolver()
            };
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: GET {uri}"))
            {
                var result = await _httpClient.GetAsync(uri);
                return await ParseHttpResponseMessageAsync<T>(result);
            }
        }
        
        public async Task<T> PostAsync<T>(string uri, object data)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: POST {uri}"))
            {
                var result = await _httpClient.PostAsync(uri, data, _formatter);
                return await ParseHttpResponseMessageAsync<T>(result);
            }
        }
        
        public async Task<bool> PostAsync(string uri, object data)
        {
            using (MiniProfiler.Current.Step($"TEXAPI: POST {uri}"))
            {
                var result = await _httpClient.PostAsync(uri, data, _formatter);
                return result.IsSuccessStatusCode;
            }
        }

        private async Task<T> ParseHttpResponseMessageAsync<T>(HttpResponseMessage message)
        {
            if (message.IsSuccessStatusCode)
            {
                if (!message.Content.Headers.ContentType.MediaType.Equals("application/json"))
                    throw new TexunaApiSystemException($"The API returned an invalid content type: '{message.Content.Headers.ContentType.MediaType}' (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
                return await message.Content.ReadAsAsync<T>(new[] { _formatter });
            }
            else throw new TexunaApiSystemException($"The API returned an error with status code: {message.StatusCode}. (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
        }
    }
}
