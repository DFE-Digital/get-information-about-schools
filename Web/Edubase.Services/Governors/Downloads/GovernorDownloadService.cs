using Edubase.Common.Cache;
using Edubase.Common.IO;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Governors.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using Edubase.Services.Lookup;
using MoreLinq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Governors.Downloads
{
    public class GovernorDownloadService : FileDownloadFactoryService, IGovernorDownloadService
    {
        private ICacheAccessor _cacheAccessor;
        private ICachedLookupService _cachedLookupService;
        private IMessageLoggingService _messageLoggingService;
        private IGovernorsReadService _governorsReadService;

        public GovernorDownloadService(ICacheAccessor cacheAccessor,
            IGovernorsReadService governorsReadService,
            IBlobService blobService,
            ICachedLookupService cachedLookupService,
            IMessageLoggingService messageLoggingService)
            : base(cacheAccessor, blobService)
        {
            _cacheAccessor = cacheAccessor;
            _governorsReadService = governorsReadService;
            _cachedLookupService = cachedLookupService;
            _messageLoggingService = messageLoggingService;
        }

        /// <summary>
        /// Creates a download
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="payload"></param>
        /// <param name="principal"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public async Task SearchWithDownloadGenerationAsync(Guid taskId, GovernorSearchPayload payload, IPrincipal principal, eFileFormat format)
        {
            var progress = await _cacheAccessor.GetAsync<SearchDownloadGenerationProgressDto>(taskId.ToString());
            if (progress == null) throw new Exception($"Search download has not been initialised; call {nameof(SearchWithDownloadGeneration_InitialiseAsync)} first");

            Func<Task> updateProgressCache = async () => await _cacheAccessor.SetAsync(taskId.ToString(), progress, TimeSpan.FromHours(12));
            progress.Status = "Initialising...";
            await updateProgressCache();

            try
            {
                payload.Skip = 0;
                payload.Take = 1000;
                var results = await _governorsReadService.SearchAsync(payload, principal);
                progress.TotalRecordsCount = results.Count.Value;
                progress.Status = "Retrieving data...";
                progress.FileExtension = ToFileExtension(format);
                await updateProgressCache();

                var tempPath = DirectoryHelper.CreateTempDirectory().FullName;
                var fileName = Path.Combine(tempPath, string.Concat("edubase-governor-search-results", progress.FileExtension));

                if (format == eFileFormat.CSV)
                {
                    await GenerateCsvFile(payload, principal, progress, updateProgressCache, results, fileName);
                }
                else
                {
                    await GenerateXlsxFile(payload, principal, progress, updateProgressCache, results, fileName);
                }

                progress.Status = "Creating zip file...";
                await updateProgressCache();
                string zipFileName = CreateZipFile(fileName);

                progress.Status = "Preparing download package...";
                await updateProgressCache();
                string url = await UploadFileToBlobStorage(zipFileName);

                progress.FileLocation = url;
                progress.IsComplete = true;
                await updateProgressCache();

                Directory.Delete(tempPath, true);
                File.Delete(zipFileName);
            }
            catch (Exception ex)
            {
                progress.ExceptionMessageId = _messageLoggingService.Push(ex);
                await updateProgressCache();
            }
        }

        private async Task GenerateXlsxFile(GovernorSearchPayload payload, IPrincipal principal, 
            SearchDownloadGenerationProgressDto progress, Func<Task> updateProgressCache, 
            AzureSearchResult<SearchGovernorDocument> results, string fileName)
        {
            var headers = GetHeaders(principal);

            using (var xlsx = new ExcelPackage(new FileInfo(fileName)))
            {
                xlsx.Workbook.Properties.Author = "Department for Education";
                xlsx.Workbook.Properties.Title = "Edubase Governor Search Results";

                var cursor = 1;
                var sheet = xlsx.Workbook.Worksheets.Add("Governor search results");
                headers.ForEach((header, index) =>
                {
                    var cell = sheet.Cells[cursor, (index + 1)];
                    cell.Value = header;
                    cell.Style.Font.Bold = true;
                });
                cursor++;

                while (results.Items.Any())
                {
                    foreach (var item in results.Items)
                    {
                        var data = await GetRowData(item, principal);
                        data.ForEach((columnValue, index) => sheet.Cells[cursor, (index + 1)].Value = columnValue);
                        progress.ProcessedCount++;
                        cursor++;
                    }

                    payload.Skip += 1000;
                    results = await _governorsReadService.SearchAsync(payload, principal);
                    await updateProgressCache();
                }

                xlsx.Save();
            }
        }


        private async Task GenerateCsvFile(GovernorSearchPayload payload, IPrincipal principal, SearchDownloadGenerationProgressDto progress, Func<Task> updateProgressCache, AzureSearchResult<SearchGovernorDocument> results, string fileName)
        {
            var headers = GetHeaders(principal);

            using (var fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                using (var streamWriter = new StreamWriter(fs))
                {
                    await streamWriter.WriteLineAsync(ToCsv(headers));
                    while (results.Items.Any())
                    {
                        foreach (var item in results.Items)
                        {
                            var csv = ToCsv(await GetRowData(item, principal));
                            await streamWriter.WriteLineAsync(csv);
                            progress.ProcessedCount++;
                        }

                        payload.Skip += 1000;
                        results = await _governorsReadService.SearchAsync(payload, principal);
                        await updateProgressCache();
                    }
                }
            }
        }


        private List<string> GetHeaders(IPrincipal principal)
        {
            var items = new List<string>();
            items.Add("GID");
            items.Add("URN");
            items.Add("UID");
            items.Add("Role");
            items.Add("Title");
            items.Add("Forename 1");
            items.Add("Forename 2");
            items.Add("Surname");
            items.Add("Date of appointment");
            items.Add("Date term of office ends/ended");
            items.Add("Appointing body");
            return items;
        }

        private async Task<List<string>> GetRowData(SearchGovernorDocument item, IPrincipal principal)
        {
            var items = new List<string>();
            items.Add(item.Id.ToString());
            items.Add(item.EstablishmentUrn?.ToString());
            items.Add(item.GroupUID?.ToString());
            items.Add((await _cachedLookupService.GovernorRolesGetAllAsync()).FirstOrDefault(x => x.Id == item.RoleId)?.Name);
            items.Add(item.Person_Title);
            items.Add(item.Person_FirstName);
            items.Add(item.Person_MiddleName);
            items.Add(item.Person_LastName);
            items.Add(item.AppointmentStartDate?.ToString("dd/MM/yyyy"));
            items.Add(item.AppointmentEndDate?.ToString("dd/MM/yyyy"));
            items.Add((await _cachedLookupService.GovernorAppointingBodiesGetAllAsync()).FirstOrDefault(x => x.Id == item.AppointingBodyId)?.Name);
            return items;
        }
    }
    
}
