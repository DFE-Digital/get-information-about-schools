using Edubase.Common;
using Edubase.Services.Establishments;
using Edubase.Services.Governors.Downloads;
using Edubase.Services.Governors;
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
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Services.Governors.Search;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [RouteArea("Governors"), RoutePrefix("Search"), Route("{action=index}")]
    public class GovernorSearchController : EduBaseController
    {
        private IGovernorDownloadService _governorDownloadService;
        private IGovernorsReadService _governorsReadService;
        private ICachedLookupService _cachedLookupService;
        private IGroupReadService _groupReadService;
        private IEstablishmentReadService _establishmentReadService;

        public GovernorSearchController(IGovernorDownloadService governorDownloadService,
                    IGovernorsReadService governorsReadService,
                    ICachedLookupService cachedLookupService,
                    IGroupReadService groupReadService,
                    IEstablishmentReadService establishmentReadService)
        {
            _governorDownloadService = governorDownloadService;
            _governorsReadService = governorsReadService;
            _cachedLookupService = cachedLookupService;
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
        }


        [HttpGet]
        public async Task<ActionResult> Index(GovernorSearchViewModel model) => await SearchGovernors(model);

        [HttpGet, Route("results-js")]
        public async Task<ActionResult> ResultsPartial(GovernorSearchViewModel model)
        {
            var result = await SearchGovernors(model);
            HttpContext.Response.Headers.Add("x-count", model.Count.ToString());
            return PartialView("Partials/_GovernorSearchResults", model);
        }


        [HttpGet, Route("PrepareDownload")]
        public async Task<ActionResult> PrepareDownload(GovernorSearchDownloadViewModel viewModel)
        {
            if (!viewModel.FileFormat.HasValue) return View("Downloads/SelectFormat", viewModel);
            else
            {
                var progressId = await InvokeGovernorSearchDownloadGenerationAsync(viewModel);
                return RedirectToAction(nameof(Download), new { id = progressId });
            }
        }

        [HttpGet, Route("Download")]
        public async Task<ActionResult> Download(Guid id)
        {
            var model = await _governorDownloadService.GetDownloadGenerationProgressAsync(id);
            var viewModel = new GovernorSearchDownloadGenerationProgressViewModel(model, (model.IsComplete ? 3 : 2));
            if (model.HasErrored) throw new Exception($"Download generation failed; Further details can be obtained from the logs using exception message id: {model.ExceptionMessageId}");
            else if (!model.IsComplete) return View("Downloads/PreparingFilePleaseWait", viewModel);
            else return View("Downloads/ReadyToDownload", viewModel);
        }

        private async Task<Guid> InvokeGovernorSearchDownloadGenerationAsync(GovernorSearchDownloadViewModel viewModel)
        {
            var payload = CreateSearchPayload(viewModel);
            var progress = await _governorDownloadService.SearchWithDownloadGeneration_InitialiseAsync();
            var principal = User;

            // todo: remove post-texuna integration.
            HostingEnvironment.QueueBackgroundWorkItem(async ct =>
            {
                await _governorDownloadService.SearchWithDownloadGenerationAsync(progress.Id, payload, principal, viewModel.FileFormat.Value);
            });
            return progress.Id;
        }

        private async Task<ActionResult> SearchGovernors(GovernorSearchViewModel model)
        {
            if (model.GovernorSearchModel?.RoleId != null && model.GovernorSearchModel.RoleId.Any())
            {
                model.SelectedRoleIds.AddRange(model.GovernorSearchModel.RoleId
                    .Where(r => r.HasValue && !model.SelectedRoleIds.Contains(r.Value))
                    .Cast<int>());
            }

            model.GovernorRoles = (await _cachedLookupService.GovernorRolesGetAllAsync()).Select(x => new LookupItemViewModel(x)).ToList();
            model.AppointingBodies = (await _cachedLookupService.GovernorAppointingBodiesGetAllAsync()).Select(x => new LookupItemViewModel(x)).ToList();

            using (MiniProfiler.Current.Step("Searching governors..."))
            {
                var payload = CreateSearchPayload(model);
                using (MiniProfiler.Current.Step("Searching governors (in text mode)..."))
                {
                    var results = await _governorsReadService.SearchAsync(payload, User);
                    model.Results = results.Items;
                    if (model.StartIndex == 0) model.Count = results.Count;

#if (!TEXAPI)
                    foreach (var item in model.Results)
                    {
                        if (item.EstablishmentUrn.HasValue && item.EstablishmentName.IsNullOrEmpty())
                        {
                            var establishment = await _establishmentReadService.GetAsync(item.EstablishmentUrn.Value, User);
                            if (establishment.Success) item.EstablishmentName = establishment.ReturnValue.Name;
                        }

                        if (item.GroupUID.HasValue && item.GroupName.IsNullOrEmpty())
                        {
                            var result = await _groupReadService.GetAsync(item.GroupUID.Value, User);
                            if (result.Success) item.GroupName = result.ReturnValue.Name;
                        }
                    }
#endif
                }
            }
            
            return View("Index", model);
        }

        private GovernorSearchPayload CreateSearchPayload(GovernorSearchViewModel model) => new GovernorSearchPayload(model.StartIndex, model.PageSize)
        {
            Gid = model.GovernorSearchModel.Gid?.ToString(),
            FirstName = model.GovernorSearchModel.Forename.Clean(),
            LastName = model.GovernorSearchModel.Surname.Clean(),
            RoleIds = model.SelectedRoleIds.ToArray(),
            SortBy = model.SortOption,
            IncludeHistoric = model.GovernorSearchModel.IncludeHistoric
        };

    }
}