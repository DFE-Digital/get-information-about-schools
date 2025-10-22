using Edubase.Services.Approvals;
using Edubase.Services.Approvals.Models;
using Edubase.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        [HttpGet("change-requests")]
        public async Task<IActionResult> GetAsync(int skip, int take, string sortBy)
        {
            try
            {
                var result = await _approvalService.GetAsync(skip, take, sortBy, User);
                return Ok(result);
            }
            catch (PermissionDeniedException)
            {
                return StatusCode(403); // Forbidden
            }
        }

        [HttpPost("change-request")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActionAsync(PendingChangeRequestAction model)
        {
            var result = await _approvalService.ActionAsync(model, User);
            return result.Success
                ? NoContent()
                : BadRequest(result.Errors);
        }
    }
}
