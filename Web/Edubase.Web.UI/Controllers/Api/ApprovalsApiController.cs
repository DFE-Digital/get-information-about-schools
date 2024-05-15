using Edubase.Services.Approvals;
using Edubase.Services.Approvals.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

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
        public async Task<PendingApprovalsResult> GetAsync(int skip, int take, string sortBy)
            => await _approvalService.GetAsync(skip, take, sortBy, User);

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
