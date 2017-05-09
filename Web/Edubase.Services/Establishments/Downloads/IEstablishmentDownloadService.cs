using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Core;
using static Edubase.Services.Core.FileDownloadFactoryService;

namespace Edubase.Services.Establishments.Downloads
{
    public interface IEstablishmentDownloadService : IFileDownloadFactoryService
    {
        Task SearchWithDownloadGenerationAsync(Guid taskId, EstablishmentSearchPayload payload, 
            IPrincipal principal, eDataSet dataSet, eFileFormat format);
    }
}