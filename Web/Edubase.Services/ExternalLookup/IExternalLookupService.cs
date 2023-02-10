using System.Threading.Tasks;

namespace Edubase.Services.ExternalLookup
{
    public interface IExternalLookupService
    {
        Task<bool> FscpCheckExists(int? urn, string name, bool mat = false);
        string FscpURL(int? urn, string name, bool mat = false);
        Task<bool> SfbCheckExists(int? lookupId, FbType lookupType);
        string SfbURL(int? lookupId, FbType lookupType);
    }
}
