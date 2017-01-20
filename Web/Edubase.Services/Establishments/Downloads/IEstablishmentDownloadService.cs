using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.Search;
using static Edubase.Services.Establishments.Downloads.EstablishmentDownloadService;
using static Edubase.Services.Core.FileDownloadFactoryService;
using Edubase.Services.Core;

namespace Edubase.Services.Establishments.Downloads
{
    public interface IEstablishmentDownloadService : IFileDownloadFactoryService
    {
        Task SearchWithDownloadGenerationAsync(Guid taskId, EstablishmentSearchPayload payload, 
            IPrincipal principal, eDataSet dataSet, eFileFormat format);
    }
}