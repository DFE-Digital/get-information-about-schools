using Edubase.Common.IO;
using Edubase.Services.Governors;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Helpers;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    using R = Services.Security.EdubaseRoles;

    [RouteArea("Governors"), RoutePrefix("BulkUpdate"), MvcAuthorizeRoles(R.EDUBASE_GROUP_MAT, R.ESTABLISHMENT, R.EFADO, R.ROLE_BACKOFFICE)]
    public class GovernorsBulkUpdateController : Controller
    {
        readonly IGovernorsWriteService _governorsWriteService;
        readonly IGovernorsReadService _governorsReadService;

        public GovernorsBulkUpdateController(IGovernorsWriteService governorsWriteService, IGovernorsReadService governorsReadService)
        {
            _governorsWriteService = governorsWriteService;
            _governorsReadService = governorsReadService;
        }

        [HttpGet, Route(Name = "GovernorsBulkUpdate")]
        public async Task<ActionResult> Index()
        {
            var viewModel = new GovernorsBulkUpdateViewModel
            {
                TemplateUri = await _governorsReadService.GetGovernorBulkUpdateTemplateUri(User)
            };
            return View(viewModel);
        }


        [HttpPost, Route(Name = "GovernorsProcessBulkUpdate")]
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
