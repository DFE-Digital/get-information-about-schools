using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Security;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Controllers.Api
{
    [Route("api/amalgamate-merge")]
    [ApiController] // Optional: enables automatic model validation and binding behavior
    public class AmalgamateMergeApiController : ControllerBase
    {
        private readonly IEstablishmentWriteService _establishmentWriteService;

        public AmalgamateMergeApiController(IEstablishmentWriteService establishmentWriteService)
        {
            _establishmentWriteService = establishmentWriteService;
        }

        [HttpPost]
        [HttpAuthorizeRoles(EdubaseRoles.AP_AOS, EdubaseRoles.ROLE_BACKOFFICE, EdubaseRoles.EFADO, EdubaseRoles.SOU, EdubaseRoles.IEBT)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessRequestAsync(AmalgamateMergeRequest payload)
        {
            var result = await _establishmentWriteService.AmalgamateOrMergeAsync(payload, User);
            return result.HasErrors ? BadRequest(result) : Ok(result);
        }
    }
}
