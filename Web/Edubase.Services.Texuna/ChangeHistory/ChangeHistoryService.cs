using Edubase.Services.Domain;
using Edubase.Services.Texuna.ChangeHistory.Models;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Linq;
using MoreLinq;

namespace Edubase.Services.Texuna.ChangeHistory
{
    public class ChangeHistoryService : IChangeHistoryService
    {
        private readonly HttpClientWrapper _httpClient;

        public ChangeHistoryService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EstablishmentField>> GetEstablishmentFieldsAsync(IPrincipal principal)
        {
            var fieldList = (await _httpClient.GetAsync<string[]>("establishment/change-history/fields", principal)).GetResponse().AsEnumerable();
            var baseFieldList = GetEstablishmentFieldDescriptors();
            var fields = baseFieldList.Select(x => x.Key);
            var intersection = fieldList.Intersect(fields);
            
            var retVal = intersection.OrderBy(x => baseFieldList.IndexOf(baseFieldList.FirstOrDefault(f => f.Key == x))).Select(x => baseFieldList.FirstOrDefault(f => f.Key == x)).Select(x => new EstablishmentField(x.Key, x.Text));

            return retVal.ToList();
        }

        public async Task<ApiSearchResult<ChangeHistorySearchItem>> SearchAsync(SearchChangeHistoryBrowsePayload payload, IPrincipal principal)
            => (await _httpClient.PostAsync<ApiSearchResult<ChangeHistorySearchItem>>("change-history", payload, principal)).GetResponse();

        public async Task<List<UserGroupModel>> GetSuggesterGroupsAsync(IPrincipal principal)
            => (await _httpClient.GetAsync<List<UserGroupModel>>("groups/suggesters", principal)).GetResponse();

        public async Task<List<UserGroupModel>> GetApproversGroupsAsync(IPrincipal principal)
        {
            throw new NotImplementedException("GetApproversGroupsAsync is not implemented because it returns the exact same information as GetSuggesterGroupsAsync");
            //return (await _httpClient.GetAsync<List<UserGroupModel>>("groups/approvers", principal)).GetResponse();
        }

        public async Task<ProgressDto> SearchWithDownloadGenerationAsync(SearchChangeHistoryDownloadPayload payload, IPrincipal principal)
            => (await _httpClient.PostAsync<ProgressDto>("change-history/download", payload, principal)).GetResponse();

        public async Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal) 
            => (await _httpClient.GetAsync<ProgressDto>($"change-history/download/progress/{taskId}", principal)).GetResponse();

        private List<EstablishmentField> GetEstablishmentFieldDescriptors()
        {
            var fields = new List<EstablishmentField>();
            fields.Add(new EstablishmentField("AdmissionsPolicy", "Admissions policy"));
            fields.Add(new EstablishmentField("Boarders", "Boarders"));
            fields.Add(new EstablishmentField("CloseDate", "Close date"));
            fields.Add(new EstablishmentField("Address3", "Address3"));
            fields.Add(new EstablishmentField("Locality", "Locality"));
            fields.Add(new EstablishmentField("Town", "City or town"));
            fields.Add(new EstablishmentField("Postcode", "Postcode"));
            fields.Add(new EstablishmentField("Diocese", "Diocese"));
            fields.Add(new EstablishmentField("EstablishmentName", "School / College Name"));
            fields.Add(new EstablishmentField("EstablishmentNumber", "Establishment Number"));
            fields.Add(new EstablishmentField("EstablishmentStatus", "Establishment status"));
            fields.Add(new EstablishmentField("FurtherEducationType", "Further education type"));
            fields.Add(new EstablishmentField("Gender", "Gender of entry"));
            fields.Add(new EstablishmentField("GOR", "Government office region"));
            fields.Add(new EstablishmentField("GSSLACode", "GSS LA code"));
            fields.Add(new EstablishmentField("HeadFirstName", "Headteacher/Principal first name"));
            fields.Add(new EstablishmentField("HeadLastName", "Headteacher/Principal last name"));
            fields.Add(new EstablishmentField("HeadTitle", "Headteacher/Principal title"));
            fields.Add(new EstablishmentField("LA", "Local authority"));
            fields.Add(new EstablishmentField("NurseryProvision", "Nursery provision"));
            fields.Add(new EstablishmentField("OfficialSixthForm", "Official sixth form"));
            fields.Add(new EstablishmentField("OpenDate", "Open date"));
            fields.Add(new EstablishmentField("ParliamentaryConstituency", "Parliamentary constituency"));
            fields.Add(new EstablishmentField("PhaseOfEducation", "Phase of education"));
            fields.Add(new EstablishmentField("ReasonEstablishmentClosed", "Reason establishment closed"));
            fields.Add(new EstablishmentField("ReasonEstablishmentOpened", "Reason establishment opened"));
            fields.Add(new EstablishmentField("ReligiousCharacter", "Religious character"));
            fields.Add(new EstablishmentField("ReligiousEthos", "Religious ethos"));
            fields.Add(new EstablishmentField("RSCRegion", "RSC region"));
            fields.Add(new EstablishmentField("SchoolCapacity", "School capacity"));
            fields.Add(new EstablishmentField("SCUpreferredemail", "Email"));
            fields.Add(new EstablishmentField("StatutoryHighAge", "Age range High"));
            fields.Add(new EstablishmentField("StatutoryLowAge", "Age range Low"));
            fields.Add(new EstablishmentField("TelephoneNum", "Telephone"));
            fields.Add(new EstablishmentField("TypeOfEstablishment", "Establishment type"));
            fields.Add(new EstablishmentField("UKPRN", "UK provider reference number (UKPRN)"));
            fields.Add(new EstablishmentField("UPRN", "UPRN"));
            fields.Add(new EstablishmentField("UrbanRural", "Urban/Rural Description"));
            fields.Add(new EstablishmentField("WebsiteAddress", "Website"));
            return fields;
        }
    }
}
