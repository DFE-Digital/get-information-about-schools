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

            // Order the intersection based on the descriptors' order and append any undefined establishment fields that we have from the API that don't map onto our list of fields.
            var retVal = intersection.OrderBy(x => baseFieldList.IndexOf(baseFieldList.FirstOrDefault(f => f.Key == x))).Select(x => baseFieldList.FirstOrDefault(f => f.Key == x))
                .Concat(fieldList.Where(x => !intersection.Contains(x)).Select(x => new EstablishmentField(x, x)));

            return retVal.ToList();
        }

        public async Task<ApiSearchResult<ChangeHistorySearchItem>> SearchAsync(SearchChangeHistoryBrowsePayload payload, IPrincipal principal)
            => (await _httpClient.PostAsync<ApiSearchResult<ChangeHistorySearchItem>>("change-history", payload, principal)).GetResponse();

        public async Task<List<UserGroupModel>> GetSuggesterGroupsAsync(IPrincipal principal)
            => (await _httpClient.GetAsync<List<UserGroupModel>>("suggesters", principal)).GetResponse();

        public async Task<List<UserGroupModel>> GetApproversGroupsAsync(IPrincipal principal)
            => (await _httpClient.GetAsync<List<UserGroupModel>>("approvers", principal)).GetResponse();

        public async Task<ProgressDto> SearchWithDownloadGenerationAsync(SearchChangeHistoryDownloadPayload payload, IPrincipal principal)
            => (await _httpClient.PostAsync<ProgressDto>("change-history/download", payload, principal)).GetResponse();

        public async Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal) 
            => (await _httpClient.GetAsync<ProgressDto>($"change-history/download/progress/{taskId}", principal)).GetResponse();

        private List<EstablishmentField> GetEstablishmentFieldDescriptors()
        {
            /*
             * JS FIDDLE CODE - to export from google s/s, to JSON to csharp
             *      
             *      var buffer = "";
                    for(var i = 0; i < test.length; i++) {
	                    if(test[i][2].toLowerCase() == "y"){
  	                    buffer += "fields.Add(new EstablishmentField(\""+test[i][0]+"\", \""+test[i][1]+"\"));\r\n";
                      }
                    }

                    var c = document.getElementById("c");
                    c.innerHTML=buffer;
             * 
             * 
             * */

            var fields = new List<EstablishmentField>();
            fields.Add(new EstablishmentField("AdministrativeWard", "Administrative ward"));
            fields.Add(new EstablishmentField("AdmissionsPolicy", "Admissions policy"));
            fields.Add(new EstablishmentField("Boarders", "Boarders"));
            fields.Add(new EstablishmentField("BoardingEstablishment", "Boarding establishment"));
            fields.Add(new EstablishmentField("CensusAreaStatisticWard", "CAS ward"));
            fields.Add(new EstablishmentField("CloseDate", "Close date"));
            fields.Add(new EstablishmentField("Locality", "Locality"));
            fields.Add(new EstablishmentField("Town", "City or town"));
            fields.Add(new EstablishmentField("Postcode", "Postcode"));
            fields.Add(new EstablishmentField("Diocese", "Diocese"));
            fields.Add(new EstablishmentField("Easting", "Easting"));
            fields.Add(new EstablishmentField("EstablishmentName", "Establishment name"));
            fields.Add(new EstablishmentField("EstablishmentNumber", "Establishment number"));
            fields.Add(new EstablishmentField("EstablishmentStatus", "Status"));
            fields.Add(new EstablishmentField("FurtherEducationType", "Further education type"));
            fields.Add(new EstablishmentField("Gender", "Gender"));
            fields.Add(new EstablishmentField("GOR", "Government office region"));
            fields.Add(new EstablishmentField("GSSLACode", "GSSLA code"));
            fields.Add(new EstablishmentField("HeadFirstName", "Head first name"));
            fields.Add(new EstablishmentField("HeadLastName", "Head last name"));
            fields.Add(new EstablishmentField("HeadTitle", "Head title"));
            fields.Add(new EstablishmentField("Inspectorate", "Inspectorate"));
            fields.Add(new EstablishmentField("LA", "Local authority"));
            fields.Add(new EstablishmentField("LSOA", "LSOA"));
            fields.Add(new EstablishmentField("MSOA", "MSOA"));
            fields.Add(new EstablishmentField("Northing", "Northing"));
            fields.Add(new EstablishmentField("NurseryProvision", "Nursery provision"));
            fields.Add(new EstablishmentField("OfficialSixthForm", "Official sixth form"));
            fields.Add(new EstablishmentField("OpenDate", "Open date"));
            fields.Add(new EstablishmentField("ParliamentaryConstituency", "Parliamentary constituency"));
            fields.Add(new EstablishmentField("PropsLastName", "Props last name"));
            fields.Add(new EstablishmentField("ReasonEstablishmentClosed", "Reason establishment closed"));
            fields.Add(new EstablishmentField("ReasonEstablishmentOpened", "Reason establishment opened"));
            fields.Add(new EstablishmentField("ReligiousCharacter", "Religious character"));
            fields.Add(new EstablishmentField("ReligiousEthos", "Religious ethos"));
            fields.Add(new EstablishmentField("ResourcedProvisionOnRoll", "Resourced provision on roll"));
            fields.Add(new EstablishmentField("RSCRegion", "RSC region"));
            fields.Add(new EstablishmentField("Section41Approved", "Section 41 approved"));
            fields.Add(new EstablishmentField("SEN1", "SEN 1"));
            fields.Add(new EstablishmentField("SEN2", "SEN 2"));
            fields.Add(new EstablishmentField("SEN3", "SEN 3"));
            fields.Add(new EstablishmentField("SEN4", "SEN 4"));
            fields.Add(new EstablishmentField("SenUnitCapacity", "SEN unit capacity"));
            fields.Add(new EstablishmentField("SenUnitOnRoll", "SEN unit on roll"));
            fields.Add(new EstablishmentField("SpecialClasses", "Special classes"));
            fields.Add(new EstablishmentField("StatutoryHighAge", "Statutory high age"));
            fields.Add(new EstablishmentField("StatutoryLowAge", "Statutory low age"));
            fields.Add(new EstablishmentField("TelephoneNum", "Telephone number"));
            fields.Add(new EstablishmentField("TypeOfEstablishment", "Type"));
            fields.Add(new EstablishmentField("TypeOfReservedProvision", "Type of resourced provision"));
            fields.Add(new EstablishmentField("UKPRN", "UKPRN"));
            fields.Add(new EstablishmentField("UrbanRural", "Urban rural"));
            fields.Add(new EstablishmentField("WebsiteAddress", "Website address"));
            return fields;
        }
    }
}
