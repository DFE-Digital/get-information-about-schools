using AngleSharp.Html.Dom;
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

        public async Task<IHtmlDocument> GetHtmlAsync(string url)
        {
            using var response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.GetHtmlDocumentAsync();
        }

        public async Task<IHtmlDocument> PostFormAsync(string url, IEnumerable<KeyValuePair<string, string>> formData)
        {
            using var content = new FormUrlEncodedContent(formData);
            using var response = await httpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            return await response.GetHtmlDocumentAsync();
        }
    }
}
