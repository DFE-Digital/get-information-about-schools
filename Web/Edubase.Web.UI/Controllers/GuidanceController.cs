using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CsvHelper;
using CsvHelper.Configuration;
using Edubase.Services;
using Edubase.Web.UI.Controllers.Api;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Guidance;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Guidance"), Route("{action=index}")]
    public class GuidanceController : EduBaseController
    {
        private readonly IBlobService _blobService;
        private readonly ISqlLaNameCodeRepository _laNameCodeRepository;
        private const string GUIDANCE_CONTAINER = "guidance";
        private const string ENGLISH_LA_NAME_CODES = "EnglishLaNameCodes.csv";
        private const string WELSH_LA_NAME_CODES = "WelshLaNameCodes.csv";
        private const string OTHER_LA_NAME_CODES = "OtherLaNameCodes.csv";

        private static readonly Dictionary<string, string> GroupCodes = new Dictionary<string, string>
        {
            { "EnglishLaNameCodes", "english" }, { "WelshLaNameCodes", "welsh" }, { "OtherLaNameCodes", "other" }
        };

        public GuidanceController(IBlobService blobService, ISqlLaNameCodeRepository laNameCodeRepository)
        {
            _blobService = blobService;
            _laNameCodeRepository = laNameCodeRepository;
        }

        [Route(Name = "Guidance")]
        public ActionResult Index() => View();
        public ActionResult General() => View();
        public ActionResult EstablishmentBulkUpdate() => View();
        public ActionResult ChildrensCentre() => View();
        public ActionResult Federation() => View();
        public ActionResult Governance() => View();

        public async Task<ActionResult> LaNameCodes()
        {
            var all = await _laNameCodeRepository.GetAllAsync();

            return View(new GuidanceLaNameCodeViewModel()
            {
                EnglishLas = MapByGroup(all, GroupCodes["EnglishLaNameCodes"]),
                WelshLas = MapByGroup(all, GroupCodes["WelshLaNameCodes"]),
                OtherLas = MapByGroup(all, GroupCodes["OtherLaNameCodes"])
            });
        }

        [Route("LaNameCodes/DataTables", Name = "LaNameCodesSelectData")]
        public ActionResult LaNameCodesSelectData(GuidanceLaNameCodeViewModel viewModel)
        {
            return View("SelectData", viewModel);
        }

        [Route("LaNameCodes/DataTables/SelectFormat", Name = "LaNameCodesSelectFormat")]
        public ActionResult LaNameCodesSelectFormat(GuidanceLaNameCodeViewModel viewModel)
        {
            return View("SelectFormat", viewModel);
        }

        [Route("LaNameCodes/DataTables/SelectFormat/GenerateDownload", Name = "LaNameCodesGenerateDownload"), ValidateAntiForgeryToken]
        public async Task<ActionResult> LaNameCodesGenerateDownload(GuidanceLaNameCodeViewModel viewModel)
        {
            var blobName = viewModel.DownloadName + "." + viewModel.FileFormat.ToString().ToLower();

            var memoryStream = new MemoryStream();

            try
            {
                var blob = _blobService.GetBlobReference(GUIDANCE_CONTAINER, blobName);

                blob.DownloadToStreamAsync(memoryStream).GetAwaiter().GetResult();
                memoryStream.Position = 0;

                TempData["ArchivedBlob"] = await _blobService.ArchiveBlobAsync(memoryStream, blobName);

                return View("ReadyToDownload");

            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [Route("LaNameCodes/DataTables/SelectFormat/GenerateDownload/Download", Name = "LaNameCodesDownload")]
        public ActionResult LaNameCodesDownload()
        {
            return new FileStreamResult((MemoryStream) TempData["ArchivedBlob"], "application/octet-stream")
            {
                FileDownloadName = "Results.zip"
            };
        }

        private static List<LaNameCodes> MapByGroup(IEnumerable<SqlLaNameCode> source, string groupCode)
        {
            return source
                .Where(x => x.GroupCode == groupCode)
                .Select(x => new LaNameCodes { LaName = x.LaName, LaCode = x.LaCode, OnsLaCode = x.GsLaCode })
                .ToList();
        }
    }
}

