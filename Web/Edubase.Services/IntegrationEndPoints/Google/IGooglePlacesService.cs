using System.Threading.Tasks;
using Edubase.Common.Spatial;
using Edubase.Services.IntegrationEndPoints.Google.Models;

namespace Edubase.Services.IntegrationEndPoints.Google
{
    public interface IGooglePlacesService
    {
        Task<LatLon> GetCoordinateAsync(string placeId);
        Task<AutocompleteItemDto[]> SearchAsync(string text);
    }
}