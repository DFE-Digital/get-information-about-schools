namespace Edubase.Services
{
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
            var result = _httpClient.GetAsync(uri).Result;
            return ParseHttpResponseMessage<T>(result);
        }

        public async Task<T> PostAsync<T>(string uri, object data)
        {
            var result = await _httpClient.PostAsJsonAsync(uri, data);
            return await ParseHttpResponseMessageAsync<T>(result);
        }

        public T Post<T>(string uri, object data)
        {
            var result = _httpClient.PostAsJsonAsync(uri, data).Result;
            return ParseHttpResponseMessage<T>(result);
        }

        public async Task<bool> PostAsync(string uri, object data)
        {
            var result = await _httpClient.PostAsJsonAsync(uri, data);
            return result.IsSuccessStatusCode;
        }

        public bool Post(string uri, object data)
        {
            var result = _httpClient.PostAsJsonAsync(uri, data).Result;
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

        private T ParseHttpResponseMessage<T>(HttpResponseMessage message)
        {
            if (message.IsSuccessStatusCode)
            {
                return message.Content.ReadAsAsync<T>().Result;
            }

            return default(T);
        }
    }
}
