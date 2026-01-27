using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Services;
using Edubase.Services.Domain;
using Edubase.Services.Downloads;
using Edubase.Services.Downloads.Models;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Groups.Downloads;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace Edubase.Web.UI.Controllers
{
    [Route("Downloads")]
    public class DownloadsController : Controller
    {
        private readonly IDownloadsService _downloadsService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupDownloadService _groupDownloadService;

        public DownloadsController(IDownloadsService downloadsService,
            IEstablishmentReadService establishmentReadService, IGroupDownloadService groupDownloadService)
        {
            _downloadsService = downloadsService;
            _establishmentReadService = establishmentReadService;
            _groupDownloadService = groupDownloadService;
        }

        public async Task<ActionResult> Index(int? skip, DateTimeViewModel filterDate,
            eDownloadFilter searchType = eDownloadFilter.Latest)
        {
            var viewModel = await GetDownloads(skip, filterDate, searchType);
            return View(viewModel);
        }

        [HttpGet, Route("results-js")]
        public async Task<PartialViewResult> ResultsPartial(DownloadsViewModel model)
        {
            var viewModel = await GetDownloads(model.Skip, model.FilterDate, model.SearchType);
            return PartialView("Partials/_DownloadsResults", viewModel);
        }

        private async Task<DownloadsViewModel> GetDownloads(int? skip, DateTimeViewModel filterDate,
            eDownloadFilter searchType = eDownloadFilter.Latest)
        {
            var dateLookup = searchType == eDownloadFilter.Latest ? new DateTimeViewModel(DateTime.Today) :
                filterDate.IsValid() ? filterDate : new DateTimeViewModel(DateTime.Today);

            var viewModel = new DownloadsViewModel
            {
                Downloads = await _downloadsService.GetListAsync(dateLookup.ToDateTime() ?? DateTime.Today, User),
                ScheduledExtracts =
                    await _downloadsService.GetScheduledExtractsAsync(skip.GetValueOrDefault(), 100, User),
                FilterDate = dateLookup,
                SearchType = searchType,
                Skip = skip
            };

            return viewModel;
        }

        private Guid getIdFromFileLocationUri(ProgressDto response)
        {
            var uri = new Uri(response.FileLocationUri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            return Guid.Parse(query["id"]);
        }

        [Route("Collate", Name = "CollateDownloads"), ValidateAntiForgeryToken]
        //assume this is a post??
        public async Task<ActionResult> CollateDownloads(DownloadsViewModel model)
        {
            var collection = new List<FileDownloadRequest>();
            foreach (var fileDownload in model.Downloads.Where(x => x.Selected))
            {
                collection.Add(new FileDownloadRequest(fileDownload.Tag, fileDownload.FileGeneratedDate));
            }

            if (!collection.Any())
            {
                var routeValuesDictionary = new RouteValueDictionary
                {
                    { nameof(model.Skip), model.Skip },
                    { $"{nameof(model.FilterDate)}.{nameof(model.FilterDate.Day)}", model.FilterDate.Day },
                    { $"{nameof(model.FilterDate)}.{nameof(model.FilterDate.Month)}", model.FilterDate.Month },
                    { $"{nameof(model.FilterDate)}.{nameof(model.FilterDate.Year)}", model.FilterDate.Year },
                    { nameof(model.SearchType), model.SearchType },
                };
                return RedirectToAction(nameof(Index), routeValuesDictionary);
            }

            var response = await _downloadsService.CollateDownloadsAsync(collection, User);
            if (response.Contains(
                    "fileLocationUri")) // Hack because the API sometimes returns ApiResultDto and sometimes ProgressDto!
            {
                return RedirectToAction(nameof(DownloadGenerated),
                    new
                    {
                        id = getIdFromFileLocationUri(JsonConvert.DeserializeObject<ProgressDto>(response)),
                        isExtract = true
                    });
            }
            else
            {
                return RedirectToAction(nameof(DownloadGenerated),
                    new { id = JsonConvert.DeserializeObject<ApiResultDto<Guid>>(response).Value });
            }
        }

        [Route("Generate/{id}", Name = "GenerateDownload")]
        public async Task<ActionResult> GenerateDownload(string id)
        {
            if (!Guid.TryParse(id, out _))
            {
                return InvalidGuidResult(isExtract: false);
            }

            string? response = await TryGenerateExtractAsync(id);
            if (string.IsNullOrWhiteSpace(response))
            {
                return InvalidGuidResult(isExtract: false);
            }

            // API sometimes returns ProgressDto and sometimes ApiResultDto<Guid>
            if (response.Contains("fileLocationUri", StringComparison.OrdinalIgnoreCase))
            {
                var progress = JsonConvert.DeserializeObject<ProgressDto>(response);
                var generatedId = getIdFromFileLocationUri(progress);

                return RedirectToAction(
                    nameof(DownloadGenerated),
                    new { id = generatedId, isExtract = true });
            }

            var apiResult = JsonConvert.DeserializeObject<ApiResultDto<Guid>>(response);

            return RedirectToAction(
                nameof(DownloadGenerated),
                new { id = apiResult.Value });
        }

        private async Task<string?> TryGenerateExtractAsync(string id)
        {
            try
            {
                return await _downloadsService.GenerateExtractAsync(id, User);
            }
            catch (Exception ex)
            {
                // The API uses exception messages to signal known failure states
                if (ex.Message.StartsWith("The API returned 404 Not Found", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.StartsWith("The API returned an 'Internal Server Error'", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                throw; // Unexpected error — let it bubble up
            }
        }

        [Route("Generated/{id}", Name = "DownloadGenerated")]
        public async Task<ActionResult> DownloadGenerated(string id, bool isExtract = false)
        {
            if (!Guid.TryParse(id, out Guid parsedId))
            {
                return InvalidGuidResult(isExtract);
            }

            var model = new ProgressDto();
            try
            {
                model = isExtract
                    ? await _downloadsService.GetProgressOfScheduledExtractGenerationAsync(parsedId, User)
                    : await _downloadsService.GetProgressOfGeneratedExtractAsync(parsedId, User);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("The API returned 404 Not Found") ||
                    ex.Message.StartsWith("The API returned an 'Internal Server Error'"))
                {
                    // if the file no longer exists (user refreshes the page post download etc) then the api returns a 404 and throws an error. This allows for a more graceful response
                    model.Error = "Download process not found for associated id";
                }
                else
                {
                    throw new Exception($"Download generation failed; Underlying error: '{model.Error}'");
                }
            }

            if (model.HasErrored)
                return View("Downloads/DownloadError",
                    new DownloadErrorViewModel
                    {
                        ReturnSource = isExtract ? eDownloadReturnSource.Extracts : eDownloadReturnSource.Downloads,
                        NeedsRegenerating = true
                    });

            ViewBag.isExtract = isExtract;
            if (!model.IsComplete)
                return View("PreparingFilePleaseWait", model);

            return View("ReadyToDownload", model);
        }

        /// <summary>
        /// Generates a standard error response when a supplied identifier
        /// cannot be parsed into a valid <see cref="Guid"/>.
        /// </summary>
        /// <param name="isExtract">
        /// Indicates whether the request originated from the extracts area
        /// or the general downloads area. This determines the value of
        /// <see cref="eDownloadReturnSource"/> used in the returned view model.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> rendering the download error view,
        /// populated with a <see cref="DownloadErrorViewModel"/> describing
        /// the failure and the appropriate return source.
        /// </returns>
        private ViewResult InvalidGuidResult(bool isExtract)
        {
            return View("Downloads/DownloadError", new DownloadErrorViewModel
            {
                NeedsRegenerating = false,
                ReturnSource = isExtract
                    ? eDownloadReturnSource.Extracts
                    : eDownloadReturnSource.Downloads
            });
        }

        [Route("RequestScheduledExtract/{eid}", Name = "RequestScheduledExtract")]
        public async Task<ActionResult> RequestScheduledExtract(int eid)
        {
            string response = null;
            try
            {
                response = await _downloadsService.GenerateScheduledExtractAsync(eid, User);
                
                if (response.Contains("\"code\"") && response.Contains("\"message\""))
                {
                    var apiError = JsonConvert.DeserializeObject<ApiWarning>(response);
                    throw new InvalidOperationException($"{apiError.Message} (Code: {apiError.Code})");
                }

                if (response.Contains(
                        "fileLocationUri")) // Hack because the API sometimes returns ApiResultDto and sometimes ProgressDto!
                {
                    var progressDto = JsonConvert.DeserializeObject<ProgressDto>(response);
                    return RedirectToAction(nameof(DownloadGenerated), new { id = getIdFromFileLocationUri(progressDto), isExtract = true });
                }

                var apiResult = JsonConvert.DeserializeObject<ApiResultDto<Guid>>(response);
                return RedirectToAction(nameof(DownloadGenerated), new { id = apiResult.Value });
            }
            catch (InvalidOperationException ex)
            {
                return HandleDownloadError(
                    ex,
                    "Request Error",
                    "An extract is already being generated for this request.",
                    "Please wait a few minutes for the extract to complete. If nothing happens after that time, please try again.");
            }
            catch (Exception ex)
            {
                return HandleDownloadError(
                    ex,
                    "Unknown Error",
                    "We could not generate your download due to a system issue",
                    "Please try again shortly or contact support.");
            }
        }

        private ActionResult HandleDownloadError(Exception ex, string errorType, string userMessage, string nextSteps)
        {
            var errorVm = new DownloadErrorViewModel
            {
                NeedsRegenerating = false,
                FriendlyMessage = true,
                ReturnSource = eDownloadReturnSource.Extracts,
                ErrorMessage = userMessage,
                ErrorType = errorType,
                NextSteps = nextSteps
            };
            return View("Downloads/DownloadError", errorVm);
        }

        [HttpGet, Route("Download/Establishment/{urn}", Name = "EstabDataDownload")]
        public async Task<ActionResult> DownloadEstablishmentData(int urn, string state,
            DownloadType? downloadType = null, bool start = false)
        {
            state.AssertIsNotEmpty(nameof(state));
            ViewBag.RouteName = "EstabDataDownload";
            ViewBag.BreadcrumbRoutes = UriHelper.DeserializeUrlToken<RouteDto[]>(state);
            if (downloadType.HasValue && !start) return View("Download");
            else if (downloadType.HasValue && start)
                return Redirect((await _establishmentReadService.GetDownloadAsync(urn, downloadType.Value, User)).Url);
            else return View("SelectFormat");
        }

        [HttpGet, Route("Download/Group/{uid}", Name = "GroupDataDownload")]
        public async Task<ActionResult> DownloadGroupData(int uid, string state, DownloadType? downloadType = null,
            bool start = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(state))
                {
                    return new StatusCodeResult((int) 400);
                }

                ViewBag.RouteName = "GroupDataDownload";

                try
                {
                    ViewBag.BreadcrumbRoutes = UriHelper.DeserializeUrlToken<RouteDto[]>(state);
                }
                catch (FormatException ex)
                {
                    return new StatusCodeResult((int) 400);
                }

                if (downloadType.HasValue && !start) return View("Download");
                else if (downloadType.HasValue && start)
                    return Redirect((await _groupDownloadService.DownloadGroupData(uid, downloadType.Value, User)).Url);
                else return View("SelectFormat");
            }
            catch (Exception ex)
            {
                return new StatusCodeResult((int) 500);
            }
        }

        [HttpGet]
        [Authorize(Policy = "EdubasePolicy")]
        [Route("Download/ChangeHistory/{downloadType}")]
        public async Task<ActionResult> DownloadChangeHistory(int? groupId, int? establishmentUrn, DownloadType downloadType)
        {
            if (groupId != null)
            {
                return Redirect((await _groupDownloadService.DownloadGroupHistory(groupId.Value, downloadType, null, null, null, User)).Url);
            }

            if (establishmentUrn != null)
            {
                return Redirect((await _establishmentReadService.GetChangeHistoryDownloadAsync(establishmentUrn.Value, downloadType, User)).Url);
            }

            return null;
        }

        [HttpGet]
        [Authorize(Policy = "EdubasePolicy")]
        [Route("Download/Establishment/{id}/{downloadType}", Name = "DownloadEstablishmentGovernanceChangeHistory")]
        public async Task<ActionResult> DownloadGovernanceChangeHistoryAsync(int id, DownloadType downloadType)
            => Redirect((await _establishmentReadService.GetGovernanceChangeHistoryDownloadAsync(id, downloadType, User)).Url);

        [HttpGet]
        [Authorize(Policy = "EdubasePolicy")]
        [Route("Download/Group/{id}/Governance/{downloadType}", Name = "DownloadGroupGovernanceChangeHistory")]
        public async Task<ActionResult> DownloadGroupGovernanceChangeHistoryAsync(int id, DownloadType downloadType)
            => Redirect((await _groupDownloadService.GetGovernanceChangeHistoryDownloadAsync(id, downloadType, User)).Url);

        [HttpGet]
        [Route("Download/File", Name = "DownloadFile")]
        public async Task<ActionResult> DownloadFileAsync(string id, DateTime? filterDate)
        {
            var file = (await _downloadsService.GetListAsync(filterDate ?? DateTime.Today, User)).FirstOrDefault(x => x.Tag == id);
            if (file != null)
            {
                var response = await _downloadsService.DownloadFile(file, User);

                return new FileStreamResult(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType.MediaType)
                {
                    FileDownloadName = response.Content.Headers.ContentDisposition.FileName.RemoveSubstring("\"")
                };
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost, Route("Download/Extract", Name = "DownloadExtract"), ValidateAntiForgeryToken]
        public async Task<ActionResult> DownloadExtractAsync(string path, string id, string searchQueryString = null,
            eLookupSearchSource? searchSource = null, eDownloadReturnSource? returnSource = null)
        {
            var uri = new Uri(path);
            var downloadAvailable = await _downloadsService.IsDownloadAvailable($"/{uri.Segments.Last()}", id, User);

            if (downloadAvailable)
            {
                return Redirect($"{path}?id={id}");
            }
            else
            {
                var view = new DownloadErrorViewModel
                {
                    SearchQueryString = searchQueryString,
                    SearchSource = searchSource,
                    ReturnSource = returnSource
                };
                return View("Downloads/DownloadError", view);
            }
        }

        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanManageAcademyTrusts)]
        [Authorize(Policy = "EdubasePolicy")]
        [Route("Download/MATClosureReport", Name = "DownloadMATClosureReport")]
        public async Task<ActionResult> DownloadMATClosureReportAsync(string filename)
        {
            var response = await _downloadsService.DownloadMATClosureReport(User);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }
            else
            {
                response.EnsureSuccessStatusCode();
                return new FileStreamResult(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType.MediaType) { FileDownloadName = filename };
            }
        }

    }
}
