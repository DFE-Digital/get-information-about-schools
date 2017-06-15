using Edubase.Common.IO;
using Edubase.Services.Governors;
using Edubase.Web.UI.Areas.Governors.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [RouteArea("Governors"), RoutePrefix("BulkUpdate"), Route("{action=index}")]
    public class GovernorsBulkUpdateController : Controller
    {
        readonly IGovernorsWriteService _governorsWriteService;

        public GovernorsBulkUpdateController(IGovernorsWriteService governorsWriteService)
        {
            _governorsWriteService = governorsWriteService;
        }

        [HttpGet, Route(Name = "GovernorsBulkUpdate")]
        public ActionResult Index() => View(new GovernorsBulkUpdateViewModel());


        [HttpPost, Route("Index", Name = "GovernorsProcessBulkUpdate")]
        public async Task<ActionResult> ProcessBulkUpdate(GovernorsBulkUpdateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var fileName = FileHelper.GetTempFileName(Path.GetExtension(viewModel.BulkFile.FileName));
                viewModel.BulkFile.SaveAs(fileName);

                var result = await _governorsWriteService.BulkUpdateValidateAsync(fileName, User);
                if (result.Success.GetValueOrDefault())
                {
                    var apiResponse = await _governorsWriteService.BulkUpdateProcessRequestAsync(result.Id, User);
                    viewModel.WasSuccessful = apiResponse.Success;
                    if(apiResponse.HasErrors) ModelState.AddModelError("", apiResponse.Errors[0].Message);
                }
                else
                {
                    if (result.Errors != null && result.Errors.Any()) ModelState.AddModelError("", result.Errors[0].Message);
                    else ModelState.AddModelError("", "Please download the error log to correct your data before resubmitting");
                    viewModel.ErrorLogDownload = result.ErrorLogFile;
                }

                System.IO.File.Delete(fileName);
            }
            else viewModel.BadFileType = true;

            return View("Index", viewModel);
        }
    }
}