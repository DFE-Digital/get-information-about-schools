using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Domain;
using System;
using Edubase.Services.Enums;

namespace Edubase.Services.Establishments
{
    public interface IEstablishmentWriteService
    {
        Task SaveAsync(EstablishmentModel model, bool overrideCR, DateTime? effectiveDate, IPrincipal principal);
        Task<ApiResponse<BulkUpdateProgressModel>> BulkUpdateAsync(BulkUpdateDto bulkUpdateInfo, IPrincipal principal);
        Task<BulkUpdateProgressModel> BulkUpdateAsync_GetProgressAsync(Guid taskId, IPrincipal principal);
        Task<ApiResponse<int>> CreateNewAsync(NewEstablishmentModel model, IPrincipal principal);
        Task<ApiResponse<int>> CreateNewAsync(EstablishmentModel model, bool generateEstablishmentNumber,IPrincipal principal);
        Task<ValidationEnvelopeDto> ValidateAsync(EstablishmentModel model, IPrincipal principal);
        Task<ValidationEnvelopeDto> ValidateCreateAsync(EstablishmentModel model, bool generateEstablishmentNumber, IPrincipal principal);
        Task<ApiResponse> PartialUpdateAsync(EstablishmentModel model, EstablishmentFieldList fieldsToUpdate, IPrincipal principal);
        Task<ApiResponse<AmalgamateMergeResult, AmalgamateMergeValidationEnvelope[]>> AmalgamateOrMergeAsync(AmalgamateMergeRequest request, IPrincipal principal);
        Task<ApiResponse<BulkCreateAcademiesProgress, NewAcademyRequestValidationEnvelope[]>> BulkCreateAcademies(NewAcademyRequest[] newAcademyRequests, IPrincipal principal);
        Task<ApiResponse<BulkCreateAcademiesProgress>> GetBulkCreateAcademiesProgress(Guid id, IPrincipal principal);
        Task<ApiResponse<NewAcademyRequestValidationEnvelope[]>> ValidateBulkCreateAcademies(NewAcademyRequest[] newAcademyRequests, IPrincipal principal);
        Task<ApiResponse> UpdateGovernanceModeAsync(int urn, eGovernanceMode mode, IPrincipal principal);
        Task<ApiResponse<object, ValidationEnvelopeDto[]>> SaveLinkedEstablishmentsAsync(int urn, LinkedEstablishmentModel[] linkedEstablishmentModels, IPrincipal principal);
        Task<ApiResponse<object, ValidationEnvelopeDto[]>> DeleteLinkedEstablishmentAsync(int urn, int linkId, IPrincipal principal);
        Task<ApiResponse<object, ValidationEnvelopeDto[]>> AddLinkedEstablishmentAsync(int parentUrn, int urnToLink, int linkTypeId, DateTime linkDate, IPrincipal principal);
        Task<ApiResponse> ConfirmAsync(int urn, IPrincipal principal);
        Task<ApiResponse> ConfirmGovernanceAsync(int urn, IPrincipal principal);
    }
}