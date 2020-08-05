using System.Threading.Tasks;

namespace Edubase.Services.ExternalLookup
{
    public interface ICSCPService
    {
        Task<bool> CheckExists(int? urn, string name);
        string SchoolURL(int? urn, string name);
    }
}
