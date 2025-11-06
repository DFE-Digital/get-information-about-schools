using System.Threading.Tasks;
using Edubase.Services.IntegrationEndPoints.AzureMaps;
using Edubase.Services.IntegrationEndPoints.OSPlaces;

namespace Edubase.Services.Geo
{
    public class PlacesLookupService : IPlacesLookupService
    {
        private readonly IAzureMapsService _azureMapsService;
        private readonly IOSPlacesApiService _osPlacesApiService;

        public PlacesLookupService(
            IAzureMapsService azureMapsService,
            IOSPlacesApiService osPlacesApiService)
        {
            _azureMapsService = azureMapsService;
            _osPlacesApiService = osPlacesApiService;
        }

        public async Task<PlaceDto[]> SearchAsync(string text, bool isTypeahead)
        {
            PlaceDto[] retVal = [.. await _azureMapsService.SearchAsync(text, isTypeahead)];

            if (retVal.Length != 0)
            {
                return retVal;
            }

            if (!isTypeahead)
            {
                retVal = [.. await _osPlacesApiService.SearchAsync(text)];
            }

            return retVal;
        }
    }
}
