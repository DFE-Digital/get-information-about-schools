using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Common.Spatial;
using Edubase.Services.Geo;
using Edubase.Services.IntegrationEndPoints.OSPlaces.Models;
using Microsoft.Extensions.Configuration;
using Polly;

namespace Edubase.Services.IntegrationEndPoints.OSPlaces
{
    public class OSPlacesApiService : IOSPlacesApiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _osApiClient;
        private readonly IAsyncPolicy<HttpResponseMessage> RetryPolicy;

        public OSPlacesApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _osApiClient = httpClient;

            // Read configuration values using IConfiguration
            _apiKey = configuration["AppSettings:OSPlacesApiKey"] ?? throw new ArgumentNullException("OSPlacesApiKey is missing");

            var retryCsv = configuration["AppSettings:OSPlacesApiServices_RetryIntervals"] ?? "1,2,5";
            var timeoutValue = configuration["AppSettings:OSPlacesApiServices_Timeout"] ?? "10";

            var retryIntervals = PollyUtil.CsvSecondsToTimeSpans(retryCsv);
            RetryPolicy = PollyUtil.CreateRetryPolicy(configuration, retryIntervals, timeoutValue);
        }

        public OSPlacesApiService(HttpClient httpClient)
        {
            _osApiClient = httpClient;
        }

        public async Task<PlaceDto[]> SearchAsync(string text)
        {
            var retVal = new PlaceDto[0];
            text = text.Clean();

            if (text != null && Regex.IsMatch(text, @"\b[A-Z]{1,2}[0-9][A-Z0-9]? [0-9][ABD-HJLNP-UW-Z]{2}\b", RegexOptions.IgnoreCase))
            {
                using (var message = await RetryPolicy.ExecuteAsync(async () =>
                    {
                        return await _osApiClient.GetAsync($"search/places/v1/postcode?postcode={text}&key={_apiKey}&output_srs=WGS84&dataset=DPA,LPI");
                    }))
                {
                    if (!message.IsSuccessStatusCode)
                    {
                        return [];
                    }

                    var response = await ParseHttpResponseMessageAsync<OSPlacesResponse>(message);

                    var addresses = response.Results?.Where(x => x.OSAddress != null
                        && (x.OSAddress.PostalAddressCode == "D" || x.OSAddress.PostalAddressCode == "L")) // POSTAL_ADDRESS_CODE_DESCRIPTION: D = "A record which is linked to PAF", L="A record which is identified as postal based on Local Authority information"
                        .Select(x => x.OSAddress)
                        .GroupBy(x => x.Uprn)
                        .Select(x => x.FirstOrDefault(u => u.Address.Length == x.Max(y => y.Address.Length)))
                        .OrderBy(x => x.Address);

                    if (addresses != null)
                    {
                        return addresses.Select(x => new PlaceDto(x.Address, LatLon.Create(x.Lat, x.Lng))).ToArray();
                    }
                }
            }

            return retVal;
        }

        private Task<T> ParseHttpResponseMessageAsync<T>(HttpResponseMessage message)
        {
            return message.IsSuccessStatusCode
                ? !message.Content.Headers.ContentType.MediaType.Equals("application/json")
                    ? throw new Exception($"The API returned an invalid content type: '{message.Content.Headers.ContentType.MediaType}' (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})")
                    : message.Content.ReadAsAsync<T>([new JsonMediaTypeFormatter()])
                : throw new Exception($"The API returned an error with status code: {message.StatusCode}. (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
        }
    }
}
