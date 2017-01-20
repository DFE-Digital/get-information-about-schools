using Edubase.Common.Cache;
using Edubase.Common.IO;
using Edubase.Services.Domain;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Core
{
    /// <summary>
    /// Responsible for the creation of file downloads and publication of them.
    /// </summary>
    public abstract class FileDownloadFactoryService : IFileDownloadFactoryService
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
    }
}
