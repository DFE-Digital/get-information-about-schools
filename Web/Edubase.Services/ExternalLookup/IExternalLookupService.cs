using System.Threading.Tasks;

namespace Edubase.Services.ExternalLookup
{
    public interface IExternalLookupService
    {
        Task<bool> CscpCheckExists(int? urn, string name, bool mat = false);
        string CscpURL(int? urn, string name, bool mat = false);
        Task<bool> SfbCheckExists(int? urn, string companiesHouse);
        string SfbURL(int? urn, string companiesHouse);
    }
}
