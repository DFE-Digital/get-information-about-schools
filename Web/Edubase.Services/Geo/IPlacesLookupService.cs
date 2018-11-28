using System.Threading.Tasks;

namespace Edubase.Services.Geo
{
    public interface IPlacesLookupService
    {
        Task<PlaceDto[]> SearchAsync(string text, bool isTypeahead);
    }
}
