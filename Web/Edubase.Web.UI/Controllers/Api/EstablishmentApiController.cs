using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using System.Threading.Tasks;
using System.Web.Http;
using Edubase.Web.UI.Filters;

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
            else return Ok(retVal);
        }
    }
}
