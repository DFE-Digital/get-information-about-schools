using System.Threading.Tasks;
using Edubase.Services.Approvals;
using Edubase.Services.Approvals.Models;
using Edubase.Services.Core;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [ApiController]
    [Route("Approvals")]
    [MvcAuthorizeRoles(AuthorizedRoles.CanApprove)]
    public class ApprovalsController : Controller
    {
        private readonly IApprovalService _approvalService;
        private const string DefaultSort = "effectiveDateUtc-asc";

        public ApprovalsController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [HttpGet("")]
        [Authorize(Policy = "EdubasePolicy")]
        public async Task<IActionResult> Index(string sortBy, int skip = 0)
        {
            var vm = await PopulateChangesModel(sortBy, skip);
            return View("Index", vm);
        }

        [HttpPost("")]
        [Authorize(Policy = "EdubasePolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PendingChangeRequestAction viewModel)
        {
            if (viewModel.Ids == null)
            {
                ModelState.AddModelError("changes-table-no-js", $"Select some changes to {viewModel.Action.ToString().ToLower()}.");
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
            sortBy ??= DefaultSort;
            var res = await _approvalService.GetAsync(skip, 100, sortBy, User);

            var viewModel = new ApprovalsViewModel
            {
                Skip = skip,
                SortBy = sortBy,
                Items = res.Items,
                Count = res.Count,
                SortedAscending = sortBy.Contains("-asc"),
                ApprovalItems = new PaginatedResult<PendingApprovalItem>(skip, 100, res.Count, res.Items)
            };

            return viewModel;
        }
    }
}
