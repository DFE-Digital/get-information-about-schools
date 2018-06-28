using Edubase.Common;
using Edubase.Common.Spatial;
using Edubase.Services.IntegrationEndPoints.OSPlaces.Models;
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
        static readonly string _apiKey = ConfigurationManager.AppSettings["OSPlacesApiKey"];

        public async Task<OSPlacesItemDto[]> SearchAsync(string text)
        {
            var retVal = new OSPlacesItemDto[0];
            text = text.Clean();

            if (text != null && Regex.IsMatch(text, @"\b[A-Z]{1,2}[0-9][A-Z0-9]? [0-9][ABD-HJLNP-UW-Z]{2}\b", RegexOptions.IgnoreCase))
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var message = await client.GetAsync($"https://api.ordnancesurvey.co.uk/places/v1/addresses/postcode?postcode={text}&key={_apiKey}&output_srs=WGS84");
                        var response = await ParseHttpResponseMessageAsync<OSPlacesResponse>(message);
                        return response?.Results.Where(x => x.Dpa != null).Select(x => x.Dpa).Select(x => new OSPlacesItemDto(UriHelper.SerializeToUrlToken(LatLon.Create(x.Lat, x.Lng)), x.Address)).ToArray() ?? new OSPlacesItemDto[0];
                    }
                }
                catch { }
            }

            return retVal;
        }

        public LatLon GetCoordinate(string placeId)
        {
            placeId = placeId.Clean();
            if (placeId == null) throw new ArgumentNullException(nameof(placeId));
            return UriHelper.DeserializeUrlToken<LatLon>(placeId);
        }

        private async Task<T> ParseHttpResponseMessageAsync<T>(HttpResponseMessage message)
        {
            if (message.IsSuccessStatusCode)
            {
                if (!message.Content.Headers.ContentType.MediaType.Equals("application/json"))
                    throw new Exception($"The API returned an invalid content type: '{message.Content.Headers.ContentType.MediaType}' (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
                return await message.Content.ReadAsAsync<T>(new[] { new JsonMediaTypeFormatter() });
            }
            else throw new Exception($"The API returned an error with status code: {message.StatusCode}. (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
        }
    }
}
