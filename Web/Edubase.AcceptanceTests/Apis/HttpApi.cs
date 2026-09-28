using Newtonsoft.Json;

namespace Edubase.AcceptanceTests.Apis
{
    public class HttpApi : IApi
    {
        protected readonly HttpClient httpClient;

        public HttpApi(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var response = await httpClient.GetAsync($"{httpClient.BaseAddress}{url}");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var responseType = JsonConvert.DeserializeObject<T>(json);

            return responseType;
        }
    }
}
