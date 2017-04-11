using Edubase.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using System.IO;

namespace Edubase.Services.Texuna.Core
{
    public class FileDownloadFactoryApiService : IFileDownloadFactoryService
    {
        public Task<MemoryStream> CreateCsvStreamAsync(List<string> headers, List<List<string>> rows)
        {
            throw new NotImplementedException();
        }

        public MemoryStream CreateXlsxStream(string title, string worksheetTitle, List<string> headers, List<List<string>> rows)
        {
            throw new NotImplementedException();
        }

        public Task<SearchDownloadGenerationProgressDto> GetDownloadGenerationProgressAsync(Guid taskId)
        {
            throw new NotImplementedException();
        }

        public Task<SearchDownloadGenerationProgressDto> SearchWithDownloadGeneration_InitialiseAsync()
        {
            throw new NotImplementedException();
        }
    }
}
