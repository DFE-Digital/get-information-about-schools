using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CsvHelper;
using CsvHelper.Configuration;
using Edubase.Services;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models.Guidance;
using Glimpse.Mvc.Model;
using Microsoft.Data.Edm.Csdl;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Guidance"), Route("{action=index}")]
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

        [Route(Name = "Guidance")]
        public ActionResult Index() => View();
        public ActionResult General() => View();
        public ActionResult EstablishmentBulkUpdate() => View();
        public ActionResult ChildrensCentre() => View();
        public ActionResult Federation() => View();
        public ActionResult Governance() => View();

        public async Task<ActionResult> LaNameCodes()
        {
            return View(new GuidanceLaNameCodeViewModel()
            {
                EnglishLas = await GetCsvFromContainer(GUIDANCE_CONTAINER, ENGLISH_LA_NAME_CODES),
                WelshLas = await GetCsvFromContainer(GUIDANCE_CONTAINER, WELSH_LA_NAME_CODES),
                OtherLas = await GetCsvFromContainer(GUIDANCE_CONTAINER, OTHER_LA_NAME_CODES),
            });
        }

        [Route("LaNameCodes/DataTables", Name = "SelectData")]
        public ActionResult SelectData(GuidanceLaNameCodeViewModel viewModel)
        {
            return View("SelectData", viewModel);
        }

        [Route("LaNameCodes/DataTables/SelectFormat", Name = "LaNameCodesSelectFormat")]
        public ActionResult LaNameCodesSelectFormat(GuidanceLaNameCodeViewModel viewModel)
        {
            return View("SelectFormat", viewModel);
        }

        [Route("LaNameCodes/DataTables/SelectFormat/GenerateDownload", Name = "LaNameCodesGenerateDownload")]
        public async Task<ActionResult> LaNameCodesGenerateDownload(GuidanceLaNameCodeViewModel viewModel)
        {
            var blobName = viewModel.DownloadName + "." + viewModel.FileFormat.ToString().ToLower();

            var memoryStream = new MemoryStream();

            try
            {
               // var blobStream = await _blobService.GetBlobAsStreamAsync("/" + GUIDANCE_CONTAINER + "/" + blob);

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
    

        private async Task<List<LaNameCodes>> GetCsvFromContainer(string container, string file)
        {
            var blob = _blobService.GetBlobReference(container, file);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
               HasHeaderRecord = false,          
            };

            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStreamAsync(memoryStream).GetAwaiter().GetResult();
                memoryStream.Position = 0;
                using (var reader = new StreamReader(memoryStream))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Read();
                    var records = csv.GetRecords<LaNameCodes>().ToList();

                    return records;
                }
            }
        }
    }
}

