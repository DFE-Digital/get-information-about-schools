using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Services;
using Edubase.Web.UI.Controllers.Api;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Guidance;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [Route("Guidance")]
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

        [HttpGet("", Name = "Guidance")]
        [HttpGet("Index")]
        public ActionResult Index() => View();

        [HttpGet("General")]
        public ActionResult General() => View();

        [HttpGet("EstablishmentBulkUpdate")]
        public ActionResult EstablishmentBulkUpdate() => View();

        [HttpGet("ChildrensCentre")]
        public ActionResult ChildrensCentre() => View();

        [HttpGet("Federation")]
        public ActionResult Federation() => View();

        [HttpGet("Governance")]
        public ActionResult Governance() => View();

        [HttpGet("LaNameCodes")]
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

        [HttpGet("LaNameCodes/DataTables", Name = "LaNameCodesSelectData")]
        public ActionResult LaNameCodesSelectData(GuidanceLaNameCodeViewModel viewModel)
        {
            return View("SelectData", viewModel);
        }

        [HttpGet("LaNameCodes/DataTables/SelectFormat", Name = "LaNameCodesSelectFormat")]
        public ActionResult LaNameCodesSelectFormat(GuidanceLaNameCodeViewModel viewModel)
        {
            return View("SelectFormat", viewModel);
        }

        [HttpPost("LaNameCodes/DataTables/SelectFormat/GenerateDownload", Name = "LaNameCodesGenerateDownload"), ValidateAntiForgeryToken]
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

                using (var fileStream = _fileGenerator.Generate(Map(entities), viewModel.FileFormat.Value,
                           ToNameColumnHeader(viewModel.DownloadName), viewModel.DownloadName))
                {
                    TempData["ArchivedBlob"] = await _blobService.ArchiveBlobAsync(fileStream, fileName);
                }

                return View("ReadyToDownload");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpGet("LaNameCodes/DataTables/SelectFormat/GenerateDownload/Download", Name = "LaNameCodesDownload")]
        public ActionResult LaNameCodesDownload()
        {
            return new FileStreamResult((MemoryStream) TempData["ArchivedBlob"], "application/octet-stream")
            {
                FileDownloadName = "Results.zip"
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
