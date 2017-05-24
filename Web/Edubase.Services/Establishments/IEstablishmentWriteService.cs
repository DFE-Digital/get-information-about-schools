using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Establishments.Models;
using System.Collections.Generic;
using Edubase.Common;
using Edubase.Services.Domain;
using System;

namespace Edubase.Services.Establishments
{
    public interface IEstablishmentWriteService
    {
        Task SaveAsync(EstablishmentModel model, bool overrideCR, DateTime? effectiveDate, IPrincipal principal);
        Task<BulkUpdateProgressModel> BulkUpdateAsync(BulkUpdateDto bulkUpdateInfo, IPrincipal principal);
        Task<BulkUpdateProgressModel> BulkUpdateAsync_GetProgressAsync(Guid taskId, IPrincipal principal);
        Task<int> CreateNewAsync(NewEstablishmentModel model, IPrincipal principal);
    }
}