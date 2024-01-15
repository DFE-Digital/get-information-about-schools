using Edubase.Services.IntegrationEndPoints.AzureMaps;
using Edubase.Services.IntegrationEndPoints.OSPlaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            var retVal = (await _azureMapsService.SearchAsync(text, isTypeahead)).ToArray();

            if (retVal.Any())
            {
                return retVal;
            }

            if (!isTypeahead)
            {
                retVal = (await _osPlacesApiService.SearchAsync(text)).ToArray();
            }

            return retVal;
        }
    }
}
