using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Web.UI.Filters;
using System.Web.Mvc;
using Edubase.Services.Approvals;
using Edubase.Services.Approvals.Models;
using Edubase.Services.Core;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models.Tools;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Approvals"), Route("{action=index}"), MvcAuthorizeRoles(AuthorizedRoles.CanApprove)]
    public class ApprovalsController : Controller
    {
        private readonly IApprovalService _approvalService;
        private const string DefaultSort = "effectiveDateUtc-asc";

        public ApprovalsController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [HttpGet, EdubaseAuthorize, Route(Name = "PendingApprovals")]
        public async Task<ActionResult> Index(string sortBy, int skip = 0)
        {
            var vm = await PopulateChangesModel(sortBy, skip);

            return View("Index", vm);
        }

        [HttpPost, EdubaseAuthorize, Route(Name = "PendingApprovalsPost"), ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(PendingChangeRequestAction viewModel)
        {
            if (viewModel.Ids == null)
            {
                ModelState.AddModelError("changes-table-no-js", string.Concat("Select some changes to ", viewModel.Action.ToString().ToLower(), "."));
                var vm = await PopulateChangesModel(DefaultSort);
                return View("Index", vm);
            }

            if (viewModel.Action == ePendingChangeRequestAction.Reject)
            {

                if (string.IsNullOrEmpty(viewModel.RejectionReason))
                {
                    if (viewModel.ActionSpecifier == "setRejection")
                    {
                        ModelState.AddModelError("RejectionReason", "Enter a reason for the rejection");
                    }
                    return View("ExplainRejections", viewModel);
                }

                viewModel.ActionSpecifier = "reject";

                var result = await _approvalService.ActionAsync(viewModel, User);

                if (result.HasErrors)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("RejectionReason", error.Message);
                    }

                    return View("ExplainRejections", viewModel);
                }

                TempData["ShowSuccess"] = "Items rejected. The editor has been notified by email";
                return RedirectToAction("Index");
            }
            else
            {
                viewModel.ActionSpecifier = "approve";
                var result = await _approvalService.ActionAsync(viewModel, User);
                if (result.HasErrors)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Message);
                    }
                }
                else
                {
                    TempData["ShowSuccess"] = "Items approved. The editor has been notified by email";
                }
                var vm = await PopulateChangesModel(DefaultSort);
                return View("Index", vm);
            }
        }


        private async Task<ApprovalsViewModel> PopulateChangesModel(string sortBy, int skip = 0)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = DefaultSort;
            }
            var res =  await _approvalService.GetAsync(skip, 100, sortBy, User);
            var viewModel = new ApprovalsViewModel()
            {
                Skip = skip, SortBy = sortBy, Items = res.Items, Count = res.Count, SortedAscending = sortBy.Contains("-asc")
            };
            viewModel.ApprovalItems = new PaginatedResult<PendingApprovalItem>(viewModel.Skip, viewModel.Take, res.Count, res.Items);
            return viewModel;
        }
    }
}
