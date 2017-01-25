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
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;
    using System.Web.Hosting;
    using static AdvancedSearchViewModel;
    using ViewModel = AdvancedSearchViewModel;

    public partial class SearchController : EduBaseController
    {
        [HttpGet]
        public async Task<ActionResult> PrepareDownload(AdvancedSearchDownloadViewModel viewModel)
        {
            if(!viewModel.Dataset.HasValue && viewModel.SearchCollection == eSearchCollection.Establishments)
                return View("Download\\SelectDataset");
            else if (!viewModel.FileFormat.HasValue) return View("Download\\SelectFormat", viewModel);
            else
            {
                var progressId = Guid.Empty;
                if (viewModel.SearchCollection == eSearchCollection.Establishments)
                {
                    progressId = await InvokeEstablishmentDownloadGenerationAsync(viewModel);
                }
                else if (viewModel.SearchCollection == eSearchCollection.Groups)
                {
                    progressId = await InvokeGroupDownloadGenerationAsync(viewModel);
                }
                //else if (viewModel.SearchCollection == eSearchCollection.Governors)
                //{
                //    progressId = await InvokeGovernorDownloadGenerationAsync(viewModel);
                //}
                else throw new NotImplementedException($"No download generation supported for SearchCollection:'{viewModel.SearchCollection}'");

                return RedirectToAction(nameof(Download), new { id = progressId, collection = viewModel.SearchCollection.ToString().ToLower() });
            }
        }

        private async Task<Guid> InvokeEstablishmentDownloadGenerationAsync(AdvancedSearchDownloadViewModel viewModel)
        {
            var payload = GetEstablishmentSearchPayload(viewModel).Object;
            var progress = await _establishmentDownloadService.SearchWithDownloadGeneration_InitialiseAsync();
            
            // todo: if this process is hosted by us post-Texuna, then need to put into a separate process/server that processes in serial/limited parallelism due to memory consumption.
            HostingEnvironment.QueueBackgroundWorkItem(async ct =>
            {
                await _establishmentDownloadService.SearchWithDownloadGenerationAsync(progress.Id, payload, User, viewModel.Dataset.Value, viewModel.FileFormat.Value);
            });
            return progress.Id;
        }

        private async Task<Guid> InvokeGroupDownloadGenerationAsync(AdvancedSearchDownloadViewModel viewModel)
        {
            var payload = GetGroupSearchPayload(viewModel);
            var progress = await _groupDownloadService.SearchWithDownloadGeneration_InitialiseAsync();
            
            // todo: if this process is hosted by us post-Texuna, then need to put into a separate process/server that processes in serial/limited parallelism due to memory consumption.
            HostingEnvironment.QueueBackgroundWorkItem(async ct =>
            {
                await _groupDownloadService.SearchWithDownloadGenerationAsync(progress.Id, payload, User, viewModel.FileFormat.Value);
            });
            return progress.Id;
        }
        
        [HttpGet]
        public async Task<ActionResult> Download(Guid id, eSearchCollection collection)
        {
            var model = await _establishmentDownloadService.GetDownloadGenerationProgressAsync(id);
            var viewModel = new SearchDownloadGenerationProgressViewModel(model, collection);
            if (model.HasErrored) throw new Exception($"Download generation failed; Further details can be obtained from the logs using exception message id: {model.ExceptionMessageId}");
            else if (!model.IsComplete) return View("Download\\PreparingFilePleaseWait", viewModel);
            else return View("Download\\ReadyToDownload", viewModel);
        }
        

    }
}