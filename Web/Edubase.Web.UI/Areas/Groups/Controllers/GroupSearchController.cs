using Edubase.Common;
using Edubase.Services.Establishments;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Downloads;
using Edubase.Services.Groups.Models;
using Edubase.Services.Groups.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Groups.Models;
using Edubase.Web.UI.Controllers;
using Edubase.Web.UI.Models;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

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
            if (!viewModel.FileFormat.HasValue) return View("Downloads/SelectFormat", viewModel);
            else
            {
                var progressId = await InvokeGroupSearchDownloadGenerationAsync(viewModel);
                return RedirectToAction(nameof(Download), new { id = progressId });
            }
        }

        [HttpGet, Route("Download")]
        public async Task<ActionResult> Download(Guid id)
        {
            var model = await _groupDownloadService.GetDownloadGenerationProgressAsync(id);
            var viewModel = new GroupSearchDownloadGenerationProgressViewModel(model, (model.IsComplete ? 3 : 2));
            if (model.HasErrored) throw new Exception($"Download generation failed; Further details can be obtained from the logs using exception message id: {model.ExceptionMessageId}");
            else if (!model.IsComplete) return View("Downloads/PreparingFilePleaseWait", viewModel);
            else return View("Downloads/ReadyToDownload", viewModel);
        }

        private async Task<Guid> InvokeGroupSearchDownloadGenerationAsync(GroupSearchDownloadViewModel viewModel)
        {
            var payload = CreateSearchPayload(viewModel);
            var progress = await _groupDownloadService.SearchWithDownloadGeneration_InitialiseAsync();

            // todo: if this process is hosted by us post-Texuna, then need to put into a separate process/server that processes in serial/limited parallelism due to memory consumption.
            HostingEnvironment.QueueBackgroundWorkItem(async ct =>
            {
                await _groupDownloadService.SearchWithDownloadGenerationAsync(progress.Id, payload, User, viewModel.FileFormat.Value);
            });
            return progress.Id;
        }

        private async Task<ActionResult> SearchGroups(GroupSearchViewModel model)
        {
            if (model.GroupSearchModel.AutoSuggestValueAsInt.HasValue) return RedirectToDetailPage(model.GroupSearchModel.AutoSuggestValueAsInt.Value);
            else
            {
                var text = model.GroupSearchModel.Text.Clean();
                model.GroupTypes = (await _lookupService.GroupTypesGetAllAsync()).Select(x => new LookupItemViewModel(x)).ToList();
                using (MiniProfiler.Current.Step("Searching groups..."))
                {
                    AzureSearchResult<SearchGroupDocument> results = null;
                    if (text != null) results = await _groupReadService.SearchByIdsAsync(text, text.ToInteger(), text, User);

                    if (results != null && results.Count > 0)
                    {
                        model.Results.Add(results.Items[0]);
                        model.Count = 1;
                    }
                    else
                    {
                        var payload = CreateSearchPayload(model);
                        using (MiniProfiler.Current.Step("Searching groups (in text mode)..."))
                        {
                            results = await _groupReadService.SearchAsync(payload, User);
                            model.Results = results.Items;
                            if (model.StartIndex == 0) model.Count = results.Count.Value;
                        }
                    }
                }

                if (model.Count == 1)  return RedirectToDetailPage(model.Results.Single().GroupUID);

                return View("GroupResults", model);

            }
        }

        private ActionResult RedirectToDetailPage(int id) 
            => new RedirectToRouteResult(null, new RouteValueDictionary { { "action", "Details" }, { "controller", "Group" }, { "id", id }, { "area", "" } });

        private GroupSearchPayload CreateSearchPayload(GroupSearchViewModel model) => new GroupSearchPayload(model.StartIndex, model.PageSize)
        {
            Text = model.GroupSearchModel.Text.Clean(),
            GroupTypeIds = model.SelectedGroupTypeIds.ToArray(),
            SortBy = model.SortOption
        };

    }
}