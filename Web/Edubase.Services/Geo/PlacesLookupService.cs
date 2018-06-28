using Edubase.Common;
using Edubase.Common.Spatial;
using Edubase.Services.IntegrationEndPoints.Google;
using Edubase.Services.IntegrationEndPoints.OSPlaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Services.Geo
{
    public class PlacesLookupService : IPlacesLookupService
    {
        private readonly IGooglePlacesService _googlePlacesService;
        private readonly IOSPlacesApiService _osPlacesApiService;

        private const string GooglePlacesIdPrefixer = "gp-";
        private const string OSPlacesIdPrefixer = "os-";

        public PlacesLookupService(IGooglePlacesService googlePlacesService, IOSPlacesApiService osPlacesApiService)
        {
            _googlePlacesService = googlePlacesService;
            _osPlacesApiService = osPlacesApiService;
        }

        public async Task<PlaceDto[]> SearchAsync(string text)
        {
            var retVal = (await _googlePlacesService.SearchAsync(text)).Select(x => new PlaceDto(GooglePlacesIdPrefixer + x.Id, x.Name)).ToArray();
            if (retVal.Any()) return retVal;

            retVal = (await _osPlacesApiService.SearchAsync(text)).Select(x => new PlaceDto(OSPlacesIdPrefixer + x.Id, x.Name)).ToArray();

            return retVal;
        }

        public async Task<LatLon> GetCoordinateAsync(string id)
        {
            if (id.Clean() == null) throw new ArgumentNullException(nameof(id));
            else if (id.Length <= 3) throw new InvalidOperationException("The id isn't long enough to be deemed valid.");
            else if (id.StartsWith(GooglePlacesIdPrefixer)) return await _googlePlacesService.GetCoordinateAsync(id.Remove(0, GooglePlacesIdPrefixer.Length));
            else if (id.StartsWith(OSPlacesIdPrefixer)) return _osPlacesApiService.GetCoordinate(id.Remove(0, OSPlacesIdPrefixer.Length));
            else throw new NotSupportedException($"Place ID {id} is not supported.");
        }
    }
}
