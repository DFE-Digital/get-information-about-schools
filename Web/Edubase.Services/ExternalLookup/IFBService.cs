using System.Threading.Tasks;

namespace Edubase.Services.ExternalLookup
{
    public interface IFBService
    {
        Task<bool> CheckExists(int? urn);
        string SchoolURL(int? urn);
    }
}
