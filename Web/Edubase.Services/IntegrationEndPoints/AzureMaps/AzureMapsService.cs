namespace Edubase.Services.IntegrationEndPoints.AzureMaps
{
    using Common;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Edubase.Services.Geo;
    using Polly;
    using System.Net;
    using Newtonsoft.Json;
    using Edubase.Services.IntegrationEndPoints.AzureMaps.Models;
    using System.IO;
    using Edubase.Common.Spatial;

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
        
        public async Task<PlaceDto[]> SearchAsync(string text)
        {
            text = text.Clean();

            if (string.IsNullOrWhiteSpace(text))
            {
                return new PlaceDto[0];
            }

            var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"/search/address/json?api-version=1.0&countrySet=GB&typeahead=true&limit=10&query={text}&subscription-key={_apiKey}");

            using (var response = await RetryPolicy.ExecuteAsync(async () =>
                {
                    return await _azureMapsClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                }))
            {
                var stream = await response.Content.ReadAsStreamAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"The API returned an error with status code: {response.StatusCode}. (Request URI: {request.RequestUri.PathAndQuery})");
                }

                AzureMapsSearchResponseDto azureMapsResponse;

                using (var sr = new StreamReader(stream))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();

                    azureMapsResponse = serializer.Deserialize<AzureMapsSearchResponseDto>(reader);
                }

                return azureMapsResponse.results
                    .Where(result => result.type != "Cross Street"
                        && !(result.entityType != null && result.entityType == "CountrySecondarySubdivision"))
                    .Select(x => new PlaceDto(GetAddressDescription(x), new LatLon(x.position.lat, x.position.lon)))
                    .ToArray();
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
