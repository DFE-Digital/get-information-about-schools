using System.Threading.Tasks;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    [ApiController]
    [Route("api/amalgamate-merge")]
    [Authorize(Roles = "AP_AOS,ROLE_BACKOFFICE,EFADO,SOU,IEBT")]
    public class AmalgamateMergeApiController : ControllerBase
    {
        private readonly IEstablishmentWriteService _establishmentWriteService;

        public AmalgamateMergeApiController(IEstablishmentWriteService establishmentWriteService)
        {
            _establishmentWriteService = establishmentWriteService;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken] // Use this if you're calling from a non-browser client like Postman or JS fetch
        public async Task<IActionResult> ProcessRequestAsync([FromBody] AmalgamateMergeRequest payload)
        {
            var result = await _establishmentWriteService.AmalgamateOrMergeAsync(payload, User);
            return result.HasErrors ? BadRequest(result) : Ok(result);
        }
    }
}
