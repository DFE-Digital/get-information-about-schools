using Edubase.Services.Establishments.Downloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.Search;
using System.IO;
using System.Security.Principal;

namespace Edubase.Services.Texuna.Establishments
{
    public class EstablishmentDownloadApiService : IEstablishmentDownloadService
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

        public Task SearchWithDownloadGenerationAsync(Guid taskId, EstablishmentSearchPayload payload, IPrincipal principal, eDataSet dataSet, FileDownloadFactoryService.eFileFormat format)
        {
            throw new NotImplementedException();
        }

        public Task<SearchDownloadGenerationProgressDto> SearchWithDownloadGeneration_InitialiseAsync()
        {
            throw new NotImplementedException();
        }
    }
}
