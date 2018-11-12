using Edubase.Services.Geo;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.OSPlaces
{
    public interface IOSPlacesApiService
    {
        Task<PlaceDto[]> SearchAsync(string text);
    }
}
