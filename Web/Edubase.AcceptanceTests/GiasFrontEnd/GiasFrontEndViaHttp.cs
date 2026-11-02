using System.Globalization;
using System.Text;
using AngleSharp.Common;
using Edubase.AcceptanceTests.Apis;
using Edubase.AcceptanceTests.SigninAuthorities;
using Edubase.AcceptanceTests.UserPreferences;
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
                Content = new FormUrlEncodedContent(BuildSearchFilterFormData(establishmentSearchFilter))
            };

            request.Headers.Add("RequestVerificationToken", csrfToken);

            using var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

            var token = (string?) payload["token"];

            return token ?? throw new InvalidOperationException("Token not found.");
        }

        private static IEnumerable<KeyValuePair<string, string>> BuildSearchFilterFormData(EstablishmentSearchFilter filter)
        {
            var fields = new List<KeyValuePair<string, string>>();

            AddField(fields, "SearchType", filter.SearchType);
            AddField(fields, "filter-set", filter.FilterSet);
            AddField(fields, "SavedFilterToken", filter.SavedFilterToken);

            if (filter.SearchType == "Text")
            {
                AddField(fields, "TextSearchModel.Text", filter.TextSearchText);
                AddField(fields, "TextSearchModel.AutoSuggestValue", filter.TextSearchAutoSuggestValue);
            }

            if (filter.SearchType == "Location")
            {
                AddField(fields, "LocationSearchModel.Text", filter.LocationSearchText);
                AddField(fields, "LocationSearchModel.AutoSuggestValue", filter.LocationSearchAutoSuggestValue);
                fields.Add(new KeyValuePair<string, string>(
                    "aa",
                    (filter.RadiusInMiles ?? 3).ToString("0.##", CultureInfo.InvariantCulture)));
            }

            var sortBy = filter.SortBy ?? (filter.SearchType == "Location" ? "d" : "a");
            AddField(fields, "z", sortBy);

            AddValues(fields, "a", filter.EstablishmentTypeIds);
            AddValues(fields, "b", filter.StatusIds);
            AddValues(fields, "c", filter.EducationPhaseIds);
            AddValues(fields, "d", filter.LocalAuthorityIds);
            AddValues(fields, "e", filter.ReligiousCharacterIds);
            AddValues(fields, "g", filter.AdmissionsPolicyIds);
            AddValues(fields, "h", filter.BoardingProvisionIds);
            AddValues(fields, "i", filter.DioceseIds);
            AddValues(fields, "j", filter.DistrictIds);
            AddValues(fields, "k", filter.FurtherEducationTypeIds);
            AddValues(fields, "l", filter.GenderIds);
            AddValues(fields, "m", filter.GovernmentOfficeRegionIds);
            AddValues(fields, "n", filter.NurseryProvisionIds);
            AddValues(fields, "p", filter.ParliamentaryConstituencyIds);
            AddValues(fields, "8", filter.RegistrationStatusIds);
            AddValues(fields, "q", filter.ReligiousEthosIds);
            AddValues(fields, "r", filter.RscRegionIds);
            AddValues(fields, "s", filter.Section41Ids);
            AddValues(fields, "t", filter.SixthFormProvisionIds);
            AddValues(fields, "u", filter.SpecialClassesProvisionIds);
            AddValues(fields, "v", filter.TypeOfSenProvisionIds);
            AddValues(fields, "w", filter.UrbanRuralIds);

            AddDate(fields, "OpenDateFrom", filter.OpenDateFrom);
            AddDate(fields, "OpenDateTo", filter.OpenDateTo);
            AddDate(fields, "CloseDateFrom", filter.CloseDateFrom);
            AddDate(fields, "CloseDateTo", filter.CloseDateTo);
            AddAgeRange(fields, "AgeRangeLow", filter.AgeRangeLow);
            AddAgeRange(fields, "AgeRangeHigh", filter.AgeRangeHigh);

            return fields
                .Distinct()
                .OrderBy(field => $"{field.Key}={field.Value}", StringComparer.Ordinal);
        }

        private static void AddField(ICollection<KeyValuePair<string, string>> fields, string name, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                fields.Add(new KeyValuePair<string, string>(name, value));
            }
        }

        private static void AddValues(ICollection<KeyValuePair<string, string>> fields, string name, IEnumerable<int> values)
        {
            foreach (var value in values)
            {
                fields.Add(new KeyValuePair<string, string>(name, value.ToString(CultureInfo.InvariantCulture)));
            }
        }

        private static void AddDate(ICollection<KeyValuePair<string, string>> fields, string name, DateTime? value)
        {
            if (!value.HasValue)
            {
                return;
            }

            var date = value.Value;
            fields.Add(new KeyValuePair<string, string>($"{name}.Day", date.Day.ToString("00", CultureInfo.InvariantCulture)));
            fields.Add(new KeyValuePair<string, string>($"{name}.Month", date.Month.ToString("00", CultureInfo.InvariantCulture)));
            fields.Add(new KeyValuePair<string, string>($"{name}.Year", date.Year.ToString("0000", CultureInfo.InvariantCulture)));
        }

        private static void AddAgeRange(ICollection<KeyValuePair<string, string>> fields, string name, AgeRangeFilter? value)
        {
            if (value?.From is int from)
            {
                fields.Add(new KeyValuePair<string, string>($"{name}.From", from.ToString(CultureInfo.InvariantCulture)));
            }

            if (value?.To is int to)
            {
                fields.Add(new KeyValuePair<string, string>($"{name}.To", to.ToString(CultureInfo.InvariantCulture)));
            }
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
    }
}
