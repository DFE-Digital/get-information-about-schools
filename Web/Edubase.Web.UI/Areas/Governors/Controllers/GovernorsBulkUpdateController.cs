using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Common.IO;
using Edubase.Services.Governors;
using Edubase.Web.UI.Areas.Governors.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [ApiController]
    [Route("governors/bulk-update")]
    [Authorize(Roles = "CanBulkUpdateGovernors")]
    public class GovernorsBulkUpdateController : Controller
    {
        private readonly IGovernorsWriteService _governorsWriteService;
        private readonly IGovernorsReadService _governorsReadService;

        public GovernorsBulkUpdateController(
            IGovernorsWriteService governorsWriteService,
            IGovernorsReadService governorsReadService)
        {
            _governorsWriteService = governorsWriteService;
            _governorsReadService = governorsReadService;
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            var vm = TempData["ProcessBulkUpdate"] as GovernorsBulkUpdateViewModel
                     ?? new GovernorsBulkUpdateViewModel();

            return View("Index", vm);
        }

        [HttpGet("download-template")]
        public async Task<IActionResult> DownloadTemplate()
        {
            var uri = await _governorsReadService.GetGovernorBulkUpdateTemplateUri(User);
            return Redirect(uri);
        }

        [HttpPost("process")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessBulkUpdate(GovernorsBulkUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.BadFileType = true;
                TempData["ProcessBulkUpdate"] = viewModel;
                return RedirectToAction("Index");
            }

            var fileName = FileHelper.GetTempFileName(Path.GetExtension(viewModel.BulkFile.FileName));
            viewModel.BulkFile.SaveAs(fileName);

            if (new FileInfo(fileName).Length > 1_000_000)
            {
                ModelState.AddModelError("BulkFile", "The file size is too large. Please use a file size smaller than 1MB");
            }
            else
            {
                var result = await _governorsWriteService.BulkUpdateValidateAsync(fileName, User);
                if (result.Success.GetValueOrDefault())
                {
                    var apiResponse = await _governorsWriteService.BulkUpdateProcessRequestAsync(result.Id, User);
                    viewModel.WasSuccessful = apiResponse.Success;

                    if (apiResponse.HasErrors)
                    {
                        ModelState.AddModelError("BulkFile", apiResponse.Errors[0].Message);
                    }
                }
                else
                {
                    if (result.Errors != null && result.Errors.Any())
                    {
                        ModelState.AddModelError("BulkFile", result.Errors[0].Message);
                    }
                    else
                    {
                        ModelState.AddModelError("error-log", "Please download the error log to correct your data before resubmitting");
                        viewModel.ErrorLogDownload = result.ErrorLogFile;
                    }
                }
            }

            System.IO.File.Delete(fileName);
            TempData["ProcessBulkUpdate"] = viewModel;
            return RedirectToAction("Index");
        }
    }
}
