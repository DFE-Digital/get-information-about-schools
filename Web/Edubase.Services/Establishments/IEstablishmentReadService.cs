using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments
{
    using EP = eLookupEducationPhase;
    using ET = eLookupEstablishmentType;

    public interface IEstablishmentReadService
    {
        Task<ServiceResultDto<bool>> CanAccess(int urn, IPrincipal principal);

        /// <summary>
        /// Returns whether the current principal can edit a given establishment
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <remarks>NON-BESPOKE BE</remarks>
        Task<bool> CanEditAsync(int urn, IPrincipal user);

        Task<IEnumerable<AddressLookupResult>> GetAddressesByPostCodeAsync(string postCode, IPrincipal principal);

        Task<ServiceResultDto<EstablishmentModel>> GetAsync(int urn, IPrincipal principal);

        Task<PaginatedResult<EstablishmentChangeDto>> GetChangeHistoryAsync(int urn, int skip, int take, string sortBy, IPrincipal user);

        Task<PaginatedResult<EstablishmentChangeDto>> GetChangeHistoryAsync(int urn, int skip, int take, string sortBy, EstablishmentChangeHistoryFilters filters, IPrincipal user);

        Task<FileDownloadDto> GetChangeHistoryDownloadAsync(int urn, DownloadType format, IPrincipal principal);

        Task<FileDownloadDto> GetChangeHistoryDownloadAsync(int urn, EstablishmentChangeHistoryDownloadFilters filters, IPrincipal principal);

        Task<EstablishmentDisplayEditPolicy> GetDisplayPolicyAsync(EstablishmentModel establishment, IPrincipal user);

        Task<FileDownloadDto> GetDownloadAsync(int urn, DownloadType format, IPrincipal principal);

        Task<EstablishmentDisplayEditPolicy> GetEditPolicyAsync(EstablishmentModel establishment, IPrincipal user);

        Task<string> GetEstablishmentNameAsync(int urn, IPrincipal principal);

        Dictionary<ET, EP[]> GetEstabType2EducationPhaseMap();

        Task<IEnumerable<LinkedEstablishmentModel>> GetLinkedEstablishmentsAsync(int urn, IPrincipal principal);

        Task<List<ChangeDescriptorDto>> GetModelChangesAsync(EstablishmentModel model, IPrincipal principal);

        Task<List<ChangeDescriptorDto>> GetModelChangesAsync(EstablishmentModel original, EstablishmentModel model);

        Task<IEnumerable<LookupDto>> GetPermissibleLocalGovernorsAsync(int urn, IPrincipal principal);

        Task<int[]> GetPermittedStatusIdsAsync(IPrincipal principal);

        /// <summary>
        /// Searches establishments based on the supplied payload/filters.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        Task<ApiPagedResult<EstablishmentSearchResultModel>> SearchAsync(EstablishmentSearchPayload payload, IPrincipal principal);

        Task<IEnumerable<EstablishmentSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10);
    }
}