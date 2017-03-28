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
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var result = await _httpClient.GetAsync(uri);
            return await ParseHttpResponseMessageAsync<T>(result);
        }

        public T Get<T>(string uri)
        {
            throw new NotImplementedException();
        }

        public async Task<T> PostAsync<T>(string uri, object data)
        {
            var result = await _httpClient.PostAsJsonAsync(uri, data);
            return await ParseHttpResponseMessageAsync<T>(result);
        }

        public T Post<T>(string uri, object data)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PostAsync(string uri, object data)
        {
            var result = await _httpClient.PostAsJsonAsync(uri, data);
            return result.IsSuccessStatusCode;
        }

        public bool Post(string uri, object data)
        {
            throw new NotImplementedException();
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
