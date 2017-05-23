using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Edubase.Services.Downloads;
using System.Threading.Tasks;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Downloads"), Route("{action=index}")]
    public class DownloadsController : Controller
    {
        private readonly IDownloadsService _downloadsService;

        public DownloadsController(IDownloadsService downloadsService)
        {
            _downloadsService = downloadsService;
        }
        
        public async Task<ActionResult> Index(int? skip, int? take)
        {
            var viewModel = new DownloadsViewModel
            {
                Downloads = await _downloadsService.GetListAsync(User),
                ScheduledExtracts = await _downloadsService.GetScheduledExtractsAsync(skip.GetValueOrDefault(), take.GetValueOrDefault(10), User),
                Skip = skip.GetValueOrDefault(),
                Take = take.GetValueOrDefault(10)
            };

            return View(viewModel);
        }

        [Route("RequestScheduledExtract/{id}", Name = "RequestScheduledExtract")]
        public async Task<ActionResult> RequestScheduledExtract(int id)
        {
            var requestId = await _downloadsService.GenerateScheduledExtractAsync(id, User);
            return RedirectToAction(nameof(Download), new { id = requestId });
        }

        [Route("Download/{id}", Name = "DownloadScheduledExtract")]
        public async Task<ActionResult> Download(Guid id)
        {
            var model = await _downloadsService.GetProgressOfScheduledExtractGenerationAsync(id, User);

            if (model.HasErrored)
                throw new Exception($"Download generation failed; Underlying error: '{model.Error}'");

            if (!model.IsComplete)
                return View("PreparingScheduledExtractPleaseWait", model);

            return View("ReadyToDownloadScheduledExtract", model);
        }
    }
}