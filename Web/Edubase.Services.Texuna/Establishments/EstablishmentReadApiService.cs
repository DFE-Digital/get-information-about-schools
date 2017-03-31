using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna.Establishments
{

    /// <summary>
    /// Implementation of IEstablishmentReadService that will gradually be changed to call the API rather than custom backend
    /// </summary>
    public class EstablishmentReadApiService : IEstablishmentReadService
    {
        private const string ApiSuggestPath = "suggest/establishment";

        private readonly HttpClientWrapper _httpClient;

        public EstablishmentReadApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<ServiceResultDto<bool>> CanAccess(int urn, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResultDto<EstablishmentModel>> GetAsync(int urn, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EstablishmentChangeDto>> GetChangeHistoryAsync(int urn, int take, IPrincipal user)
        {
            throw new NotImplementedException();
        }

        public EstablishmentDisplayPolicy GetDisplayPolicy(IPrincipal user, EstablishmentModelBase establishment)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LinkedEstablishmentModel>> GetLinkedEstablishments(int urn)
        {
            throw new NotImplementedException();
        }

        public Task<List<ChangeDescriptorDto>> GetModelChangesAsync(EstablishmentModel model)
        {
            throw new NotImplementedException();
        }

        public Task<List<ChangeDescriptorDto>> GetModelChangesAsync(EstablishmentModel original, EstablishmentModel model)
        {
            throw new NotImplementedException();
        }

        public int[] GetPermittedStatusIds(IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<AzureSearchResult<SearchEstablishmentDocument>> SearchAsync(EstablishmentSearchPayload payload, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EstablishmentSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10)
        {
            return await _httpClient.GetAsync<List<EstablishmentSuggestionItem>>($"{ApiSuggestPath}?q={text}&take={take}");
        }
        

    }
}
