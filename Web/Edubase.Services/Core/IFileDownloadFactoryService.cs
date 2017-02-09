using System;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using System.Collections.Generic;
using System.IO;

namespace Edubase.Services.Core
{
    public interface IFileDownloadFactoryService
    {
        Task<SearchDownloadGenerationProgressDto> GetDownloadGenerationProgressAsync(Guid taskId);
        Task<SearchDownloadGenerationProgressDto> SearchWithDownloadGeneration_InitialiseAsync();
        MemoryStream CreateXlsxStream(string title, string worksheetTitle, List<string> headers, List<List<string>> rows);
        Task<MemoryStream> CreateCsvStreamAsync(List<string> headers, List<List<string>> rows);
    }
}