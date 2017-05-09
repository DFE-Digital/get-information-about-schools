using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Linq;
using Edubase.Services.Texuna.Models;

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

        public async Task<ServiceResultDto<bool>> CanAccess(int urn, IPrincipal principal)
        {
            return new ServiceResultDto<bool>((await _httpClient.GetAsync<BoolResult>($"establishment/{urn}/canaccess", principal)).Value);
        }

        public async Task<bool> CanEditAsync(int urn, IPrincipal principal)
        {
            return (await _httpClient.GetAsync<BoolResult>($"establishment/{urn}/canedit", principal)).Value;
        }

        public async Task<ServiceResultDto<EstablishmentModel>> GetAsync(int urn, IPrincipal principal)
        {
            return new ServiceResultDto<EstablishmentModel>(await _httpClient.GetAsync<EstablishmentModel>($"establishment/{urn}", principal));
        }

        public async Task<IEnumerable<EstablishmentChangeDto>> GetChangeHistoryAsync(int urn, int take, IPrincipal user)
        {
            return await _httpClient.GetAsync<List<EstablishmentChangeDto>>($"establishment/{urn}/changes", user);
        }

        public async Task<EstablishmentDisplayPolicy> GetDisplayPolicyAsync(IPrincipal user, EstablishmentModel establishment)
        {
            return await _httpClient.GetAsync<EstablishmentDisplayPolicy>($"establishment/{establishment.Urn}/display-policy", user);
        }

        public async Task<IEnumerable<LinkedEstablishmentModel>> GetLinkedEstablishmentsAsync(int urn, IPrincipal principal)
        {
            return await _httpClient.GetAsync<List<LinkedEstablishmentModel>>($"establishment/{urn}/linked-establishments", principal);
        }

        public Task<List<ChangeDescriptorDto>> GetModelChangesAsync(EstablishmentModel model)
        {
            throw new NotImplementedException(nameof(GetModelChangesAsync) + " not implemented yet");
        }

        public Task<List<ChangeDescriptorDto>> GetModelChangesAsync(EstablishmentModel original, EstablishmentModel model)
        {
            throw new NotImplementedException(nameof(GetModelChangesAsync) + " not implemented yet");
        }

        public async Task<int[]> GetPermittedStatusIdsAsync(IPrincipal principal)
        {
            return (await _httpClient.GetAsync<List<LookupDto>>("establishment/permittedstatuses", principal)).Select(x => x.Id).ToArray();
        }

        public async Task<ApiSearchResult<EstablishmentModel>> SearchAsync(EstablishmentSearchPayload payload, IPrincipal principal)
        {
            return await _httpClient.PostAsync<ApiSearchResult<EstablishmentModel>>("establishment/search", payload, principal);
        }

        public async Task<IEnumerable<EstablishmentSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10)
        {
            return await _httpClient.GetAsync<List<EstablishmentSuggestionItem>>($"{ApiSuggestPath}?q={text}&take={take}", principal);
        }
        

    }
}
