//using Edubase.Services.Establishments;
//using Edubase.Services.Governors;
//using Edubase.Services.Governors.Downloads;
//using Edubase.Services.Governors.Search;
//using Edubase.Services.Groups;
//using Edubase.Services.Lookup;
//using Edubase.Web.UI.Areas.Governors.Models;
//using Edubase.Web.UI.Models;
//using Edubase.Web.UI.Models.Search;
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web.Hosting;
//using System.Web.Mvc;

//namespace Edubase.Web.UI.Areas.Governors.Controllers
//{
//    public class SearchController : Controller
//    {
//        private IGovernorDownloadService _governorDownloadService;
//        private IGovernorsReadService _governorsReadService;
//        private ICachedLookupService _cachedLookupService;
//        private IGroupReadService _groupReadService;
//        private IEstablishmentReadService _establishmentReadService;

//        public SearchController(IGovernorDownloadService governorDownloadService,
//            IGovernorsReadService governorsReadService,
//            ICachedLookupService cachedLookupService,
//            IGroupReadService groupReadService,
//            IEstablishmentReadService establishmentReadService)
//        {
//            _governorDownloadService = governorDownloadService;
//            _governorsReadService = governorsReadService;
//            _cachedLookupService = cachedLookupService;
//            _establishmentReadService = establishmentReadService;
//            _groupReadService = groupReadService;
//        }
        
//        public ActionResult Index() => View(new SearchModel());

//        public async Task<ActionResult> Search(SearchModel model)
//        {
//            const int TAKE = 50;
//            if (!ModelState.IsValid) return View("Index", model);
//            else
//            {
//                model.AppointingBodies = (await _cachedLookupService.GovernorAppointingBodiesGetAllAsync()).Select(x => new LookupItemViewModel(x)).ToList();
//                model.GovernorRoles = (await _cachedLookupService.GovernorRolesGetAllAsync()).Select(x => new LookupItemViewModel(x)).ToList();

//                if (model.RoleId.HasValue) model.RoleName = model.GovernorRoles.FirstOrDefault(x => x.Id == model.RoleId)?.Name;

//                var p = new GovernorSearchPayload
//                {
//                    FirstName = model.Forename,
//                    LastName = model.Surname,
//                    IncludeHistoric = model.IncludeHistoric,
//                    Skip = model.StartIndex,
//                    Take = TAKE
//                };

//                if (model.RoleId.HasValue) p.RoleId = model.RoleId.Value;
//                var results = await _governorsReadService.SearchAsync(p, User);
//                if (!model.Count.HasValue) model.Count = results.Count;
//                model.Results = results;

//                foreach (var item in model.Results.Items)
//                {
//                    if (item.EstablishmentUrn.HasValue)
//                    {
//                        var establishment = await _establishmentReadService.GetAsync(item.EstablishmentUrn.Value, User);
//                        if (establishment.Success) model.EstablishmentNames[item] = establishment.ReturnValue.Name;
//                    }

//                    if (item.GroupUID.HasValue)
//                    {
//                        var group = await _groupReadService.GetAsync(item.GroupUID.Value);
//                        if (group != null) model.GroupNames[item] = group.Name;
//                    }
//                }

//                model.CalculatePageStats(50);
//                return View("Results", model);
//            }
//        }

//        [HttpGet]
//        public async Task<ActionResult> PrepareDownload(SearchModel viewModel)
//        {
//            if (!viewModel.FileFormat.HasValue) return View("Download\\SelectFormat", viewModel);
//            else
//            {
//                var progressId = await InvokeDownloadGenerationAsync(viewModel);
//                return RedirectToAction(nameof(Download), new { id = progressId });
//            }
//        }

//        private async Task<Guid> InvokeDownloadGenerationAsync(SearchModel viewModel)
//        {
//            var payload = new GovernorSearchPayload
//            {
//                FirstName = viewModel.Forename,
//                LastName = viewModel.Surname,
//                IncludeHistoric = viewModel.IncludeHistoric,
//                RoleId = viewModel.RoleId
//            };
//            var progress = await _governorDownloadService.SearchWithDownloadGeneration_InitialiseAsync();

//            // todo: if this process is hosted by us post-Texuna, then need to put into a separate process/server that processes in serial/limited parallelism due to memory consumption.
//            HostingEnvironment.QueueBackgroundWorkItem(async ct =>
//            {
//                await _governorDownloadService.SearchWithDownloadGenerationAsync(progress.Id, payload, User, viewModel.FileFormat.Value);
//            });
//            return progress.Id;
//        }
        
//        [HttpGet]
//        public async Task<ActionResult> Download(Guid id)
//        {
//            var model = await _governorDownloadService.GetDownloadGenerationProgressAsync(id);
//            var viewModel = new SearchDownloadGenerationProgressViewModel(model, AdvancedSearchViewModel.eSearchCollection.Governors);
//            if (model.HasErrored) throw new Exception($"Download generation failed; Further details can be obtained from the logs using exception message id: {model.ExceptionMessageId}");
//            else if (!model.IsComplete) return View("Download\\PreparingFilePleaseWait", viewModel);
//            else return View("Download\\ReadyToDownload", viewModel);
//        }



//    }
//}