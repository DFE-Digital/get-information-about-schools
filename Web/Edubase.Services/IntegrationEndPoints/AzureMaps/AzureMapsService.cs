namespace Edubase.Services.IntegrationEndPoints.AzureMaps
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common;
    using Edubase.Common.Spatial;
    using Edubase.Services.Geo;
    using Edubase.Services.IntegrationEndPoints.AzureMaps.Models;
    using Newtonsoft.Json;
    using Polly;

    public class AzureMapsService : IAzureMapsService
    {
        private static readonly string _apiKey = ConfigurationManager.AppSettings["AzureMapsApiKey"];

        private static readonly HttpClient _azureMapsClient = new HttpClient
        {
            BaseAddress = new Uri("https://atlas.microsoft.com")
        };

        private static readonly Policy RetryPolicy = CreateRetryPolicy();

        private static Policy CreateRetryPolicy()
        {
            var retryIntervalSettings = ConfigurationManager.AppSettings["AzureMapService_TimeoutIntervals"];

            if (string.IsNullOrEmpty(retryIntervalSettings?.Trim()))
            {
                return Policy.NoOp();
            }

            var retryIntervals = CsvSecondsToTimeSpans(retryIntervalSettings);

            return Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(retryIntervals);
        }

        private static TimeSpan[] CsvSecondsToTimeSpans(string csvSeconds)
        {
            var retryIntervals = csvSeconds.Split(',')
                .Select(x => x.Trim())
                .Where(x => int.TryParse(x, out var seconds) && seconds >= 0)
                .Select(x => TimeSpan.FromSeconds(int.Parse(x)))
                .ToArray();

            if (retryIntervals.Length == 0)
            {
                return new[] { TimeSpan.FromSeconds(2) };
            }

            return retryIntervals;
        }

        public async Task<PlaceDto[]> SearchAsync(string text, bool isTypeahead)
        {
            text = text.Clean();

            if (string.IsNullOrWhiteSpace(text))
            {
                return new PlaceDto[0];
            }

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/search/address/json?api-version=1.0&countrySet=GB&typeahead={(isTypeahead ? "true" : "false")}&limit=10&query={text}&subscription-key={_apiKey}");

            using (var response = await RetryPolicy.ExecuteAsync(async () => await _azureMapsClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)))
            {
                var stream = await response.Content.ReadAsStreamAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new PlaceDto[0];
                }

                using (var sr = new StreamReader(stream))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    var azureMapsResponse = serializer.Deserialize<AzureMapsSearchResponseDto>(reader);
                    var results = azureMapsResponse.results
                        .Where(result => result.type != "Cross Street"
                                         && !(result.entityType != null && result.entityType == "CountrySecondarySubdivision"))
                        .ToList();
                    
                    var municipalities = results.Where(x => x.entityType == "Municipality").ToList();
                    var subMunicipalities = results.Where(x => x.entityType == "MunicipalitySubdivision").ToList();
                    // If the response contains a "MunicipalitySubdivision" with the same name as a returned Municipality (town),
                    // use the coordinates of that result for the position of the town and remove it from the result set.
                    // This addresses an issue where a small number of towns have inaccurate coordinates associated with them.
                    foreach (var municipality in municipalities)
                    {
                        var child = subMunicipalities.FirstOrDefault(
                            x => x.address.municipality == municipality.address.municipality
                                 && x.address.municipalitySubdivision == municipality.address.municipality);
                        if (child == null)
                        {
                            continue;
                        }
                        municipality.position.lat = child.position.lat;
                        municipality.position.lon = child.position.lon;
                        results.Remove(child);
                    }

                    var parsedResults = results.Select(x => new PlaceDto(GetAddressDescription(x, text), new LatLon(x.position.lat, x.position.lon))).ToArray();

                    // If the search string is a postcode and none of the returned results contain the given post code, then return zero results, so that the search is deferred to OS places.
                    if (text.IsUkPostCode())
                    {
                        var postCode = text.Remove(" ").ToLower();
                        if (!parsedResults.Any(x => (x.Name ?? "").ToLower().Remove(" ").Contains("," + postCode.Remove(" "))))
                        {
                            return new PlaceDto[0];
                        }
                    }

                    return parsedResults;
                }
            }
        }

        private static string GetAddressDescription(Result locationResult, string text)
        {
            var output = "";

            if (locationResult.entityType != null && locationResult.entityType.ToString() == "MunicipalitySubdivision")
            {
                output = $"{locationResult.address.municipalitySubdivision}, {locationResult.address.municipality}";
            }
            else
            {
                output = locationResult.address.freeformAddress;
            }

            // if a location shares multiple postcodes, azure does not include it within the normal freeformaddress. So we need to build the appropriate address.
            if (locationResult.address.postalCode != null && !output.Contains(locationResult.address.postalCode.Split(',')[0]))
            {
                if (text.IsUkPostCode())
                {
                    if (locationResult.address.extendedPostalCode.ToLower().Remove(" ").Contains(text.ToLower().Remove(" ")))
                    {
                        output += $", {text.ToUpper()}";
                    }
                    else
                    {
                        output += $", {locationResult.address.postalCode.Split(',')[0]}";
                    }
                }
            }

            return $"{output}, {locationResult.address.countrySecondarySubdivision}";
        }

    }
}
