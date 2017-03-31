namespace Edubase.Services
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class HttpClientWrapper
    {
        private readonly HttpClient _httpClient;

        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("sa_user_id", "3600026");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "cmVzdC1hcGktdXNlcjp6ITdrVSJYOyVmI0s+I2U7");

        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var result = await _httpClient.GetAsync(uri);
            return await ParseHttpResponseMessageAsync<T>(result);
        }
        
        public async Task<T> PostAsync<T>(string uri, object data)
        {
            var result = await _httpClient.PostAsJsonAsync(uri, data);
            return await ParseHttpResponseMessageAsync<T>(result);
        }
        
        public async Task<bool> PostAsync(string uri, object data)
        {
            var result = await _httpClient.PostAsJsonAsync(uri, data);
            return result.IsSuccessStatusCode;
        }
        
        private async Task<T> ParseHttpResponseMessageAsync<T>(HttpResponseMessage message)
        {
            if (message.IsSuccessStatusCode)
            {
                return await message.Content.ReadAsAsync<T>();
            }

            return default(T);
        }
    }
}
