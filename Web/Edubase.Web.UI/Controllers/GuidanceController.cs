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
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models.Guidance;
using Microsoft.Data.Edm.Csdl;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Guidance"), Route("{action=index}")]
    public class GuidanceController : EduBaseController
    {
        private readonly IBlobService _blobService;
        private const string GUIDANCE = "guidance";
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
                EnglishLas = await GetCsvFromContainer(GUIDANCE, ENGLISH_LA_NAME_CODES),
                WelshLas = await GetCsvFromContainer(GUIDANCE, WELSH_LA_NAME_CODES),
                OtherLas = await GetCsvFromContainer(GUIDANCE, OTHER_LA_NAME_CODES),
            });
        }

        [HttpPost, Route("LaNameCodes", Name = "LaNameCodesDownload")]
        public async Task<ActionResult> LaNameCodesDownload(GuidanceLaNameCodeViewModel viewModel)
        {
            if (!viewModel.FileFormat.HasValue)
            {
                return View("SelectFormat", viewModel);
            }

            var file = viewModel.DownloadType + "." + viewModel.FileFormat.ToString().ToLower();

            var blob = _blobService.GetBlobReference(GUIDANCE, file);
            if (await blob.ExistsAsync())
            {
                var stream = await blob.OpenReadAsync();
                return new FileStreamResult(stream, blob.Properties.ContentType)
                {
                    FileDownloadName = blob.Name
                };
            }
            throw new Exception("File not available");
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

