using System.Linq;
using System.Threading.Tasks;
using Edubase.Services.Enums;
using Edubase.Services.Exceptions;
using Edubase.Services.Governors;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Validation;
using Edubase.Web.UI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [RouteArea("Governors"), RoutePrefix("SharedGovernor")]
    public class SharedGovernorController : Controller
    {
        private const string EstabSelectSharedGovernor = "~/Establishment/Edit/{establishmentUrn:int}/Governance/SelectSharedGovernor";
        private const string EstabEditSharedGovernor = "~/Establishment/Edit/{establishmentUrn:int}/Governance/EditSharedGovernor";

        private readonly ICachedLookupService _cachedLookupService;
        private readonly IGovernorsReadService _governorsReadService;
        private readonly IGovernorsWriteService _governorsWriteService;
        private readonly LayoutHelper _layoutHelper;

        public SharedGovernorController(
            ICachedLookupService cachedLookupService,
            IGovernorsReadService governorsReadService,
            IGovernorsWriteService governorsWriteService,
            LayoutHelper layoutHelper)
        {
            _cachedLookupService = cachedLookupService;
            _governorsReadService = governorsReadService;
            _governorsWriteService = governorsWriteService;
            _layoutHelper = layoutHelper;
        }

        [HttpGet, Route(EstabSelectSharedGovernor, Name = "SelectSharedGovernor"), EdubaseAuthorize]
        public async Task<ActionResult> SelectSharedGovernor(int establishmentUrn, eLookupGovernorRole role)
        {
            var roleName = (await _cachedLookupService.GovernorRolesGetAllAsync()).Single(x => x.Id == (int)role).Name;
            var governors = (await _governorsReadService.GetSharedGovernorsAsync(establishmentUrn, User)).Where(g => RoleEquivalence.GetEquivalentRole(role).Contains((eLookupGovernorRole)g.RoleId)).ToList();

            var sharedGovernors = governors.Select(async g => await SharedGovernorViewModel.MapFromGovernor(g, establishmentUrn, _cachedLookupService));

            var viewModel = new SelectSharedGovernorViewModel
            {
                Governors = (await Task.WhenAll(sharedGovernors)).ToList(),
                GovernorType = roleName.ToLowerInvariant(),

            };

            await _layoutHelper.PopulateLayoutProperties(viewModel, establishmentUrn, null, User);

            return View(viewModel);
        }

        [HttpPost, Route(EstabSelectSharedGovernor), EdubaseAuthorize, ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectSharedGovernor(SelectSharedGovernorViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var governor in model.Governors.Where(g => (g.Selected && !g.PreExisting) || string.Equals(g.Id.ToString(), model.SelectedGovernorId)))
                {
                    var response = await _governorsWriteService.AddSharedGovernorAppointmentAsync(governor.Id, model.Urn.Value, governor.AppointmentStartDate.ToDateTime().Value, governor.AppointmentEndDate.ToDateTime(), User);
                    if (!response.Success)
                    {
                        response.ApplyToModelState(ControllerContext);
                    }
                }

                if (ModelState.IsValid)
                {
                    var url = $"{Url.RouteUrl("EstabDetails", new { id = model.Urn, saved = true })}#school-governance";

                    return Redirect(url);
                }
            }

            var governors = (await Task.WhenAll(
                (await _governorsReadService.GetSharedGovernorsAsync(model.Urn.Value, User))
                .Where(g => RoleEquivalence.GetEquivalentRole(model.Role).Contains((eLookupGovernorRole)g.RoleId))
                .Select(g => SharedGovernorViewModel.MapFromGovernor(g, model.Urn.Value, _cachedLookupService)))).ToList();

            foreach (var previousGovernor in model.Governors)
            {
                var newGovernor = governors.Single(g => g.Id == previousGovernor.Id);
                if (!newGovernor.PreExisting)
                {
                    newGovernor.Selected = previousGovernor.Selected;
                }

                // Retain the user-submitted selection (even if it is invalid):
                if(newGovernor.Id.ToString() == model.SelectedGovernorId)
                {
                    newGovernor.Selected = true;
                }
            }

            model.Governors = governors;
            await _layoutHelper.PopulateLayoutProperties(model, model.Urn.Value, null, User);

            return View(model);
        }

        [HttpGet, Route(EstabEditSharedGovernor, Name = "EditSharedGovernor"), EdubaseAuthorize]
        public async Task<ActionResult> EditSharedGovernor(int establishmentUrn, int governorId)
        {
            var governor = await _governorsReadService.GetGovernorAsync(governorId, User);
            var roleName = (await _cachedLookupService.GovernorRolesGetAllAsync()).Single(x => x.Id == governor.RoleId.Value).Name;

            var model = new EditSharedGovernorViewModel
            {
                Governor = await SharedGovernorViewModel.MapFromGovernor(governor, establishmentUrn, _cachedLookupService),
                GovernorType = roleName
            };

            await _layoutHelper.PopulateLayoutProperties(model, establishmentUrn, null, User);
            return View(model);
        }

        [HttpPost, Route(EstabEditSharedGovernor), EdubaseAuthorize, ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSharedGovernor(EditSharedGovernorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _governorsWriteService.UpdateDatesAsync(
                    model.Governor.Id,
                    model.Governor.AppointmentStartDate.ToDateTime().Value,
                    model.Governor.AppointmentEndDate.ToDateTime().Value,
                    User
                );
                if (result.Success)
                {
                    var changeSuccessUrl = $"{Url.RouteUrl("EstabDetails", new { id = model.Urn, saved = true })}#school-governance";
                    return Redirect(changeSuccessUrl);
                }

                if (result.Errors is null || result.Errors.Length == 0)
                {
                    throw new TexunaApiSystemException($"The API denied the update (but gave no details!)...");
                }

                // Update the view model with errors from the API response, then fall-through to re-rendering the view,
                // showing the edit form with these errors displayed.
                result.ApplyToModelState(ControllerContext);
            }

            var governor = await _governorsReadService.GetGovernorAsync(model.Governor.Id, User);
            var roleName = (await _cachedLookupService.GovernorRolesGetAllAsync()).Single(x => x.Id == governor.RoleId.Value).Name;
            model.Governor = await SharedGovernorViewModel.MapFromGovernor(governor, model.Urn.Value, _cachedLookupService);
            model.GovernorType = roleName;

            await _layoutHelper.PopulateLayoutProperties(model, model.Urn.Value, null, User);

            return View(model);
        }
    }
}
