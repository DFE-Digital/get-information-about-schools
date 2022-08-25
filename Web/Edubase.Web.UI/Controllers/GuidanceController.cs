using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CsvHelper;
using Edubase.Services;
using Edubase.Web.UI.Models.Guidance;
using Microsoft.Data.Edm.Csdl;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Guidance"), Route("{action=index}")]
    public class GuidanceController : EduBaseController
    {
        private readonly IBlobService _blobService;

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
        // public ActionResult LaNameCodes() => View();

        public async Task<ActionResult> LaNameCodes()
        {
            var viewModel = new GuidanceLaNameCodeViewModel();

            //populate viewmodel with data from blob
            var data = await GetCsvFromContainer("guidance", "EnglishLaNameCodes.csv");
            using (var streamReader = new StreamReader(data))
            using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
            {
                var records = csvReader.GetRecords<LaNameCodes>().ToList();
            }

            return View();
        }

        private async Task<string> GetCsvFromContainer(string container, string file)
        {
            var blob = _blobService.GetBlobReference(container, file);
            if (await blob.ExistsAsync())
            {
                return blob.DownloadTextAsync().Result;
                // return contents;
                //var stream = await blob.OpenReadAsync();
                //return new FileStreamResult(stream, blob.Properties.ContentType)
                //{
                //    FileDownloadName = blob.Name
                //};
            }
            throw new Exception("File not available");
        }

        private async Task<ActionResult> GetFileFromContainer(string container, string file)
        {
            var blob = _blobService.GetBlobReference(container, file);
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
    }
}

