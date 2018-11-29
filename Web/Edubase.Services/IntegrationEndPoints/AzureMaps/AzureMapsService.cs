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

        private static readonly Policy RetryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4)
                });

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
                    throw new Exception(
                        $"The API returned an error with status code: {response.StatusCode}. (Request URI: {request.RequestUri.PathAndQuery})");
                }

                using (var sr = new StreamReader(stream))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    var azureMapsResponse = serializer.Deserialize<AzureMapsSearchResponseDto>(reader);
                    var retVal = azureMapsResponse.results
                        .Where(result => result.type != "Cross Street"
                                         && !(result.entityType != null && result.entityType == "CountrySecondarySubdivision"))
                        .Select(x => new PlaceDto(GetAddressDescription(x), new LatLon(x.position.lat, x.position.lon)))
                        .ToArray();

                    // If the search string is a postcode and none of the returned results contain the given post code, then return zero results, so that the search is deferred to OS places.
                    if (text.IsUkPostCode())
                    {
                        var postCode = text.Remove(" ").ToLower();
                        if (!retVal.Any(x => (x.Name ?? "").ToLower().Remove(" ").Contains(postCode)))
                        {
                            return new PlaceDto[0];
                        }
                    }

                    return retVal;
                }
            }
        }

        private static string GetAddressDescription(Result locationResult)
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

            return $"{output}, {locationResult.address.countrySecondarySubdivision}";
        }

    }
}
