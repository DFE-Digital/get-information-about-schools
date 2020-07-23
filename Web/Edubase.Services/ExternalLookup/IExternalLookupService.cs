using System.Threading.Tasks;

namespace Edubase.Services.ExternalLookup
{
    public interface IExternalLookupService
    {
        Task<bool> CscpCheckExists(int? urn, string name);
        string CscpSchoolURL(int? urn, string name);
        Task<bool> SfbCheckExists(int? urn);
        string SfbSchoolURL(int? urn);
    }
}
