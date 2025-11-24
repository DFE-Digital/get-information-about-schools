using System.Threading.Tasks;

namespace Edubase.Services.ExternalLookup
{
    public interface IOfstedService
    {
        Task<bool> CheckExists(int? urn);
        string PublicURL(int? urn);
    }
}
