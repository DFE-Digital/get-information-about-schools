using Edubase.Common;
using Edubase.Web.UI.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System;

namespace Edubase.Web.UI.Controllers
{
    using Common.Spatial;
    using Helpers;
    using Models.Search;
    using Services;
    using Services.Domain;
    using Services.Establishments;
    using Services.Establishments.Downloads;
    using Services.Establishments.Search;
    using Services.Groups;
    using Services.Groups.Downloads;
    using Services.Groups.Models;
    using Services.Groups.Search;
    using Services.Lookup;
    using StackExchange.Profiling;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ViewModel = AdvancedSearchViewModel;

    public partial class SearchController : EduBaseController
    {
        IEstablishmentReadService _establishmentReadService;
        IGroupReadService _groupReadService;
        IEstablishmentDownloadService _establishmentDownloadService;
        ICachedLookupService _lookupService;
        IGroupDownloadService _groupDownloadService;

        public SearchController(IEstablishmentReadService establishmentReadService, 
            IGroupReadService groupReadService,
            IEstablishmentDownloadService establishmentDownloadService,
            IGroupDownloadService groupDownloadService,
            ICachedLookupService lookupService)
        {
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
            _establishmentDownloadService = establishmentDownloadService;
            _lookupService = lookupService;
            _groupDownloadService = groupDownloadService;
        }

        public async Task<ActionResult> Index(ViewModel vm) => View(await PopulateLookups(vm));
        
        [HttpGet]
        public async Task<ActionResult> Results(ViewModel model)
        {
            if (model.LocalAuthorityToRemove.HasValue)
            {
                return Redirect("/?" + QueryStringHelper.ToQueryString("l", model.RemoveLocalAuthorityId(model.LocalAuthorityToRemove.Value).SelectedLocalAuthorityIds.ToArray()) + "#la");
            }
            else if (model.SearchType == ViewModel.eSearchType.LocalAuthorityDisambiguation)
            {
                return await ProcessLocalAuthorityDisambiguation(model);
            }
            else if (model.SearchCollection == ViewModel.eSearchCollection.Establishments)
            {
                var retVal = await SearchByUrnAsync(model);
                if (retVal != null) return retVal;

                var payload = GetEstablishmentSearchPayload(model);
                if (!payload.Success) model.Error = payload.ErrorMessage;
                return await ProcessEstablishmentsSearch(model, payload.Object);
            }
            else
            {
                if (model.SearchType == ViewModel.eSearchType.Group) return await SearchGroups(model);
                else if (model.SearchType == ViewModel.eSearchType.Governor) return SearchGovernors(model.GovernorSearchModel);
                throw new NotImplementedException();
            }
        }

        private async Task<ActionResult> ProcessLocalAuthorityDisambiguation(ViewModel model)
        {
            var localAuthorities = await _lookupService.LocalAuthorityGetAllAsync();
            var localAuthority = localAuthorities.FirstOrDefault(x => x.Name.Equals(model.LocalAuthorityToAdd, StringComparison.OrdinalIgnoreCase));
            if (localAuthority != null) return Redirect("/?" + QueryStringHelper.ToQueryString("l", model.AddLocalAuthorityId(localAuthority.Id).SelectedLocalAuthorityIds.ToArray()) + "#la");
            else
            {
                var localAuthorityDisambiguationViewModel = new LocalAuthorityDisambiguationViewModel(model.SelectedLocalAuthorityIds, model.LocalAuthorityToAdd,
                    localAuthorities.Where(x => x.Name.IndexOf(model.LocalAuthorityToAdd, StringComparison.OrdinalIgnoreCase) > -1).Take(10).ToList());
                return View("LocalAuthorityDisambiguation", localAuthorityDisambiguationViewModel);
            }
        }

        private ActionResult SearchGovernors(Areas.Governors.Models.SearchModel governorSearchModel)
        {
            return Redirect("/Governors/Search?Forename=" + governorSearchModel.Forename + "&Surname=" + governorSearchModel.Surname + "&RoleId=" + governorSearchModel.RoleId + "&IncludeHistoric=" + governorSearchModel.IncludeHistoric);
        }
        
        private async Task<ActionResult> SearchGroups(ViewModel model)
        {
            if (model.GroupSearchModel.AutoSuggestValueAsInt.HasValue)
            {
                return new RedirectToRouteResult(null, new RouteValueDictionary
                {
                    { "action", "Details" },
                    { "controller", "Group" },
                    { "id", model.GroupSearchModel.AutoSuggestValueAsInt }
                });
            }
            else
            {
                var text = model.GroupSearchModel.Text.Clean();
                var viewModel = new GroupSearchResultsModel(text) { StartIndex = model.StartIndex, Count = model.Count };
                viewModel.GroupTypes = (await _lookupService.GroupTypesGetAllAsync()).Select(x => new LookupItemViewModel(x)).ToList();
                using (MiniProfiler.Current.Step("Searching groups..."))
                {
                    var results = await _groupReadService.SearchByIdsAsync(text, text.ToInteger(), text, User);

                    if (results.Count > 0)
                    {
                        viewModel.Results.Add(results.Items[0]);
                        viewModel.Count = 1;
                    }
                    else
                    {
                        GroupSearchPayload payload = GetGroupSearchPayload(model);
                        using (MiniProfiler.Current.Step("Searching groups (in text mode)..."))
                        {
                            results = await _groupReadService.SearchAsync(payload, User);
                            viewModel.Results = results.Items;
                            if (model.StartIndex == 0) viewModel.Count = results.Count.Value;
                        }
                    }
                }
                
                viewModel.SearchType = "Groups";

                if (viewModel.Count == 1)
                {
                    return new RedirectToRouteResult(null, new RouteValueDictionary
                                {
                                    { "action", "Details" },
                                    { "controller", "Group" },
                                    { "id", viewModel.Results.Single().GroupUID }
                                });
                }
                else viewModel.CalculatePageStats(model.PageSize);

                return View("GroupResults", viewModel);

            }
        }

        private GroupSearchPayload GetGroupSearchPayload(ViewModel model)
        {
            return new GroupSearchPayload(nameof(GroupModel.Name), model.StartIndex, model.PageSize) { Text = model.GroupSearchModel.Text.Clean() };
        }

        [HttpGet]
        public async Task<ActionResult> Suggest(string text) => Json(await _establishmentReadService.SuggestAsync(text, User));

        [HttpGet]
        public async Task<ActionResult> SuggestGroup(string text) => Json(await _groupReadService.SuggestAsync(text, User));
        
        

    }
}