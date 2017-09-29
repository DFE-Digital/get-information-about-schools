using System;
using System.Web.Mvc;
using Edubase.Services.Downloads;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Edubase.Web.UI.Models;
using Newtonsoft.Json;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Groups.Downloads;
using Edubase.Web.UI.Filters;
using Edubase.Common;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Downloads"), Route("{action=index}")]
    public class DownloadsController : Controller
    {
        private readonly IDownloadsService _downloadsService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupDownloadService _groupDownloadService;

        public DownloadsController(IDownloadsService downloadsService, IEstablishmentReadService establishmentReadService, IGroupDownloadService groupDownloadService)
        {
            _downloadsService = downloadsService;
            _establishmentReadService = establishmentReadService;
            _groupDownloadService = groupDownloadService;
        }
        
        public async Task<ActionResult> Index(int? startIndex)
        {
            const int pageSize = 100;
            var viewModel = new DownloadsViewModel
            {
                Downloads = await _downloadsService.GetListAsync(User),
                ScheduledExtracts = await _downloadsService.GetScheduledExtractsAsync((startIndex.GetValueOrDefault() / pageSize), pageSize, User),
                Skip = startIndex.GetValueOrDefault() / pageSize,
                Take = pageSize
            };

            return View(viewModel);
        }

        [Route("RequestScheduledExtract/{id}", Name = "RequestScheduledExtract")]
        public async Task<ActionResult> RequestScheduledExtract(int id)
        {
            var response = await _downloadsService.GenerateScheduledExtractAsync(id, User);

            if (response.Contains("fileLocationUri")) // Hack because the API sometimes returns ApiResultDto and sometimes ProgressDto!
            {
                return View("ReadyToDownloadScheduledExtract", JsonConvert.DeserializeObject<ProgressDto>(response));
            }
            else
            {
                return RedirectToAction(nameof(Download), new { id = JsonConvert.DeserializeObject<ApiResultDto<Guid>>(response).Value });
            }
        }

        [Route("Download/{id}", Name = "DownloadScheduledExtract")]
        public async Task<ActionResult> Download(Guid id)
        {
            var model = await _downloadsService.GetProgressOfScheduledExtractGenerationAsync(id, User);

            if (model.HasErrored)
                throw new Exception($"Download generation failed; Underlying error: '{model.Error}'");

            if (!model.IsComplete)
                return View("PreparingScheduledExtractPleaseWait", model);

            return View("ReadyToDownloadScheduledExtract", model);
        }

        [HttpGet, Route("Download/Establishment/{urn}", Name = "EstabDataDownload")]
        public async Task<ActionResult> DownloadEstablishmentData(int urn, string state, DownloadType? downloadType = null, bool start = false)
        {
            Guard.IsNotNull(state, () => new ArgumentNullException(nameof(state)));
            ViewBag.RouteName = "EstabDataDownload";
            ViewBag.BreadcrumbRoutes = UriHelper.DeserializeUrlToken<RouteDto[]>(state);
            if (downloadType.HasValue && !start) return View("Download");
            else if (downloadType.HasValue && start) return Redirect((await _establishmentReadService.GetDownloadAsync(urn, downloadType.Value, User)).Url);
            else return View("SelectFormat");
        }

        [HttpGet, Route("Download/Group/{uid}", Name = "GroupDataDownload")]
        public async Task<ActionResult> DownloadGroupData(int uid, string state, DownloadType? downloadType = null, bool start = false)
        {
            Guard.IsNotNull(state, () => new ArgumentNullException(nameof(state)));
            ViewBag.RouteName = "GroupDataDownload";
            ViewBag.BreadcrumbRoutes = UriHelper.DeserializeUrlToken<RouteDto[]>(state);
            if (downloadType.HasValue && !start) return View("Download");
            else if (downloadType.HasValue && start) return Redirect((await _groupDownloadService.DownloadGroupData(uid, downloadType.Value, User)).Url);
            else return View("SelectFormat");
        }
        
        [HttpGet, EdubaseAuthorize]
        [Route("Download/ChangeHistory/{downloadType}")]
        public async Task<ActionResult> DownloadChangeHistory(int? groupId, int? establishmentUrn, DownloadType downloadType)
        {
            if (groupId != null)
            {
                return Redirect((await _groupDownloadService.DownloadGroupHistory(groupId.Value, downloadType, User)).Url);
            }

            if (establishmentUrn != null)
            {
                return Redirect((await _establishmentReadService.GetChangeHistoryDownloadAsync(establishmentUrn.Value, downloadType, User)).Url);
            }

            return null;
        }
    }
}