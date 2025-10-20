using System.Linq;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Services;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Exceptions;
using Edubase.Services.Groups;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Exceptions;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [RouteArea("Governors")]
    [RoutePrefix("Governance")]
    public class GovernanceController : Controller
    {
        private const string EstabEditGovernanceMode = "~/Establishment/Edit/{establishmentUrn:int}/GovernanceMode";

        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private readonly IGroupsWriteService _groupWriteService;
        private readonly LayoutHelper _layoutHelper;

        public GovernanceController(IEstablishmentReadService establishmentReadService, IEstablishmentWriteService establishmentWriteService, IGroupsWriteService groupWriteService, LayoutHelper layoutHelper)
        {
            _establishmentReadService = establishmentReadService;
            _establishmentWriteService = establishmentWriteService;
            _groupWriteService = groupWriteService;
            _layoutHelper = layoutHelper;
        }

        [HttpPost, EdubaseAuthorize, Route("ConfirmEstablishment/{urn:int}", Name = "EstablishmentGovernanceConfirmUpToDate"), ValidateAntiForgeryToken]
        public async Task<ActionResult> EstablishmentGovernanceConfirmUpToDateAsync(int urn, bool showBanner = false)
        {
            await _establishmentWriteService.ConfirmGovernanceAsync(urn, User);
            var url = $"{Url.RouteUrl("EstabDetails", new { id = urn, saved = showBanner, confirmed = "Governance" })}#school-governance";
            return Redirect(url);
        }

        [HttpPost, EdubaseAuthorize, Route("Governance/ConfirmGroup/{uid:int}", Name = "GroupGovernanceConfirmUpToDate"), ValidateAntiForgeryToken]
        public async Task<ActionResult> GroupGovernanceConfirmUpToDateAsync(int uid)
        {
            await _groupWriteService.ConfirmGovernanceAsync(uid, User);
            return RedirectToRoute("GroupDetails", new { id = uid });
        }

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="establishmentUrn"></param>
        /// <returns></returns>
        [Route(EstabEditGovernanceMode, Name = "EstabEditGovernanceMode"), HttpGet, EdubaseAuthorize]
        public async Task<ActionResult> EditGovernanceMode(int? establishmentUrn, bool failed = false)
        {
            establishmentUrn.AssertIsNotEmpty(nameof(establishmentUrn));

            if (failed)
            {
                ModelState.AddModelError("", "Unable to update Governance");
            }

            var viewModel = new EditGovernanceModeViewModel
            {
                Urn = establishmentUrn.Value,
                PermissibleGovernanceModes = (await _establishmentReadService.GetPermissibleLocalGovernorsAsync(establishmentUrn.Value, User)).Select(x => (eGovernanceMode)x.Id).ToArray()
            };
            await _layoutHelper.PopulateLayoutProperties(viewModel, establishmentUrn, null, User, x => viewModel.GovernanceMode = x.GovernanceMode ?? eGovernanceMode.LocalGovernors);
            return View(viewModel);

        }

        /// <summary>
        /// Saves the governance mode
        /// </summary>
        /// <returns></returns>
        [Route(EstabEditGovernanceMode), HttpPost, EdubaseAuthorize, ValidateAntiForgeryToken]
        public async Task<ActionResult> EditGovernanceMode(EditGovernanceModeViewModel viewModel)
        {
            try
            {
                await _establishmentWriteService.UpdateGovernanceModeAsync(viewModel.Urn.Value, viewModel.GovernanceMode.Value, User);
                var url = Url.RouteUrl("EstabDetails", new {id = viewModel.Urn, saved = true });
                return Redirect($"{url}#school-governance");
            }
            catch (EduSecurityException) // for some reason the API responds with a 403 for this one, even though it's nothing to do with authentication/authorization.
            {
                return RedirectToRoute("EstabEditGovernanceMode", new { establishmentUrn = viewModel.Urn, failed = true });
            }
        }
    }
}
