using System.Globalization;
using System.Text;
using AngleSharp.Common;
using Edubase.AcceptanceTests.Apis;
using Edubase.AcceptanceTests.Establishments;
using Edubase.AcceptanceTests.SigninAuthorities;
using Edubase.AcceptanceTests.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Edubase.AcceptanceTests.GiasFrontEnd
{
    public class GiasFrontEndViaHttp : HttpApi, IGiasFrontEnd
    {
        private readonly ISignInAuthority signInAuthority;

        public GiasFrontEndViaHttp(HttpClient httpClient, ISignInAuthority signInAuthority)
            : base(httpClient)
        {
            this.signInAuthority = signInAuthority;
        }

        public async Task SignIn(User user)
        {
            await signInAuthority.SignIn(user);
        }

        public async Task<string> TokenizeEstablishmentSearchFilter(EstablishmentSearchFilter establishmentSearchFilter)
        {
            var page = await GetHtmlAsync("/Establishments/Search?SearchType=EstablishmentAll&SelectedTab=Establishments");

            var csrfToken = page.QuerySelector("meta[name='csrf-token']")
                ?.GetAttribute("content")
                ?? throw new InvalidOperationException("CSRF token not found.");

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/tokenize")
            {
                Content = establishmentSearchFilter.AsFormUrlEncodedContent()
            };

            request.Headers.Add("RequestVerificationToken", csrfToken);

            using var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

            var token = (string?) payload["token"];

            return token ?? throw new InvalidOperationException("Token not found.");
        }

        public async Task SaveEstablishmentSearchFilter(string token)
        {
            var page = await GetHtmlAsync("/Establishments/Search?SearchType=EstablishmentAll&SelectedTab=Establishments");

            var csrfToken = page.QuerySelector("meta[name='csrf-token']")
                ?.GetAttribute("content")
                ?? throw new InvalidOperationException("CSRF token not found.");

            var json = JsonConvert.SerializeObject(new { token });

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/api/save-search-token")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("RequestVerificationToken", csrfToken);

            using var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        public async Task<EstablishmentSearchResults> GetEstablishmentSearchResults(string token)
        {
            var escapedToken = Uri.EscapeDataString(token);
            return await GetEstablishmentSearchResultsFromJsonAsync($"?tok={escapedToken}");
        }

        public async Task<EstablishmentSearchResults> GetEstablishmentSearchResults(EstablishmentSearchFilter filter)
        {
            using var content = filter.AsFormUrlEncodedContent();
            var queryString = await content.ReadAsStringAsync();

            return await GetEstablishmentSearchResultsFromJsonAsync($"?{queryString}");
        }

        private async Task<EstablishmentSearchResults> GetEstablishmentSearchResultsFromJsonAsync(string queryString)
        {
            var establishments = await GetAsync<List<EstablishmentSearchResult>>(
                $"/Establishments/Search/results-json{queryString}");

            return new EstablishmentSearchResults { Establishments = establishments };
        }
    }
}
