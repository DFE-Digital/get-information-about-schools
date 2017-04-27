using Edubase.Services.Groups.Downloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Groups.Search;
using System.IO;
using System.Security.Principal;

namespace Edubase.Services.Texuna.Groups
{
    public class GroupDownloadApiService : IGroupDownloadService
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

        public Task SearchWithDownloadGenerationAsync(Guid taskId, GroupSearchPayload payload, IPrincipal principal, FileDownloadFactoryService.eFileFormat format)
        {
            throw new NotImplementedException();
        }

        public Task<SearchDownloadGenerationProgressDto> SearchWithDownloadGeneration_InitialiseAsync()
        {
            throw new NotImplementedException();
        }
    }
}
