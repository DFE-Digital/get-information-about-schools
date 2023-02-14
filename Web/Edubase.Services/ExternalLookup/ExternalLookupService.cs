using System.Threading.Tasks;

namespace Edubase.Services.ExternalLookup
{
    public class ExternalLookupService : IExternalLookupService
    {
        private readonly IFSCPDService _fscpdService;
        private readonly IFBService _fbService;

        public ExternalLookupService(IFSCPDService fscpdService, IFBService fbService)
        {
            _fscpdService = fscpdService;
            _fbService = fbService;
        }

        public async Task<bool> FscpdCheckExists(int? urn, string name, bool mat = false)
        {
            return await _fscpdService.CheckExists(urn, name, mat);
        }

        public string FscpdURL(int? urn, string name, bool mat = false)
        {
            return _fscpdService.PublicURL(urn, name, mat);
        }

        public async Task<bool> SfbCheckExists(int? lookupId, FbType lookupType)
        {
            return await _fbService.CheckExists(lookupId, lookupType);
        }

        public string SfbURL(int? lookupId, FbType lookupType)
        {
            return _fbService.PublicURL(lookupId, lookupType);
        }
    }
}
