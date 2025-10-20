using Edubase.Services.Approvals;
using Edubase.Services.Approvals.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Edubase.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    public class ApprovalsApiController : ControllerBase
    {
        private readonly IApprovalService _approvalService;

        public ApprovalsApiController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [Route("api/approvals/change-requests"), HttpGet]
        public async Task<IActionResult> GetAsync(int skip, int take, string sortBy)
        {
            try
            {
                var result = await _approvalService.GetAsync(skip, take, sortBy, User);
                return Ok(result);
            }
            catch (PermissionDeniedException)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
        }

        [Route("api/approvals/change-request"), HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActionAsync(PendingChangeRequestAction model)
        {
            var result = await _approvalService.ActionAsync(model, User);
            return result.Success
                ? ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent))
                : (IActionResult) Content(HttpStatusCode.BadRequest, result.Errors);
        }
    }
}
