using System.Threading.Tasks;
using Edubase.Services.Approvals;
using Edubase.Services.Approvals.Models;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    [ApiController]
    [Route("api/approvals")]
    public class ApprovalsApiController : ControllerBase
    {
        private readonly IApprovalService _approvalService;

        public ApprovalsApiController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        /// <summary>
        /// GET api/approvals/change-requests?skip=0&take=10&sortBy=name
        /// </summary>
        [HttpGet("change-requests")]
        public async Task<ActionResult<PendingApprovalsResult>> GetAsync([FromQuery] int skip, [FromQuery] int take, [FromQuery] string sortBy)
        {
            var result = await _approvalService.GetAsync(skip, take, sortBy, User);
            return Ok(result);
        }

        /// <summary>
        /// POST api/approvals/change-request
        /// </summary>
        [HttpPost("change-request")]
        [IgnoreAntiforgeryToken] // Use this for API endpoints unless you're posting from a browser form
        public async Task<IActionResult> ActionAsync([FromBody] PendingChangeRequestAction model)
        {
            var result = await _approvalService.ActionAsync(model, User);
            return result.Success
                ? NoContent()
                : BadRequest(result.Errors);
        }
    }
}
