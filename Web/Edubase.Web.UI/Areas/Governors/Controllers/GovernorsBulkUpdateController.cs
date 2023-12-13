using Edubase.Common.IO;
using Edubase.Services.Governors;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Helpers;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Web.UI.Filters;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [RouteArea("Governors"), RoutePrefix("BulkUpdate"), MvcAuthorizeRoles(AuthorizedRoles.CanBulkUpdateGovernors)]
    public class GovernorsBulkUpdateController : Controller
    {
        readonly IGovernorsWriteService _governorsWriteService;
        readonly IGovernorsReadService _governorsReadService;

        public GovernorsBulkUpdateController(IGovernorsWriteService governorsWriteService, IGovernorsReadService governorsReadService)
        {
            _governorsWriteService = governorsWriteService;
            _governorsReadService = governorsReadService;
        }

        [HttpGet, Route(Name = "GovernorsBulkUpdate"), ImportModelState]
        public ActionResult Index()
        {
            var vm = TempData["ProcessBulkUpdate"] as GovernorsBulkUpdateViewModel ?? new GovernorsBulkUpdateViewModel();

            return View("Index", vm);
        }

        [HttpGet, Route("DownloadTemplate")]
        public async Task<ActionResult> DownloadTemplate()
        {
            return Redirect(await _governorsReadService.GetGovernorBulkUpdateTemplateUri(User));
        }

        [HttpPost, Route(Name = "GovernorsProcessBulkUpdate"), ExportModelState]
        public async Task<ActionResult> ProcessBulkUpdate(GovernorsBulkUpdateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var fileName = FileHelper.GetTempFileName(Path.GetExtension(viewModel.BulkFile.FileName));
                viewModel.BulkFile.SaveAs(fileName);

                if (new FileInfo(fileName).Length > 1000000)
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
                        if (apiResponse.HasErrors) { ModelState.AddModelError("BulkFile", apiResponse.Errors[0].Message); }
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
                    System.IO.File.Delete(fileName);
                }
            }
            else
            {
                viewModel.BadFileType = true;
            }

            TempData["ProcessBulkUpdate"] = viewModel;
            return RedirectToAction("Index");
        }
    }
}
