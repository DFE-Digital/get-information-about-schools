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
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Areas.Establishments.Controllers
{
    using R = Services.Security.EdubaseRoles;

    [RouteArea("Establishments"), MvcAuthorizeRoles(AuthorizedRoles.CanBulkCreateFreeSchools)]
    public class BulkCreateFreeSchoolsController : EduBaseController
    {
        const string ViewName = "BulkCreateFreeSchools";
        readonly IEstablishmentWriteService _establishmentWriteService;

        public BulkCreateFreeSchoolsController(IEstablishmentWriteService establishmentWriteService)
        {
            _establishmentWriteService = establishmentWriteService;
        }

        [Route("bulk-create-free-schools", Name = "BulkCreateFreeSchools"), HttpGet]
        public ActionResult Index()
        {
            return View(ViewName);
        }

        [Route("bulk-create-free-schools", Name = "BulkCreateFreeSchoolsPost"), HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ProcessBulkCreateFreeSchoolsAsync(BulkCreateFreeSchoolsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var fileName = FileHelper.GetTempFileName(Path.GetExtension(viewModel.BulkFile.FileName));
                viewModel.BulkFile.SaveAs(fileName);
                var result = await _establishmentWriteService.BulkCreateFreeSchoolsAsync(fileName, User);
                return result.HasErrors
                    ? await ResultInternalAsync(Guid.Empty, viewModel, result)
                    : RedirectToRoute("BulkCreateFreeSchoolsResult", new { id = result.GetResponse().Id });
            }
            return View(ViewName, viewModel);
        }

        [Route("bulk-create-free-schools/{id}", Name = "BulkCreateFreeSchoolsResult"), HttpGet]
        public async Task<ActionResult> ResultAsync(Guid id)
        {
            var viewModel = new BulkCreateFreeSchoolsViewModel();
            var apiResponse = await _establishmentWriteService.BulkCreateFreeSchoolsGetProgressAsync(id, User);
            return await ResultInternalAsync(id, viewModel, apiResponse);
        }

        [Route("bulk-create-free-schools-ajax/{id}", Name = "BulkCreateFreeSchoolsResultAjax"), HttpGet]
        public async Task<ActionResult> ResultAsyncAjax(Guid id)
        {
            var viewModel = new BulkCreateFreeSchoolsViewModel();
            var apiResponse = await _establishmentWriteService.BulkCreateFreeSchoolsGetProgressAsync(id, User);
            return ResultInternalAjax(id, viewModel, apiResponse);
        }

        private async Task<ActionResult> ResultInternalAsync(Guid id, BulkCreateFreeSchoolsViewModel viewModel, ApiResponse<BulkCreateFreeSchoolsResult> apiResponse)
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
                    if (viewModel.Result.HasCreatedEstablishments) // NOTE: You _ONLY_ ever get HasCreatedEstablishments==true, when the ENTIRE operation has succeeded.
                    {
                        return View("Completed", viewModel);
                    }
                    else // no establishments have been created. Show the main upload page with the error detail.
                    {
                        if (viewModel.Result.HasErrors)
                        {
                            AddApiErrorsToModelState(viewModel.Result.Errors);
                        }
                        else if (viewModel.Result.RowErrors > 0)
                        {
                            ModelState.AddModelError("error-log", "Please download the error log to correct your data before resubmitting");
                        }
                        else
                        {
                            try
                            {
                                var errorLogFileContent = await new System.Net.Http.HttpClient().GetStringAsync(viewModel.Result.ErrorLogFile.Url);
                                var lines = errorLogFileContent.Split('\n').Take(3);
                                if (lines.Any())
                                {
                                    lines.ForEach(x => ModelState.AddModelError(string.Empty, x));
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, "The request failed, but the API did not provide any details as to why.");
                                }
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError(string.Empty, "The request failed, but the API did not provide any details as to why");
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
                AddApiErrorsToModelState(apiResponse.Errors, nameof(viewModel.BulkFile));
                return View(ViewName, viewModel);
            }
            else
            {
                throw new Exception("ApiResponse indicated failure, but no errors were supplied");
            }
        }

        private ActionResult ResultInternalAjax(Guid id, BulkCreateFreeSchoolsViewModel viewModel, ApiResponse<BulkCreateFreeSchoolsResult> apiResponse)
        {
            var redirectUrl = string.Concat("/Establishments/bulk-create-free-schools/", id);
            if (apiResponse.Success)
            {
                viewModel.Result = apiResponse.GetResponse();
                return viewModel.Result.IsProgressing()
                    ? Json(JsonConvert.SerializeObject(new
                    {
                        status = false, redirect = redirectUrl
                    }))
                    : viewModel.Result.IsCompleted()
                        ? Json(JsonConvert.SerializeObject(new
                        {
                            status = true,
                            redirect = redirectUrl
                        }))
                        : throw new Exception($"The status of task {id} is unclear; the API did not provide a good enough response {Newtonsoft.Json.JsonConvert.SerializeObject(viewModel.Result)}");
            }
            else
            {
                return apiResponse.HasErrors
                    ? Json(JsonConvert.SerializeObject(new
                    {
                        status = true,
                        redirect = redirectUrl
                    }))
                    : throw new Exception("ApiResponse indicated failure, but no errors were supplied");
            }
        }
    }
}
