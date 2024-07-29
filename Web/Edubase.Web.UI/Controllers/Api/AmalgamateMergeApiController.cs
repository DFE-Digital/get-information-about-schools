using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Security;
using Edubase.Web.UI.Helpers;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    public class AmalgamateMergeApiController : ApiController
    {
        private readonly IEstablishmentWriteService _establishmentWriteService;

        public AmalgamateMergeApiController(IEstablishmentWriteService establishmentWriteService)
        {
            _establishmentWriteService = establishmentWriteService;
        }

        [System.Web.Http.Route("api/amalgamate-merge"), System.Web.Http.HttpPost, HttpAuthorizeRoles(EdubaseRoles.AP_AOS, EdubaseRoles.ROLE_BACKOFFICE, EdubaseRoles.EFADO, EdubaseRoles.SOU, EdubaseRoles.IEBT)]
        [ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> ProcessRequestAsync(AmalgamateMergeRequest payload)
        {
            var result = await _establishmentWriteService.AmalgamateOrMergeAsync(payload, User);
            return !result.HasErrors ? Ok(result) : (IHttpActionResult) Content(HttpStatusCode.BadRequest, result);
        }
    }
}
