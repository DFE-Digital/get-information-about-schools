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
    [ApiController]
    [Route("establishments/bulk-update")]
    [Authorize(Roles = "CanBulkUpdateEstablishments")]
    public class BulkUpdateController : Controller
    {
        private readonly IEstablishmentWriteService _establishmentWriteService;

        public BulkUpdateController(IEstablishmentWriteService establishmentWriteService)
        {
            _establishmentWriteService = establishmentWriteService;
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            var viewModel = new BulkUpdateViewModel(User.IsInRole(AuthorizedRoles.IsAdmin));
            return View(viewModel);
        }

        [HttpPost("process")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessBulkUpdate(BulkUpdateViewModel viewModel)
        {
            viewModel.CanOverrideCRProcess = User.IsInRole(AuthorizedRoles.IsAdmin);

            if (!ModelState.IsValid)
            {
                return View("Index", viewModel);
            }

            var payload = viewModel.MapToDto();
            var fileName = payload.FileName;
            viewModel.BulkFile.SaveAs(fileName);

            if (new FileInfo(fileName).Length > 1_000_000)
            {
                ModelState.AddModelError("BulkFile", "The file size is too large. Please use a file size smaller than 1MB");
                return View("Index", viewModel);
            }

            var state = UriHelper.SerializeToUrlToken(payload);
            var response = await _establishmentWriteService.BulkUpdateAsync(payload, User);
            System.IO.File.Delete(fileName);

            if (response.HasErrors)
            {
                viewModel.Result = new Services.Domain.BulkUpdateProgressModel
                {
                    Errors = response.Errors
                };
                return View("Index", viewModel);
            }

            return RedirectToAction(nameof(Result), new { response.GetResponse().Id, state });
        }

        [HttpGet("result/{id}/{state}")]
        public async Task<IActionResult> Result(Guid id, string state)
        {
            var model = await _establishmentWriteService.BulkUpdateAsync_GetProgressAsync(id, User);
            if (!model.IsCompleted())
            {
                return View("InProgress", model);
            }

            var dto = UriHelper.DeserializeUrlToken<BulkUpdateDto>(state);
            var vm = new BulkUpdateViewModel
            {
                BulkUpdateType = dto.BulkFileType,
                EffectiveDate = new UI.Models.DateTimeViewModel(dto.EffectiveDate),
                Result = model,
                CanOverrideCRProcess = User.IsInRole(AuthorizedRoles.IsAdmin),
                OverrideCRProcess = dto.OverrideCRProcess
            };

            return View("Index", vm);
        }

        [HttpGet("result-ajax/{id}/{state}")]
        public async Task<IActionResult> ResultAjax(Guid id, string state)
        {
            var model = await _establishmentWriteService.BulkUpdateAsync_GetProgressAsync(id, User);
            var json = new
            {
                status = model.IsCompleted(),
                redirect = $"/establishments/bulk-update/result/{id}/{state}"
            };

            return new JsonResult(json);
        }
    }
}
