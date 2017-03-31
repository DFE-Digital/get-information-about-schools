#if(!TEXAPI)
using Edubase.Common.Cache;
using Edubase.Services.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Core;
using Edubase.Services.Groups.Search;
using System.Security.Principal;
using Edubase.Services.Domain;
using Edubase.Common.IO;
using System.IO;
using Edubase.Services.Groups.Models;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using OfficeOpenXml;
using MoreLinq;

namespace Edubase.Services.Groups.Downloads
{
    public class GroupDownloadService : FileDownloadFactoryService, IGroupDownloadService
    {
        private ICacheAccessor _cacheAccessor;
        private ICachedLookupService _cachedLookupService;
        private IMessageLoggingService _messageLoggingService;
        private IGroupReadService _groupReadService;

        public GroupDownloadService(ICacheAccessor cacheAccessor,
            IGroupReadService groupReadService,
            IBlobService blobService,
            ICachedLookupService cachedLookupService,
            IMessageLoggingService messageLoggingService)
            : base(cacheAccessor, blobService)
        {
            _cacheAccessor = cacheAccessor;
            _groupReadService = groupReadService;
            _cachedLookupService = cachedLookupService;
            _messageLoggingService = messageLoggingService;
        }

        /// <summary>
        /// Creates a download
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="payload"></param>
        /// <param name="principal"></param>
        /// <param name="fieldList">e.g. EstablishmentDownloadCoreFieldList or EstablishmentDownloadFullFieldList</param>
        /// <returns></returns>
        public async Task SearchWithDownloadGenerationAsync(Guid taskId, GroupSearchPayload payload, IPrincipal principal, eFileFormat format)
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
                var results = await _groupReadService.SearchAsync(payload, principal);
                progress.TotalRecordsCount = results.Count.Value;
                progress.Status = "Retrieving data...";
                progress.FileExtension = ToFileExtension(format);
                await updateProgressCache();

                var tempPath = DirectoryHelper.CreateTempDirectory().FullName;
                var fileName = Path.Combine(tempPath, string.Concat("edubase-group-search-results", progress.FileExtension));

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

        private async Task GenerateXlsxFile(GroupSearchPayload payload, IPrincipal principal, 
            SearchDownloadGenerationProgressDto progress, Func<Task> updateProgressCache, 
            AzureSearchResult<SearchGroupDocument> results, string fileName)
        {
            var headers = GetHeaders(principal);

            using (var xlsx = new ExcelPackage(new FileInfo(fileName)))
            {
                xlsx.Workbook.Properties.Author = "Department for Education";
                xlsx.Workbook.Properties.Title = "Edubase Group Search Results";

                var cursor = 1;
                var sheet = xlsx.Workbook.Worksheets.Add("Group search results");
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
                    results = await _groupReadService.SearchAsync(payload, principal);
                    await updateProgressCache();
                }

                xlsx.Save();
            }
        }


        private async Task GenerateCsvFile(GroupSearchPayload payload, IPrincipal principal, SearchDownloadGenerationProgressDto progress, Func<Task> updateProgressCache, AzureSearchResult<SearchGroupDocument> results, string fileName)
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
                        results = await _groupReadService.SearchAsync(payload, principal);
                        await updateProgressCache();
                    }
                }
            }
        }


        private List<string> GetHeaders(IPrincipal principal)
        {
            var headers = new List<string>();
            headers.Add("Group Name");
            headers.Add("Companies House Number");
            headers.Add("Group Type");
            headers.Add("Number of linked providers");
            headers.Add("Open date");
            headers.Add("Address");
            headers.Add("Group manager email");
            headers.Add("Local Authority");
            headers.Add("UID");
            headers.Add("Group ID");
            if (principal.Identity.IsAuthenticated)
            {
                headers.Add("Closed date");
                headers.Add("Group status");
            }
            return headers;
        }

        private async Task<List<string>> GetRowData(SearchGroupDocument item, IPrincipal principal)
        {
            var fields = new List<string>();
            fields.Add(item.Name);
            fields.Add(item.CompaniesHouseNumber);
            fields.Add((await _cachedLookupService.GroupTypesGetAllAsync()).FirstOrDefault(x => x.Id == item.GroupTypeId)?.Name);
            fields.Add(item.EstablishmentCount.ToString());
            fields.Add(item.OpenDate?.ToString("dd/MM/yyyy"));
            fields.Add(item.Address);
            fields.Add(item.ManagerEmailAddress);

            if (item.LocalAuthorityId.HasValue) fields.Add((await _cachedLookupService.LocalAuthorityGetAllAsync()).FirstOrDefault(x => x.Id == item.LocalAuthorityId)?.Name);
            else fields.Add(string.Empty);

            fields.Add(item.GroupUID.ToString());
            fields.Add(item.GroupId);
            if (principal.Identity.IsAuthenticated)
            {
                fields.Add(item.ClosedDate?.ToString("dd/MM/yyyy"));
                fields.Add((await _cachedLookupService.GroupStatusesGetAllAsync()).FirstOrDefault(x => x.Id == item.StatusId)?.Name);
            }
            return fields;
        }
    }
    
}


#endif