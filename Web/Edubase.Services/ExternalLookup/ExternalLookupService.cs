using System.Threading.Tasks;

namespace Edubase.Services.ExternalLookup
{
    public class ExternalLookupService : IExternalLookupService
    {
        private readonly ICSCPService _cscpService;
        private readonly IFBService _fbService;

        public ExternalLookupService(ICSCPService cscpService, IFBService fbService)
        {
            _cscpService = cscpService;
            _fbService = fbService;
        }

        public async Task<bool> CscpCheckExists(int? urn, string name, bool mat = false)
        {
            return await _cscpService.CheckExists(urn, name, mat);
        }

        public string CscpURL(int? urn, string name, bool mat = false)
        {
            return _cscpService.PublicURL(urn, name, mat);
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
