using System.Threading;
using System.Threading.Tasks;
using Edubase.Services.Geo;

namespace Edubase.Services.IntegrationEndPoints.AzureMaps
{
    public interface IAzureMapsService
    {
        Task<PlaceDto[]> SearchAsync(string text, bool isTypeahead, CancellationToken cancellationToken = default);
    }
}
