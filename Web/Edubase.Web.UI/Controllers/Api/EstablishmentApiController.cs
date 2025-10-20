using System.Threading.Tasks;
using Edubase.Services.Establishments;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    [Authorize]
    public class EstablishmentApiController : ControllerBase
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly ICachedLookupService _lookupService;
        
        public EstablishmentApiController(IEstablishmentReadService establishmentReadService, ICachedLookupService lookupService)
        {
            _establishmentReadService = establishmentReadService;
            _lookupService = lookupService;
        }

        [Route("api/establishment/{urn:int}"), HttpGet, EdubaseAuthorize]
        public async Task<IActionResult> Get(int urn)
        {
            var retVal = await _establishmentReadService.GetAsync(urn, User);
            if (retVal.ReturnValue == null) return NotFound();
            else
            {
                var apiResult = retVal.GetResult();
                var minRes = new EstbalishmentApiResponse
                {
                    Status = retVal.Status.ToString(),
                    ReturnValue = new EstablishmentApiPayload()
                    {
                        Name = apiResult.Name,
                        Urn = apiResult.Urn,
                        TypeName = await _lookupService.GetNameAsync(() => apiResult.TypeId)
                    }
                };
                return Ok(minRes);
            }
        }

        public class EstablishmentApiPayload
        {
            public string Name { get; set; }
            public int? Urn { get; set; }
            public string TypeName { get; set; }
        }
        public class EstbalishmentApiResponse
        {
            public string Status { get; set; }
            public EstablishmentApiPayload ReturnValue { get; set; }
        }
    }
}
