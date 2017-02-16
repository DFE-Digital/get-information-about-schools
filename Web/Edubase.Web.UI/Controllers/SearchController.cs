using Edubase.Common;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Downloads;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Downloads;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class SearchController : EduBaseController
    {
        private IEstablishmentReadService _establishmentReadService;
        private ICachedLookupService _cachedLookupService;
        private IGroupReadService _groupReadService;

        public SearchController(IEstablishmentReadService establishmentReadService,
            ICachedLookupService cachedLookupService,
            IGroupReadService groupReadService)
        {
            _cachedLookupService = cachedLookupService;
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
        }
        
        [HttpGet, Route]
        public async Task<ActionResult> Index(SearchViewModel viewModel)
        {
            if (viewModel.LocalAuthorityToRemove.HasValue)
            {
                return Redirect("/?" + QueryStringHelper.ToQueryString(SearchViewModel.BIND_ALIAS_LAIDS,
                    viewModel.RemoveLocalAuthorityId(viewModel.LocalAuthorityToRemove.Value).SelectedLocalAuthorityIds.ToArray()) + "#la");
            }

            if (viewModel.SearchType.HasValue)
            {
                if (viewModel.SearchType == eSearchType.LocalAuthorityDisambiguation)
                {
                    return await ProcessLocalAuthorityDisambiguation(viewModel);
                }
                else if (ModelState.IsValid)
                {
                    if (viewModel.SearchType.OneOfThese(eSearchType.ByLocalAuthority, eSearchType.Location, eSearchType.Text))
                        return Redirect(Url.Action("Index", "EstablishmentsSearch", new { area = "Establishments" }) + "?" + Request.QueryString);
                    else if (viewModel.SearchType == eSearchType.Group)
                        return Redirect(Url.Action("Index", "GroupSearch", new { area = "Groups" }) + "?" + Request.QueryString);
                    else if (viewModel.SearchType == eSearchType.Governor)
                        return Redirect(Url.Action("Index", "GovernorSearch", new { area = "Governors" }) + "?" + Request.QueryString + (viewModel.GovernorSearchModel.RoleId.HasValue ? "&t=" + viewModel.GovernorSearchModel.RoleId : ""));
                    else throw new NotSupportedException($"The search type '{viewModel.SearchType}' is not recognised.");
                }
            }

            viewModel.LocalAuthorities = (await _cachedLookupService.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
            viewModel.GovernorRoles = (await _cachedLookupService.GovernorRolesGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));

            return View(viewModel);
        }

        [Route("Search/Suggest"), HttpGet]
        public async Task<ActionResult> Suggest(string text) => Json(await _establishmentReadService.SuggestAsync(StringUtil.DistillEstablishmentName(text), User));

        [Route("Search/SuggestGroup"), HttpGet]
        public async Task<ActionResult> SuggestGroup(string text) => Json(await _groupReadService.SuggestAsync(text.Distill(), User));

        private async Task<ActionResult> ProcessLocalAuthorityDisambiguation(SearchViewModel model)
        {
            var localAuthorities = await _cachedLookupService.LocalAuthorityGetAllAsync();
            var localAuthority = localAuthorities.FirstOrDefault(x => x.Name.Equals(model.LocalAuthorityToAdd, StringComparison.OrdinalIgnoreCase));
            if (localAuthority != null) return Redirect("/?" + QueryStringHelper.ToQueryString(SearchViewModel.BIND_ALIAS_LAIDS,
                model.AddLocalAuthorityId(localAuthority.Id).SelectedLocalAuthorityIds.ToArray()) + "#la");
            else
            {
                var localAuthorityDisambiguationViewModel = new LocalAuthorityDisambiguationViewModel(model.SelectedLocalAuthorityIds, model.LocalAuthorityToAdd,
                    localAuthorities.Where(x => x.Name.IndexOf(model.LocalAuthorityToAdd, StringComparison.OrdinalIgnoreCase) > -1).Take(10).ToList());
                return View("LocalAuthorityDisambiguation", localAuthorityDisambiguationViewModel);
            }
        }
    }
}