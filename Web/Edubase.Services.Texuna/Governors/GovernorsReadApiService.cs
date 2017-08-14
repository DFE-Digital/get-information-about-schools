using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Governors.Search;
using System.Security.Principal;
using Edubase.Services.Domain;
using Edubase.Services.Governors;
using System;
using Edubase.Services.Establishments;
using MoreLinq;
using System.Linq;

namespace Edubase.Services.Texuna.Governors
{
    public class GovernorsReadApiService : IGovernorsReadService
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly IEstablishmentReadService _establishmentReadService;

        public GovernorsReadApiService(HttpClientWrapper httpClient, IEstablishmentReadService establishmentReadService)
        {
            _httpClient = httpClient;
            _establishmentReadService = establishmentReadService;
        }

        public async Task<GovernorDisplayPolicy> GetEditorDisplayPolicyAsync(eLookupGovernorRole role, bool isGroup, IPrincipal principal)
            => (await _httpClient.GetAsync<GovernorDisplayPolicy>($"governor/{(int)role}/edit-policy?isForGroup={isGroup.ToString().ToLower()}", principal)).GetResponse();

        public async Task<GovernorModel> GetGovernorAsync(int gid, IPrincipal principal)
        {
            var retVal = (await _httpClient.GetAsync<GovernorModel>($"governor/{gid}", principal)).GetResponse();
            await PopulateEstablishmentName(principal, retVal);
            return retVal;
        }
        
        public async Task<GovernorsDetailsDto> GetGovernorListAsync(int? urn = default(int?), int? groupUId = default(int?), IPrincipal principal = null)
        {
            var retVal = (await _httpClient.GetAsync<GovernorsDetailsTexunaDto>($"governors?{(groupUId.HasValue ? "uid" : "urn")}={(urn.HasValue ? urn : groupUId)}", principal)).GetResponse();
            await PopulateEstablishmentName(principal, retVal.CurrentGovernors.Concat(retVal.HistoricalGovernors));
            return retVal;
        }
        public async Task<ApiPagedResult<SearchGovernorModel>> SearchAsync(GovernorSearchPayload payload, IPrincipal principal) 
            => (await _httpClient.PostAsync<ApiPagedResult<SearchGovernorModel>>("governor/search", payload, principal)).GetResponse();

        public async Task<IEnumerable<GovernorModel>> GetSharedGovernorsAsync(int establishmentUrn, IPrincipal principal)
        {
            var retVal = (await _httpClient.GetAsync<GovernorModel[]>($"governors/shared/{establishmentUrn}", principal)).GetResponse();
            await PopulateEstablishmentName(principal, retVal);
            return retVal;
        }
        public async Task<string> GetGovernorBulkUpdateTemplateUri(IPrincipal principal) => (await _httpClient.GetAsync<FileDownloadDto>($"governor/bulk-update/template", principal)).GetResponse().Url;

        private async Task PopulateEstablishmentName(IPrincipal principal, IEnumerable<GovernorModel> governors)
        {
            if (governors != null && governors.Any())
            {
                foreach (var item in governors)
                {
                    await PopulateEstablishmentName(principal, item);
                }
            }
        }

        private async Task PopulateEstablishmentName(IPrincipal principal, GovernorModel governor)
        {
            foreach (var appt in governor.Appointments ?? Enumerable.Empty<GovernorAppointment>())
            {
                appt.EstablishmentName = await _establishmentReadService.GetEstablishmentNameAsync(appt.EstablishmentUrn.Value, principal);
            }
        }
    }
}
