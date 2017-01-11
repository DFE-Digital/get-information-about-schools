using Edubase.Common;
using Edubase.Web.UI.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System;

namespace Edubase.Web.UI.Controllers
{
    using Common.Spatial;
    using Models.Search;
    using Services;
    using Services.Domain;
    using Services.Establishments;
    using Services.Establishments.Downloads;
    using Services.Establishments.Search;
    using Services.Groups;
    using Services.Groups.Models;
    using Services.Groups.Search;
    using Services.Lookup;
    using StackExchange.Profiling;
    using System;
    using System.Threading.Tasks;
    using ViewModel = AdvancedSearchViewModel;

    public partial class SearchController : EduBaseController
    {
        IEstablishmentReadService _establishmentReadService;
        IGroupReadService _groupReadService;
        IEstablishmentDownloadService _establishmentDownloadService;
        ICachedLookupService _lookupService;

        public SearchController(IEstablishmentReadService establishmentReadService, 
            IGroupReadService groupReadService,
            IEstablishmentDownloadService establishmentDownloadService,
            ICachedLookupService lookupService)
        {
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
            _establishmentDownloadService = establishmentDownloadService;
            _lookupService = lookupService;
        }

        public async Task<ActionResult> Index() => View(await PopulateLookups(new ViewModel()));
        
        [HttpGet]
        public async Task<ActionResult> Results(ViewModel model)
        {
            if(model.SearchCollection == ViewModel.eSearchCollection.Establishments)
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
                        var payload = new GroupSearchPayload(nameof(GroupModel.Name), model.StartIndex, model.PageSize) { Text = text };
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
                
        
        [HttpGet]
        public async Task<ActionResult> Suggest(string text) => Json(await _establishmentReadService.SuggestAsync(text, User));

        [HttpGet]
        public async Task<ActionResult> SuggestGroup(string text) => Json(await _groupReadService.SuggestAsync(text, User));
        
        

    }
}