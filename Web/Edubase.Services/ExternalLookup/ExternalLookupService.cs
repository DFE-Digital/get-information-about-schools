using System.Threading.Tasks;

namespace Edubase.Services.ExternalLookup
{
    public class ExternalLookupService : IExternalLookupService
    {
        private readonly IFSCPService _fscpService;
        private readonly IFBService _fbService;

        public ExternalLookupService(IFSCPService fscpService, IFBService fbService)
        {
            _fscpService = fscpService;
            _fbService = fbService;
        }

        public async Task<bool> FscpCheckExists(int? urn, string name, bool mat = false)
        {
            return await _fscpService.CheckExists(urn, name, mat);
        }

        public string FscpURL(int? urn, string name, bool mat = false)
        {
            return _fscpService.PublicURL(urn, name, mat);
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
