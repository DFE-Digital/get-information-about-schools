using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Security;
using Edubase.Web.UI.Helpers;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    public class AmalgamateMergeApiController : ControllerBase
    {
        private readonly IEstablishmentWriteService _establishmentWriteService;

        public AmalgamateMergeApiController(IEstablishmentWriteService establishmentWriteService)
        {
            _establishmentWriteService = establishmentWriteService;
        }

        [Route("api/amalgamate-merge"), HttpPost, HttpAuthorizeRoles(EdubaseRoles.AP_AOS, EdubaseRoles.ROLE_BACKOFFICE, EdubaseRoles.EFADO, EdubaseRoles.SOU, EdubaseRoles.IEBT)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessRequestAsync(AmalgamateMergeRequest payload)
        {
            var result = await _establishmentWriteService.AmalgamateOrMergeAsync(payload, User);
            return !result.HasErrors ? Ok(result) : (IActionResult) Content(HttpStatusCode.BadRequest, result);
        }
    }
}
