using System.Threading.Tasks;

namespace Edubase.Services.ExternalLookup
{
    public interface IFBService
    {
        Task<bool> CheckExists(int? lookupId, FbType lookupType);
        string PublicURL(int? lookupId, FbType lookupType);
    }
}
