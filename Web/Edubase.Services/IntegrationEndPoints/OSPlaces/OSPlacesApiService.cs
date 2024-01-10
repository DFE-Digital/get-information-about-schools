using Edubase.Common;
using Edubase.Common.Spatial;
using Edubase.Services.Geo;
using Edubase.Services.IntegrationEndPoints.OSPlaces.Models;
using Polly;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.OSPlaces
{
    public class OSPlacesApiService : IOSPlacesApiService
    {
        private static readonly string _apiKey = ConfigurationManager.AppSettings["OSPlacesApiKey"];
        private static readonly HttpClient _osApiClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.os.uk/")
        };

        private static readonly Policy RetryPolicy = CreateRetryPolicy();

        private static Policy CreateRetryPolicy()
        {
            var retryIntervalSettings = ConfigurationManager.AppSettings["OSPlacesApiServices_RetryIntervals"];

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
                return new[] { TimeSpan.FromSeconds(10) };
            }

            return retryIntervals;
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
                        return new PlaceDto[0];
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
            if (message.IsSuccessStatusCode)
            {
                if (!message.Content.Headers.ContentType.MediaType.Equals("application/json"))
                {
                    throw new Exception($"The API returned an invalid content type: '{message.Content.Headers.ContentType.MediaType}' (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
                }

                return message.Content.ReadAsAsync<T>(new[] { new JsonMediaTypeFormatter() });
            }
            else
            {
                throw new Exception($"The API returned an error with status code: {message.StatusCode}. (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
            }
        }
    }
}
