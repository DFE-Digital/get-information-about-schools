using Edubase.Services.Approvals;
using Edubase.Services.Approvals.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Edubase.Web.UI.Controllers.Api
{
    public class ApprovalsApiController : ApiController
    {
        private readonly IApprovalService _approvalService;

        public ApprovalsApiController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [Route("api/approvals/change-requests"), HttpGet]
        public async Task<PendingApprovalsResult> GetAsync(int skip, int take, string sortBy)
            => await _approvalService.GetAsync(skip, take, sortBy, User);

        [Route("api/approvals/change-request"), HttpPost]
        public async Task<IHttpActionResult> ActionAsync(PendingChangeRequestAction model)
        {
            var result = await _approvalService.ActionAsync(model, User);
            if (result.Success)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, result.Errors);
            }
        }
    }
}
