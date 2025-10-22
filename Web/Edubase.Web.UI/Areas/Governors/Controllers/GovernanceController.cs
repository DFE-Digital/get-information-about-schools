using System.Linq;
using System.Threading.Tasks;
using Edubase.Services;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Exceptions;
using Edubase.Services.Groups;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [Route("Governors/Governance")]
    public class GovernanceController : Controller
    {
        private const string EstabEditGovernanceModeRoute = "Establishment/Edit/{establishmentUrn:int}/GovernanceMode";

        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private readonly IGroupsWriteService _groupWriteService;
        private readonly LayoutHelper _layoutHelper;

        public GovernanceController(
            IEstablishmentReadService establishmentReadService,
            IEstablishmentWriteService establishmentWriteService,
            IGroupsWriteService groupWriteService,
            LayoutHelper layoutHelper)
        {
            _establishmentReadService = establishmentReadService;
            _establishmentWriteService = establishmentWriteService;
            _groupWriteService = groupWriteService;
            _layoutHelper = layoutHelper;
        }

        [HttpPost("ConfirmEstablishment/{urn:int}", Name = "EstablishmentGovernanceConfirmUpToDate")]
        [Authorize(Policy = "EdubasePolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EstablishmentGovernanceConfirmUpToDateAsync(int urn, bool showBanner = false)
        {
            await _establishmentWriteService.ConfirmGovernanceAsync(urn, User);
            var url = $"{Url.RouteUrl("EstabDetails", new { id = urn, saved = showBanner, confirmed = "Governance" })}#school-governance";
            return Redirect(url);
        }

        [HttpPost("Governance/ConfirmGroup/{uid:int}", Name = "GroupGovernanceConfirmUpToDate")]
        [Authorize(Policy = "EdubasePolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupGovernanceConfirmUpToDateAsync(int uid)
        {
            await _groupWriteService.ConfirmGovernanceAsync(uid, User);
            return RedirectToRoute("GroupDetails", new { id = uid });
        }

        [HttpGet("Establishment/Edit/{establishmentUrn:int}/GovernanceMode", Name = "EstabEditGovernanceMode")]
        [Authorize(Policy = "EdubasePolicy")]
        public async Task<IActionResult> EditGovernanceMode(int? establishmentUrn, bool failed = false)
        {
            establishmentUrn.AssertIsNotEmpty(nameof(establishmentUrn));

            if (failed)
            {
                ModelState.AddModelError("", "Unable to update Governance");
            }

            var viewModel = new EditGovernanceModeViewModel
            {
                Urn = establishmentUrn.Value,
                PermissibleGovernanceModes = (await _establishmentReadService
                    .GetPermissibleLocalGovernorsAsync(establishmentUrn.Value, User))
                    .Select(x => (eGovernanceMode) x.Id)
                    .ToArray()
            };

            await _layoutHelper.PopulateLayoutProperties(
                viewModel, establishmentUrn, null, User,
                x => viewModel.GovernanceMode = x.GovernanceMode ?? eGovernanceMode.LocalGovernors);

            return View(viewModel);
        }

        [HttpPost("Establishment/Edit/{establishmentUrn:int}/GovernanceMode")]
        [Authorize(Policy = "EdubasePolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGovernanceMode(EditGovernanceModeViewModel viewModel)
        {
            try
            {
                await _establishmentWriteService.UpdateGovernanceModeAsync(
                    viewModel.Urn.Value, viewModel.GovernanceMode.Value, User);

                var url = Url.RouteUrl("EstabDetails", new { id = viewModel.Urn, saved = true });
                return Redirect($"{url}#school-governance");
            }
            catch (EduSecurityException)
            {
                return RedirectToRoute("EstabEditGovernanceMode", new
                {
                    establishmentUrn = viewModel.Urn,
                    failed = true
                });
            }
        }
    }
}
