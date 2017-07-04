using Edubase.Common.Spatial;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.Google
{
    using Common;
    using Models;
    using System;
    using System.Net.Http.Formatting;

    public class GooglePlacesService : IGooglePlacesService
    {
        static readonly string _apiKey = ConfigurationManager.AppSettings["GoogleApiKey"];
        readonly JsonMediaTypeFormatter _formatter = new JsonMediaTypeFormatter();

        public async Task<AutocompleteItemDto[]> SearchAsync(string text)
        {
            text = text.Clean();
            if (text == null) return new AutocompleteItemDto[0];
            
            using (var client = new HttpClient())
            {
                var message = await client.GetAsync($"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={text}&key={_apiKey}&components=country:GB&types=(cities)");
                var response = await ParseHttpResponseMessageAsync<AutocompleteApiQueryResponse>(message);
                ValidateResponse(response);
                return response.predictions.Select(x => new AutocompleteItemDto(x.place_id, x.description)).ToArray();
            }
        }

        public async Task<LatLon> GetCoordinateAsync(string placeId)
        {
            placeId = placeId.Clean();
            if (placeId == null) throw new ArgumentNullException(nameof(placeId));

            using (var client = new HttpClient())
            {
                var message = await client.GetAsync($"https://maps.googleapis.com/maps/api/place/details/json?placeid={placeId}&key={_apiKey}");
                var response = await ParseHttpResponseMessageAsync<PlaceDetailApiResponse>(message);
                ValidateResponse(response);
                var loc = response?.result?.geometry?.location;
                return LatLon.Create(loc?.lat, loc?.lng);
            }
        }

        private void ValidateResponse(AutocompleteApiQueryResponse response)
        {
            if (response.status != "OK" && response.status != "ZERO_RESULTS")
                throw new GoogleApiException($"The Google API return error status: {response.status}");
            if (response.predictions == null) response.predictions = new List<Prediction>();
        }

        private void ValidateResponse(PlaceDetailApiResponse response)
        {
            if (response.status != "OK" && response.status != "ZERO_RESULTS")
                throw new GoogleApiException($"The Google API return error status: {response.status}");
        }

        private async Task<T> ParseHttpResponseMessageAsync<T>(HttpResponseMessage message)
        {
            if (message.IsSuccessStatusCode)
            {
                if (!message.Content.Headers.ContentType.MediaType.Equals("application/json"))
                    throw new GoogleApiException($"The API returned an invalid content type: '{message.Content.Headers.ContentType.MediaType}' (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
                return await message.Content.ReadAsAsync<T>(new[] { _formatter });
            }
            else throw new GoogleApiException($"The API returned an error with status code: {message.StatusCode}. (Request URI: {message.RequestMessage.RequestUri.PathAndQuery})");
        }
    }
}
