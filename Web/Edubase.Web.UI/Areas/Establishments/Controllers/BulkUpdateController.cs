using Edubase.Common;
using Edubase.Common.IO;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Areas.Establishments.Controllers
{
    using R = Services.Security.EdubaseRoles;

    [RouteArea("Establishments"), RoutePrefix("BulkUpdate"), Route("{action=index}"), MvcAuthorizeRoles(R.ROLE_PRISM, R.ROLE_STAKEHOLDER, R.ROLE_BACKOFFICE)]
    public class BulkUpdateController : Controller
    {
        readonly IEstablishmentWriteService _establishmentWriteService;

        public BulkUpdateController(IEstablishmentWriteService establishmentWriteService)
        {
            _establishmentWriteService = establishmentWriteService;
        }

        [HttpGet, Route(Name = "EstabBulkUpdate")]
        public ActionResult Index() => View(new BulkUpdateViewModel(User.IsInRole(R.ROLE_BACKOFFICE)));


        [HttpPost, Route("Index", Name = "ProcessBulkUpdate")]
        public async Task<ActionResult> ProcessBulkUpdate(BulkUpdateViewModel viewModel)
        {
            viewModel.CanOverrideCRProcess = User.IsInRole(R.ROLE_BACKOFFICE);

            if (ModelState.IsValid)
            {
                var fileName = FileHelper.GetTempFileName(Path.GetExtension(viewModel.BulkFile.FileName));
                viewModel.BulkFile.SaveAs(fileName);

                if (new FileInfo(fileName).Length > 1000000)
                {
                    viewModel.Result = new Services.Domain.BulkUpdateProgressModel
                    {
                        Errors = new[] { new Services.Domain.ApiError { Code = "error.maxRowsLimitReached.payload.bulkUpload" } }
                    };
                }
                else
                {
                    var payload = new BulkUpdateDto
                    {
                        BulkFileType = viewModel.BulkUpdateType.Value,
                        FileName = fileName,
                        OverrideCRProcess = viewModel.CanOverrideCRProcess && viewModel.OverrideCRProcess
                    };

                    var state = UriHelper.SerializeToUrlToken(payload);
                    var response = await _establishmentWriteService.BulkUpdateAsync(payload, User);
                    System.IO.File.Delete(fileName);

                    if (response.HasErrors)
                    {
                        viewModel.Result = new Services.Domain.BulkUpdateProgressModel
                        {
                            Errors = response.Errors
                        };
                    }
                    else return RedirectToAction(nameof(Result), new { response.GetResponse().Id, state });
                }
            }

            return View("Index", viewModel);
        }


        [HttpGet, Route("result/{id}/{state}")]
        public async Task<ActionResult> Result(Guid id, string state)
        {
            var model = await _establishmentWriteService.BulkUpdateAsync_GetProgressAsync(id, User);
            if (!model.IsCompleted()) return View("InProgress", model);
            else
            {
                var dto = UriHelper.DeserializeUrlToken<BulkUpdateDto>(state);
                var vm = new BulkUpdateViewModel
                {
                    BulkUpdateType = dto.BulkFileType,
                    EffectiveDate = new UI.Models.DateTimeViewModel(dto.EffectiveDate),
                    Result = model
                };
                vm.CanOverrideCRProcess = User.IsInRole(R.ROLE_BACKOFFICE);
                vm.OverrideCRProcess = dto.OverrideCRProcess;
                return View("Index", vm);
            }
        }
    }
}