using System.Threading.Tasks;

namespace Edubase.Services.ExternalLookup
{
    public interface ICSCPService
    {
        Task<bool> CheckExists(int? urn, string name, bool mat = false);
        string PublicURL(int? urn, string name, bool mat = false);
    }
}
