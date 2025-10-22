using System.Linq;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/establishment")]
    public class EstablishmentApiController : ControllerBase
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly ICachedLookupService _lookupService;

        public EstablishmentApiController(
            IEstablishmentReadService establishmentReadService,
            ICachedLookupService lookupService)
        {
            _establishmentReadService = establishmentReadService;
            _lookupService = lookupService;
        }

        [HttpGet("{urn:int}")]
        [Authorize(Policy = "EdubasePolicy")]
        public async Task<IActionResult> Get(int urn)
        {
            var retVal = await _establishmentReadService.GetAsync(urn, User);
            if (retVal.ReturnValue == null)
                return NotFound();

            var apiResult = retVal.GetResult();
            var minRes = new EstablishmentApiResponse
            {
                Status = retVal.Status.ToString(),
                ReturnValue = new EstablishmentApiPayload
                {
                    Name = apiResult.Name,
                    Urn = apiResult.Urn,
                    TypeName = await _lookupService.GetNameAsync(() => apiResult.TypeId)
                }
            };

            return Ok(minRes);
        }

        public class EstablishmentApiPayload
        {
            public string Name { get; set; }
            public int? Urn { get; set; }
            public string TypeName { get; set; }
        }

        public class EstablishmentApiResponse
        {
            public string Status { get; set; }
            public EstablishmentApiPayload ReturnValue { get; set; }
        }
    }
}
