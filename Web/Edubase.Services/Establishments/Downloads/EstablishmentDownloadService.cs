using Edubase.Common.Cache;
using Edubase.Common.IO;
using Edubase.Common.Reflection;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Exceptions;
using Ionic.Zip;
using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Downloads
{
    public class EstablishmentDownloadService : IEstablishmentDownloadService
    {
        private ICacheAccessor _cacheAccessor;
        private IEstablishmentReadService _establishmentReadService;
        private IBlobService _blobService;

        public enum eDataSet
        {
            Core,
            Full
        }

        public EstablishmentDownloadService(ICacheAccessor cacheAccessor, IEstablishmentReadService establishmentReadService, IBlobService blobService)
        {
            _cacheAccessor = cacheAccessor;
            _establishmentReadService = establishmentReadService;
            _blobService = blobService;
        }

        public async Task<SearchDownloadGenerationProgressDto> SearchWithDownloadGeneration_InitialiseAsync()
        {
            var progress = new SearchDownloadGenerationProgressDto(Guid.NewGuid());
            await _cacheAccessor.SetAsync(progress.Id.ToString(), progress, TimeSpan.FromHours(12));
            return progress;
        }

        /// <summary>
        /// Creates a download
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="payload"></param>
        /// <param name="principal"></param>
        /// <param name="fieldList">e.g. EstablishmentDownloadCoreFieldList or EstablishmentDownloadFullFieldList</param>
        /// <returns></returns>
        public async Task SearchWithDownloadGenerationAsync(Guid taskId, EstablishmentSearchPayload payload, IPrincipal principal, eDataSet dataSet)
        {
            var progress = await _cacheAccessor.GetAsync<SearchDownloadGenerationProgressDto>(taskId.ToString());
            if (progress == null) throw new Exception($"Search download has not been initialised; call {nameof(SearchWithDownloadGeneration_InitialiseAsync)} first");

            var fieldList = dataSet == eDataSet.Core ? new EstablishmentDownloadCoreFieldList() : new EstablishmentDownloadFullFieldList();
            Func<Task> updateProgressCache = async () => await _cacheAccessor.SetAsync(taskId.ToString(), progress, TimeSpan.FromHours(12));
            progress.Status = "Initialising..."; await updateProgressCache();

            try
            {
                payload.Skip = 0;
                payload.Take = 1000;
                var results = await _establishmentReadService.SearchAsync(payload, principal);
                progress.TotalRecordsCount = results.Count.Value;
                progress.Status = "Retrieving data..."; await updateProgressCache();

                var fileName = Path.GetTempFileName();
                using (var fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.None))
                {
                    using (var streamWriter = new StreamWriter(fs))
                    {
                        await streamWriter.WriteLineAsync("Urn,Name");
                        while (results.Items.Any())
                        {
                            foreach (var item in results.Items)
                            {
                                //var csv = ToCsv(item, principal, fieldList);
                                await streamWriter.WriteLineAsync($"{item.Urn},{item.Name}");
                                progress.ProcessedCount++;
                            }

                            payload.Skip += 1000;
                            results = await _establishmentReadService.SearchAsync(payload, principal);
                            await updateProgressCache();
                        }
                    }
                }

                progress.Status = "Creating zip file..."; await updateProgressCache();

                var zipFileName = FileHelper.GetTempFileName("zip");
                using (var zip = new ZipFile(zipFileName))
                {
                    zip.AddFile(fileName, string.Empty);
                    zip.Save();
                }

                progress.Status = "Preparing download package..."; await updateProgressCache();

                var blobFileName = _blobService.CreateRandomBlobName("zip");
                var blobLocation = "/downloads/" + blobFileName;
                await _blobService.UploadAsync(zipFileName, new FileHelper().GetMimeType(zipFileName), blobLocation, "edubase-search-results.zip");
                var url = _blobService.GetReadOnlySharedAccessUrl("downloads", blobFileName, DateTimeOffset.UtcNow.AddDays(1));

                progress.FileLocation = url;
                progress.IsComplete = true;
                await updateProgressCache();
            }
            catch (Exception ex)
            {
                progress.Exception = ex;
                await updateProgressCache();
                throw;
            }
        }

        public async Task<SearchDownloadGenerationProgressDto> GetDownloadGenerationProgressAsync(Guid taskId)
        {
            var retVal = await _cacheAccessor.GetAsync<SearchDownloadGenerationProgressDto>(taskId.ToString());
            if(retVal == null) throw new Exception($"Search download has not been initialised; ensure {nameof(SearchWithDownloadGeneration_InitialiseAsync)} is called first.");
            else return retVal;
        }

        //private string GetCsvHeaders(EstablishmentFieldListBase fieldList)
        //{
        //    var props = ReflectionHelper.GetProperties(fieldList);
        //    props = props.Where(x => (bool)ReflectionHelper.GetPropertyValue(fieldList, x)).ToList();


        //}

        //private string ToCsv(SearchEstablishmentDocument document, IPrincipal principal, EstablishmentFieldListBase fieldList)
        //{
        //    var displayPolicy = new DisplayPolicyFactory().Create(principal, document, null);

            
        //}
    }
}
