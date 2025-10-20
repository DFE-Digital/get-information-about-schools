using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Edubase.Services;
using Edubase.Web.UI.Models.Guidance;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [ApiController]
    [Route("guidance")]
    public class GuidanceController : EduBaseController
    {
        private readonly IBlobService _blobService;
        private const string GUIDANCE_CONTAINER = "guidance";
        private const string ENGLISH_LA_NAME_CODES = "EnglishLaNameCodes.csv";
        private const string WELSH_LA_NAME_CODES = "WelshLaNameCodes.csv";
        private const string OTHER_LA_NAME_CODES = "OtherLaNameCodes.csv";

        public GuidanceController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet("", Name = "Guidance")]
        public IActionResult Index() => View();

        [HttpGet("general")]
        public IActionResult General() => View();

        [HttpGet("establishment-bulk-update")]
        public IActionResult EstablishmentBulkUpdate() => View();

        [HttpGet("childrens-centre")]
        public IActionResult ChildrensCentre() => View();

        [HttpGet("federation")]
        public IActionResult Federation() => View();

        [HttpGet("governance")]
        public IActionResult Governance() => View();

        [HttpGet("la-name-codes")]
        public async Task<IActionResult> LaNameCodes()
        {
            var model = new GuidanceLaNameCodeViewModel
            {
                EnglishLas = await GetCsvFromContainer(GUIDANCE_CONTAINER, ENGLISH_LA_NAME_CODES),
                WelshLas = await GetCsvFromContainer(GUIDANCE_CONTAINER, WELSH_LA_NAME_CODES),
                OtherLas = await GetCsvFromContainer(GUIDANCE_CONTAINER, OTHER_LA_NAME_CODES),
            };
            return View(model);
        }

        [HttpGet("la-name-codes/data-tables", Name = "LaNameCodesSelectData")]
        public IActionResult LaNameCodesSelectData(GuidanceLaNameCodeViewModel viewModel)
        {
            return View("SelectData", viewModel);
        }

        [HttpGet("la-name-codes/data-tables/select-format", Name = "LaNameCodesSelectFormat")]
        public IActionResult LaNameCodesSelectFormat(GuidanceLaNameCodeViewModel viewModel)
        {
            return View("SelectFormat", viewModel);
        }

        [HttpPost("la-name-codes/data-tables/select-format/generate-download", Name = "LaNameCodesGenerateDownload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LaNameCodesGenerateDownload(GuidanceLaNameCodeViewModel viewModel)
        {
            var blobName = $"{viewModel.DownloadName}.{viewModel.FileFormat.ToString().ToLower()}";
            var memoryStream = new MemoryStream();

            try
            {
                var blob = _blobService.GetBlobReference(GUIDANCE_CONTAINER, blobName);
                await blob.DownloadToStreamAsync(memoryStream);
                memoryStream.Position = 0;

                TempData["ArchivedBlob"] = await _blobService.ArchiveBlobAsync(memoryStream, blobName);
                return View("ReadyToDownload");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpGet("la-name-codes/data-tables/select-format/generate-download/download", Name = "LaNameCodesDownload")]
        public IActionResult LaNameCodesDownload()
        {
            var stream = TempData["ArchivedBlob"] as MemoryStream;
            if (stream == null) return View("Error");

            return new FileStreamResult(stream, "application/octet-stream")
            {
                FileDownloadName = "Results.zip"
            };
        }

        private async Task<List<LaNameCodes>> GetCsvFromContainer(string container, string file)
        {
            var blob = _blobService.GetBlobReference(container, file);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

            using var memoryStream = new MemoryStream();
            await blob.DownloadToStreamAsync(memoryStream);
            memoryStream.Position = 0;

            using var reader = new StreamReader(memoryStream);
            using var csv = new CsvReader(reader, config);
            csv.Read();
            return csv.GetRecords<LaNameCodes>().ToList();
        }
    }
}
