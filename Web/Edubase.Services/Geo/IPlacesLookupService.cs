using System.Threading.Tasks;
using Edubase.Common.Spatial;

namespace Edubase.Services.Geo
{
    public interface IPlacesLookupService
    {
        Task<LatLon> GetCoordinateAsync(string id);
        Task<PlaceDto[]> SearchAsync(string text);
    }
}