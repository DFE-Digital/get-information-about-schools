using System.Dynamic;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using System.Threading.Tasks;
using System.Web.Http;
using Edubase.Web.UI.Filters;
using Newtonsoft.Json.Linq;

namespace Edubase.Web.UI.Controllers.Api
{
    [Authorize]
    public class EstablishmentApiController : ApiController
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        
        public EstablishmentApiController(IEstablishmentReadService establishmentReadService)
        {
            _establishmentReadService = establishmentReadService;
        }

        [Route("api/establishment/{urn:int}"), HttpGet, EdubaseAuthorize]
        public async Task<IHttpActionResult> Get(int urn)
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
                        Urn = apiResult.Urn
                    }
                };
                return Ok(minRes);
            }
        }

        public class EstablishmentApiPayload
        {
            public string Name { get; set; }
            public int? Urn { get; set; }
        }
        public class EstbalishmentApiResponse
        {
            public string Status { get; set; }
            public EstablishmentApiPayload ReturnValue { get; set; }
        }
    }
}
