using Edubase.Common.Cache;
using Edubase.Common.IO;
using Edubase.Services.Domain;
using Ionic.Zip;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;

namespace Edubase.Services.Core
{
    /// <summary>
    /// Responsible for the creation of file downloads and publication of them.
    /// </summary>
    public class FileDownloadFactoryService : IFileDownloadFactoryService
    {
        ICacheAccessor _cacheAccessor;
        IBlobService _blobService;

        public enum eFileFormat
        {
            CSV,
            XLSX
        }

        public FileDownloadFactoryService(ICacheAccessor cacheAccessor, IBlobService blobService)
        {
            _cacheAccessor = cacheAccessor;
            _blobService = blobService;
        }

        public async Task<SearchDownloadGenerationProgressDto> SearchWithDownloadGeneration_InitialiseAsync()
        {
            var progress = new SearchDownloadGenerationProgressDto(Guid.NewGuid());
            await _cacheAccessor.SetAsync(progress.Id.ToString(), progress, TimeSpan.FromHours(12));
            return progress;
        }

        protected string CreateZipFile(string fileName)
        {
            var zipFileName = FileHelper.GetTempFileName("zip");
            using (var zip = new ZipFile(zipFileName))
            {
                zip.AddFile(fileName, string.Empty);
                zip.Save();
            }

            return zipFileName;
        }

        protected async Task<string> UploadFileToBlobStorage(string zipFileName)
        {
            var blobFileName = _blobService.CreateRandomBlobName("zip");
            var blobLocation = "/downloads/" + blobFileName;
            await _blobService.UploadAsync(zipFileName, new FileHelper().GetMimeType(zipFileName), blobLocation, "edubase-search-results.zip");
            var url = _blobService.GetReadOnlySharedAccessUrl("downloads", blobFileName, DateTimeOffset.UtcNow.AddDays(7)); // TODO: add an auto-delete facility.
            return url;
        }

        public async Task<SearchDownloadGenerationProgressDto> GetDownloadGenerationProgressAsync(Guid taskId)
        {
            var retVal = await _cacheAccessor.GetAsync<SearchDownloadGenerationProgressDto>(taskId.ToString());
            if (retVal == null) throw new Exception($"Search download has not been initialised; ensure {nameof(SearchWithDownloadGeneration_InitialiseAsync)} is called first.");
            else return retVal;
        }

        protected string ToCsv(List<string> items) => string.Join(",", items.Select(x => $@"""{x}"""));

        protected string ToFileExtension(eFileFormat f) => f == eFileFormat.CSV ? ".csv" : ".xlsx";


        /// <summary>
        /// Creates a CSV file as a memory stream
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public async Task<MemoryStream> CreateCsvStreamAsync(List<string> headers, List<List<string>> rows)
        {
            var retVal = new MemoryStream();
            using (var streamWriter = new StreamWriter(retVal, Encoding.UTF8, 4096, true))
            {
                await streamWriter.WriteLineAsync(ToCsv(headers));
                foreach (var item in rows)
                {
                    var csv = ToCsv(item);
                    await streamWriter.WriteLineAsync(csv);
                }
            }

            retVal.Seek(0, SeekOrigin.Begin);
            return retVal;
        }

        /// <summary>
        /// Creates an XLSX file from the data supplied and returns a memory stream
        /// </summary>
        /// <param name="title"></param>
        /// <param name="headers"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public MemoryStream CreateXlsxStream(string title, string worksheetTitle, List<string> headers, List<List<string>> rows)
        {
            using (var xlsx = new ExcelPackage(new MemoryStream()))
            {
                xlsx.Workbook.Properties.Author = "Department for Education";
                xlsx.Workbook.Properties.Title = title;

                var cursor = 1;
                var sheet = xlsx.Workbook.Worksheets.Add(worksheetTitle);
                headers.ForEach((header, index) =>
                {
                    var cell = sheet.Cells[cursor, (index + 1)];
                    cell.Value = header;
                    cell.Style.Font.Bold = true;
                });
                cursor++;

                foreach (var item in rows)
                {
                    item.ForEach((columnValue, index) => sheet.Cells[cursor, (index + 1)].Value = columnValue);
                    cursor++;
                }

                var retVal = new MemoryStream();
                xlsx.SaveAs(retVal);
                retVal.Seek(0, SeekOrigin.Begin);
                return retVal;
            }
        }
    }
}
