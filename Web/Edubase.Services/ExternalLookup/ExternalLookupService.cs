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

        public async Task<bool> CscpCheckExists(int? urn, string name)
        {
            return await _cscpService.CheckExists(urn, name);
        }

        public string CscpSchoolURL(int? urn, string name)
        {
            return _cscpService.SchoolURL(urn, name);
        }

        public async Task<bool> SfbCheckExists(int? urn)
        {
            return await _fbService.CheckExists(urn);
        }

        public string SfbSchoolURL(int? urn)
        {
            return _fbService.SchoolURL(urn);
        }
    }
}
