using Edubase.Common.Spatial;
using Edubase.Services.IntegrationEndPoints.OSPlaces.Models;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.OSPlaces
{
    public interface IOSPlacesApiService
    {
        LatLon GetCoordinate(string placeId);
        Task<OSPlacesItemDto[]> SearchAsync(string text);
    }
}