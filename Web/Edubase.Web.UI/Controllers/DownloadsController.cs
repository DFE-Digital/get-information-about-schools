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
using Newtonsoft.Json;
using Edubase.Services.Domain;

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
        
        public async Task<ActionResult> Index(int? startIndex)
        {
            const int PAGE_SIZE = 100;
            var viewModel = new DownloadsViewModel
            {
                Downloads = await _downloadsService.GetListAsync(User),
                ScheduledExtracts = await _downloadsService.GetScheduledExtractsAsync((startIndex.GetValueOrDefault() / PAGE_SIZE), PAGE_SIZE, User),
                Skip = startIndex.GetValueOrDefault() / PAGE_SIZE,
                Take = PAGE_SIZE
            };

            return View(viewModel);
        }

        [Route("RequestScheduledExtract/{id}", Name = "RequestScheduledExtract")]
        public async Task<ActionResult> RequestScheduledExtract(int id)
        {
            var response = await _downloadsService.GenerateScheduledExtractAsync(id, User);

            if (response.Contains("fileLocationUri")) // Hack because the API sometimes returns ApiResultDto and sometimes ProgressDto!
            {
                return View("ReadyToDownloadScheduledExtract", JsonConvert.DeserializeObject<ProgressDto>(response));
            }
            else
            {
                return RedirectToAction(nameof(Download), new { id = JsonConvert.DeserializeObject<ApiResultDto<Guid>>(response).Value });
            }
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