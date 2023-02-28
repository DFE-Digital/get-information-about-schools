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
using Newtonsoft.Json;
using Edubase.Web.UI.Mappers;

namespace Edubase.Web.UI.Areas.Establishments.Controllers
{
    [RouteArea("Establishments"), RoutePrefix("BulkUpdate"), MvcAuthorizeRoles(AuthorizedRoles.CanBulkUpdateEstablishments)]
    public class BulkUpdateController : Controller
    {
        readonly IEstablishmentWriteService _establishmentWriteService;

        public BulkUpdateController(IEstablishmentWriteService establishmentWriteService)
        {
            _establishmentWriteService = establishmentWriteService;
        }

        [HttpGet, Route(Name = "EstabBulkUpdate")]
        public ActionResult Index() => View(new BulkUpdateViewModel(User.IsInRole(AuthorizedRoles.IsAdmin)));


        [HttpPost, Route(Name = "ProcessBulkUpdate")]
        public async Task<ActionResult> ProcessBulkUpdate(BulkUpdateViewModel viewModel)
        {
            viewModel.CanOverrideCRProcess = User.IsInRole(AuthorizedRoles.IsAdmin);

            if (ModelState.IsValid)
            {
                var payload = viewModel.MapToDto();
                var fileName = payload.FileName;
                viewModel.BulkFile.SaveAs(fileName);

                if (new FileInfo(fileName).Length > 1000000)
                {
                    ModelState.AddModelError("BulkFile", "The file size is too large. Please use a file size smaller than 1MB");
                }
                else
                {
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
                    else
                    {
                        return RedirectToAction(nameof(Result), new { response.GetResponse().Id, state });
                    }
                }
            }

            return View("Index", viewModel);
        }


        [HttpGet, Route("result/{id}/{state}")]
        public async Task<ActionResult> Result(Guid id, string state)
        {
            var model = await _establishmentWriteService.BulkUpdateAsync_GetProgressAsync(id, User);
            if (!model.IsCompleted())
            {
                return View("InProgress", model);
            }
            else
            {
                var dto = UriHelper.DeserializeUrlToken<BulkUpdateDto>(state);
                var vm = new BulkUpdateViewModel
                {
                    BulkUpdateType = dto.BulkFileType,
                    EffectiveDate = new UI.Models.DateTimeViewModel(dto.EffectiveDate),
                    Result = model
                };
                vm.CanOverrideCRProcess = User.IsInRole(AuthorizedRoles.IsAdmin);
                vm.OverrideCRProcess = dto.OverrideCRProcess;
                return View("Index", vm);
            }
        }

        [HttpGet, Route("resultAjax/{id}/{state}")]
        public async Task<ActionResult> ResultAjax(Guid id, string state)
        {
            var model = await _establishmentWriteService.BulkUpdateAsync_GetProgressAsync(id, User);
            return Json(JsonConvert.SerializeObject(new
            {
                status = model.IsCompleted(),
                redirect = string.Concat("/Establishments/BulkUpdate/result/", id,"/", state)
            }), JsonRequestBehavior.AllowGet);

        }
    }
}
