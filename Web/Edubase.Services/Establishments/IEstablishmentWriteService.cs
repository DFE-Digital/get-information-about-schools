using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Domain;
using System;

namespace Edubase.Services.Establishments
{
    public interface IEstablishmentWriteService
    {
        Task SaveAsync(EstablishmentModel model, bool overrideCR, DateTime? effectiveDate, IPrincipal principal);
        Task<BulkUpdateProgressModel> BulkUpdateAsync(BulkUpdateDto bulkUpdateInfo, IPrincipal principal);
        Task<BulkUpdateProgressModel> BulkUpdateAsync_GetProgressAsync(Guid taskId, IPrincipal principal);
        Task<ApiResponse<int>> CreateNewAsync(NewEstablishmentModel model, IPrincipal principal);
        Task<ValidationEnvelopeDto> ValidateAsync(EstablishmentModel model, IPrincipal principal);
        Task<ApiResponse> PartialUpdateAsync(EstablishmentModel model, EstablishmentFieldList fieldsToUpdate, IPrincipal principal);
    }
}