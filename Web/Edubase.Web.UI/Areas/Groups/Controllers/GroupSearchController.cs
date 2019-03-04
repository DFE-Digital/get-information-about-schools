using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Downloads;
using Edubase.Services.Groups.Models;
using Edubase.Services.Groups.Search;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Groups.Models;
using Edubase.Web.UI.Controllers;
using Edubase.Web.UI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Edubase.Web.UI.Models.Search;

namespace Edubase.Web.UI.Areas.Groups.Controllers
{
    [RouteArea("Groups"), RoutePrefix("Search"), Route("{action=index}")]
    public class GroupSearchController : EduBaseController
    {
        IGroupReadService _groupReadService;
        ICachedLookupService _lookupService;
        IGroupDownloadService _groupDownloadService;

        public GroupSearchController(
            IGroupReadService groupReadService,
            IGroupDownloadService groupDownloadService,
            ICachedLookupService lookupService)
        {
            _groupReadService = groupReadService;
            _lookupService = lookupService;
            _groupDownloadService = groupDownloadService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(GroupSearchViewModel model) => await SearchGroups(model);

        [HttpGet, Route("results-js")]
        public async Task<ActionResult> ResultsPartial(GroupSearchViewModel model)
        {
            var result = await SearchGroups(model);
            HttpContext.Response.Headers.Add("x-count", model.Count.ToString());
            return PartialView("Partials/_GroupSearchResults", model);
        }

        [HttpGet, Route("PrepareDownload")]
        public async Task<ActionResult> PrepareDownload(GroupSearchDownloadViewModel viewModel)
        {
            viewModel.SearchSource = eLookupSearchSource.Groups;

            if (!viewModel.FileFormat.HasValue)
            {
                viewModel.SearchQueryString = Request.QueryString.ToString();
                viewModel.Step = 1;
                return View("Downloads/SelectFormat", viewModel);
            }

            var progressId = await _groupDownloadService.SearchWithDownloadGenerationAsync(
                new SearchDownloadDto<GroupSearchPayload>
                {
                    SearchPayload = CreateSearchPayload(viewModel),
                    FileFormat = viewModel.FileFormat.Value
                }, User);
            return RedirectToAction(nameof(Download), new { id = progressId, fileFormat = viewModel.FileFormat.Value, viewModel.SearchQueryString, viewModel.SearchSource });
        }

        [HttpGet, Route("Download")]
        public async Task<ActionResult> Download(Guid id, eFileFormat fileFormat, string searchQueryString = null, eLookupSearchSource? searchSource = null)
        {
            var model = await _groupDownloadService.GetDownloadGenerationProgressAsync(id, User);
            var viewModel = new GroupSearchDownloadGenerationProgressViewModel(model)
            {
                FileFormat = fileFormat,
                SearchSource = searchSource,
                SearchQueryString = searchQueryString
            };
            

            if (model.HasErrored)
                throw new Exception($"Download generation failed; Underlying error: '{model.Error}'");

            if (!model.IsComplete) return View("Downloads/PreparingFilePleaseWait", viewModel.SetStep(2));

            return View("Downloads/ReadyToDownload", viewModel.SetStep(3));
        }

        private async Task<ActionResult> SearchGroups(GroupSearchViewModel model)
        {
            if (model.GroupSearchModel.AutoSuggestValueAsInt.HasValue) return RedirectToDetailPage(model.GroupSearchModel.AutoSuggestValueAsInt.Value);
            else
            {
                model.SearchQueryString = Request.QueryString.ToString();
                var text = model.GroupSearchModel.Text.Clean();
                model.GroupTypes = (await _lookupService.GroupTypesGetAllAsync()).Select(x => new LookupItemViewModel(x)).ToList();
                model.GroupStatuses = (await _lookupService.GroupStatusesGetAllAsync()).Select(x => new LookupItemViewModel(x)).ToList();

                ApiPagedResult<SearchGroupDocument> results = null;
                if (text != null) results = await _groupReadService.SearchByIdsAsync(text, text.ToInteger(), text, User);

                if (results != null && results.Count > 0)
                {
                    model.Results.Add(results.Items[0]);
                    model.Count = 1;
                }
                else
                {
                    var payload = CreateSearchPayload(model);
                    results = await _groupReadService.SearchAsync(payload, User);
                    model.Results = results.Items;
                    if (model.StartIndex == 0) model.Count = results.Count;
                }
                
                if (model.Count == 1) {return RedirectToDetailPage(model.Results.Single().GroupUId);}
                if (model.Count == 0) {return RedirectToSearchPage(model);}

                return View("GroupResults", model);
            }
        }

        private ActionResult RedirectToSearchPage(GroupSearchViewModel model)
        {
            var routeDictionary = new RouteValueDictionary
            {
                {"action", "Index"},
                {"controller", "Search"},
                {"area", string.Empty},
                {"SelectedTab", SearchViewModel.Tab.Groups},
                {"SearchType", eSearchType.Group},
                {"GroupSearchModel.Text", model.GroupSearchModel.Text},
                { "NoResults", "True"}
            };
            return new RedirectResult(Url.RouteUrl(routeDictionary));
        }

        private ActionResult RedirectToDetailPage(int id)
            => new RedirectToRouteResult(null, new RouteValueDictionary { { "action", "Details" }, { "controller", "Group" }, { "id", id }, { "area", "Groups" } });

        private GroupSearchPayload CreateSearchPayload(GroupSearchViewModel model) => new GroupSearchPayload(model.StartIndex, model.PageSize)
        {
            Text = model.GroupSearchModel.Text.Clean(),
            GroupTypeIds = model.SelectedGroupTypeIds.ToArray(),
            GroupStatusIds = (User.Identity.IsAuthenticated) ? model.SelectedGroupStatusIds.ToArray() : null,
            SortBy = model.SortOption
        };
    }
}
