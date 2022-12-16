using Edubase.Common.IO;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Controllers;
using Edubase.Web.UI.Helpers;
using MoreLinq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Edubase.Web.UI.Areas.Establishments.Controllers
{
    using R = Services.Security.EdubaseRoles;

    [RouteArea("Establishments"), MvcAuthorizeRoles(AuthorizedRoles.CanBulkAssociateEstabs2Groups)]
    public class BulkAssociateEstabs2GroupsController : EduBaseController
    {
        private const string ViewName = "BulkAssociateEstabs2Groups";
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private const string BaseUri = "bulk-associate-estabs-to-groups";
        private const string ErrorLinkFile = "BulkFile";
        private const string ErrorLinkLog = "error-log";

        public BulkAssociateEstabs2GroupsController(IEstablishmentWriteService establishmentWriteService) => _establishmentWriteService = establishmentWriteService;

        [Route(BaseUri, Name = "BulkAssociateEstabs2Groups"), HttpGet]
        public ActionResult Index() => View(ViewName);

        [Route(BaseUri, Name = "BulkAssociateEstabs2GroupsPost"), HttpPost]
        public async Task<ActionResult> ProcessBulkAssociateEstabs2GroupsAsync(BulkAssociateEstabs2GroupsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var fileName = FileHelper.GetTempFileName(Path.GetExtension(viewModel.BulkFile.FileName));
                viewModel.BulkFile.SaveAs(fileName);
                var result = await _establishmentWriteService.BulkAssociateEstabs2GroupsAsync(fileName, User);
                return result.HasErrors
                    ? await ResultInternalAsync(Guid.Empty, viewModel, result)
                    : RedirectToRoute("BulkAssociateEstabs2GroupsResult", new { id = result.GetResponse().Id });
            }
            return View(ViewName, viewModel);
        }

        [Route(BaseUri + "/{id}", Name = "BulkAssociateEstabs2GroupsResult"), HttpGet]
        public async Task<ActionResult> ResultAsync(Guid id)
        {
            var viewModel = new BulkAssociateEstabs2GroupsViewModel();
            var apiResponse = await _establishmentWriteService.BulkAssociateEstabs2GroupsGetProgressAsync(id, User);
            return await ResultInternalAsync(id, viewModel, apiResponse);
        }

        [Route(BaseUri + "-ajax/{id}", Name = "BulkAssociateEstabs2GroupsResultAjax"), HttpGet]
        public async Task<ActionResult> ResultAsyncAjax(Guid id)
        {
            var viewModel = new BulkAssociateEstabs2GroupsViewModel();
            var apiResponse = await _establishmentWriteService.BulkAssociateEstabs2GroupsGetProgressAsync(id, User);
            return await Task.Run(() => ResultInternalAjaxAsync(id, viewModel, apiResponse));
        }

        private async Task<ActionResult> ResultInternalAsync(Guid id, BulkAssociateEstabs2GroupsViewModel viewModel, ApiResponse<BulkUpdateProgressModel> apiResponse)
        {
            if (apiResponse.Success)
            {
                viewModel.Result = apiResponse.GetResponse();
                if (viewModel.Result.IsProgressing())
                {
                    return View("InProgress");
                }
                else if (viewModel.Result.IsCompleted())
                {
                    if (viewModel.Result.Successful())
                    {
                        return View("Completed", viewModel);
                    }
                    else // FAILED
                    {
                        if (viewModel.Result.HasErrors)
                        {
                            AddApiErrorsToModelState(viewModel.Result.Errors);
                        }
                        else if (viewModel.Result.RowErrors > 0)
                        {
                            ModelState.AddModelError(ErrorLinkLog, "Please download the error log to correct your data before resubmitting");
                        }
                        else
                        {
                            try
                            {
                                var errorLogFileContent = await new System.Net.Http.HttpClient().GetStringAsync(viewModel.Result.ErrorLogFile.Url);
                                var lines = errorLogFileContent.Split('\n').Take(3);
                                if (lines.Any())
                                {
                                    lines.ForEach(x => ModelState.AddModelError(ErrorLinkFile, x));
                                }
                                else
                                {
                                    ModelState.AddModelError(ErrorLinkFile, "The request failed, but the API did not provide any details as to why.");
                                }
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError(ErrorLinkFile, "The request failed, but the API did not provide any details as to why");
                            }
                        }
                        return View(ViewName, viewModel);
                    }
                }
                else
                {
                    throw new Exception($"The status of task {id} is unclear; the API did not provide a good enough response {Newtonsoft.Json.JsonConvert.SerializeObject(viewModel.Result)}");
                }
            }
            else if (apiResponse.HasErrors)
            {
                AddApiErrorsToModelState(apiResponse.Errors, ErrorLinkFile);
                return View(ViewName, viewModel);
            }
            else
            {
                throw new Exception("ApiResponse indicated failure, but no errors were supplied");
            }
        }

        private ActionResult ResultInternalAjaxAsync(Guid id, BulkAssociateEstabs2GroupsViewModel viewModel, ApiResponse<BulkUpdateProgressModel> apiResponse)
        {
            if (apiResponse.Success)
            {
                viewModel.Result = apiResponse.GetResponse();

                return Json(JsonConvert.SerializeObject(new
                {
                    status = viewModel.Result.IsCompleted(), redirect = string.Concat("/Establishments/bulk-associate-estabs-to-groups/", id)
                }));
            }
            else if (apiResponse.HasErrors)
            {
                // tell the ajax that the request is complete and redirect so the error page loads
                return Json(JsonConvert.SerializeObject(new
                {
                    status = true, redirect = string.Concat("/Establishments/bulk-associate-estabs-to-groups/", id)
                }));
            }
            else
            {
                throw new Exception("ApiResponse indicated failure, but no errors were supplied");
            }
        }
    }
}
