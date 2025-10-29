using System;
using System.IO;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Areas.Establishments.Controllers
{
    [Area("Establishments")]
    [Route("Establishments/BulkUpdate")]
    [Authorize(Policy = "CanBulkUpdateEstablishments")]
    public class BulkUpdateController : Controller
    {
        private readonly IEstablishmentWriteService _establishmentWriteService;

        public BulkUpdateController(IEstablishmentWriteService establishmentWriteService)
        {
            _establishmentWriteService = establishmentWriteService;
        }

        [HttpGet, Route("", Name = "EstabBulkUpdate")]
        public ActionResult Index() => View(new BulkUpdateViewModel(User.IsInRole(AuthorizedRoles.IsAdmin)));


        [HttpPost, Route("", Name = "ProcessBulkUpdate"), ValidateAntiForgeryToken]
        public async Task<ActionResult> ProcessBulkUpdate(BulkUpdateViewModel viewModel)
        {
            viewModel.CanOverrideCRProcess = User.IsInRole(AuthorizedRoles.IsAdmin);

            if (ModelState.IsValid)
            {
                var payload = viewModel.MapToDto();
                var fileName = payload.FileName;
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    await viewModel.BulkFile.CopyToAsync(stream);
                }

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

                BulkUpdateViewModel vm = new()
                {
                    BulkUpdateType = dto.BulkFileType,
                    EffectiveDate = new UI.Models.DateTimeViewModel(dto.EffectiveDate),
                    Result = model,
                    CanOverrideCRProcess = User.IsInRole(AuthorizedRoles.IsAdmin),
                    OverrideCRProcess = dto.OverrideCRProcess
                };
                return View("Index", vm);
            }
        }

        [HttpGet("resultAjax/{id}/{state}")]
        public async Task<IActionResult> ResultAjax(Guid id, string state)
        {
            var model = await _establishmentWriteService.BulkUpdateAsync_GetProgressAsync(id, User);

            return Json(new
            {
                status = model.IsCompleted(),
                redirect = $"/Establishments/BulkUpdate/result/{id}/{state}"
            });
        }
    }
}
