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
    using Services.Establishments.Search;
    using Services.Groups;
    using Services.Groups.Models;
    using Services.Groups.Search;
    using StackExchange.Profiling;
    using System;
    using System.Threading.Tasks;
    using ViewModel = AdvancedSearchViewModel;

    public partial class SearchController : EduBaseController
    {
        [HttpGet]
        public async Task<ActionResult> PrepareDownload(AdvancedSearchDownloadViewModel viewModel)
        {
            if(!viewModel.Dataset.HasValue) return View("Download\\SelectDataset");
            else if (!viewModel.FileFormat.HasValue) return View("Download\\SelectFormat");
            else
            {
                var payload = GetEstablishmentSearchPayload(viewModel).Object;
                var progress = await _establishmentDownloadService.SearchWithDownloadGeneration_InitialiseAsync();
                System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(async ct =>
                {
                    await _establishmentDownloadService.SearchWithDownloadGenerationAsync(progress.Id, payload, User, viewModel.Dataset.Value);
                });
                return RedirectToAction(nameof(Download), new { id = progress.Id });
            }
        }

        [HttpGet]
        public async Task<ActionResult> Download(Guid id)
        {
            var model = await _establishmentDownloadService.GetDownloadGenerationProgressAsync(id);
            if (!model.IsComplete) return View("Download\\PreparingFilePleaseWait", model);
            else return View("Download\\ReadyToDownload", model);
        }
        

    }
}