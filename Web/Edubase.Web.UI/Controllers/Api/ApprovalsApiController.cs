using Edubase.Services.Approvals;
using Edubase.Services.Approvals.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using Edubase.Services.Exceptions;

namespace Edubase.Web.UI.Controllers.Api
{
    public class ApprovalsApiController : ApiController
    {
        private readonly IApprovalService _approvalService;

        public ApprovalsApiController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [System.Web.Http.Route("api/approvals/change-requests"), System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> GetAsync(int skip, int take, string sortBy)
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

        [System.Web.Http.Route("api/approvals/change-request"), System.Web.Http.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> ActionAsync(PendingChangeRequestAction model)
        {
            var result = await _approvalService.ActionAsync(model, User);
            return result.Success
                ? ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent))
                : (IHttpActionResult) Content(HttpStatusCode.BadRequest, result.Errors);
        }
    }
}
