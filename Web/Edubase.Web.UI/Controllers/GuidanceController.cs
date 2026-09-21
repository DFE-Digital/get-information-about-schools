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
        private readonly ISqlLaNameCodeRepository  _laNameCodeRepository;
        private readonly ILaNameCodeFileGenerator _fileGenerator;

        private static readonly Dictionary<string, string> GroupCodes = new Dictionary<string, string>
        {
            { "EnglishLaNameCodes", "english" }, { "WelshLaNameCodes", "welsh" }, { "OtherLaNameCodes", "other" }
        };

        public GuidanceController(IBlobService blobService,
            ISqlLaNameCodeRepository laNameCodeRepository,
            ILaNameCodeFileGenerator fileGenerator)
        {
            _blobService = blobService;
            _laNameCodeRepository = laNameCodeRepository;
            _fileGenerator = fileGenerator;
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
            if (viewModel.DownloadName == null ||
                !GroupCodes.TryGetValue(viewModel.DownloadName, out var groupCode) ||
                !viewModel.FileFormat.HasValue)
            {
                return View("Error");
            }

            var fileName = viewModel.DownloadName + "." + viewModel.FileFormat.ToString().ToLower();

            try
            {
                var entities = await _laNameCodeRepository.GetByGroupAsync(groupCode);

                var fileStream = _fileGenerator.Generate(Map(entities), viewModel.FileFormat.Value,
                    ToNameColumnHeader(viewModel.DownloadName));

                TempData["ArchivedBlob"] = await _blobService.ArchiveBlobAsync(fileStream, fileName);
                TempData["DownloadFileName"] = viewModel.DownloadName + ".zip";

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
                FileDownloadName = TempData["DownloadFileName"] as string ?? "Results.zip"
            };
        }

        private static List<LaNameCodes> MapByGroup(IEnumerable<SqlLaNameCode> source, string groupCode)
        {
            return Map(source.Where(x => x.GroupCode == groupCode));
        }

        private static List<LaNameCodes> Map(IEnumerable<SqlLaNameCode> source)
        {
            return source
                .Select(x => new LaNameCodes { LaName = x.LaName, LaCode = x.LaCode, OnsLaCode = x.GsLaCode })
                .ToList();
        }

        private static string ToNameColumnHeader(string downloadName)
        {
            return downloadName.Replace("LaNameCodes", "") + " local authority (LA) name";
        }
    }
}
