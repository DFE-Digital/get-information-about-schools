using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups.Models;
using Edubase.Services.Establishments.Search;
using System;
using Edubase.Services.Enums;

namespace Edubase.Services.Establishments
{
    public interface IEstablishmentReadService
    {
        Task<ServiceResultDto<EstablishmentModel>> GetAsync(int urn, IPrincipal principal);
        Task<ServiceResultDto<bool>> CanAccess(int urn, IPrincipal principal);
        Task<IEnumerable<EstablishmentChangeDto>> GetChangeHistoryAsync(int urn, int take, IPrincipal user);

        /// <summary>
        /// Returns whether the current principal can edit a given establishment
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <remarks>NON-BESPOKE BE</remarks>
        Task<bool> CanEditAsync(int urn, IPrincipal user);
        Task<EstablishmentDisplayEditPolicy> GetDisplayPolicyAsync(EstablishmentModel establishment, IPrincipal user);
        Task<EstablishmentDisplayEditPolicy> GetEditPolicyAsync(EstablishmentModel establishment, IPrincipal user);
        Task<IEnumerable<LinkedEstablishmentModel>> GetLinkedEstablishmentsAsync(int urn, IPrincipal principal);
        Task<IEnumerable<EstablishmentSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10);

        /// <summary>
        /// Searches establishments based on the supplied payload/filters.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        /// <exception cref="SearchQueryTooLargeException">
        ///     There's a chance that when you pass in a large query with 100s of filters
        ///     you'll get a SearchQueryTooLargeException.  There is no work-around; the size of the query needs to be reduced; this is due to a limitation in Azure Search.
        /// </exception>
        Task<ApiSearchResult<EstablishmentModel>> SearchAsync(EstablishmentSearchPayload payload, IPrincipal principal);
        Task<int[]> GetPermittedStatusIdsAsync(IPrincipal principal);

        Task<List<ChangeDescriptorDto>> GetModelChangesAsync(EstablishmentModel model, IPrincipal principal);
        Task<List<ChangeDescriptorDto>> GetModelChangesAsync(EstablishmentModel original, EstablishmentModel model);
        Task<FileDownloadDto> GetChangeHistoryDownloadAsync(int urn, eFileFormat format, IPrincipal principal);
        Task<FileDownloadDto> GetDownloadAsync(int urn, eFileFormat format, IPrincipal principal);
    }
}