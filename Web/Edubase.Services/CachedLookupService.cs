using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using System.Runtime.Caching;
using Edubase.Common;
using Edubase.Services.Domain;

namespace Edubase.Services
{
    public class CachedLookupService
    {
        private Dictionary<string, Func<ApplicationDbContext, int, Task<string>>> _mapping = null;

        private LookupService _svc = new LookupService();

        public CachedLookupService()
        {
            _mapping = new Dictionary<string, Func<ApplicationDbContext, int, Task<string>>>()
            {
                { "LocalAuthorityId", async (dc, id) => (await LocalAuthorityGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "HeadTitleId", async (dc, id) => GetName((await HeadTitlesGetAllAsync()), id) },
                { "GenderId", async (dc, id) => GetName((await GendersGetAllAsync()), id) },
                { "EducationPhaseId", async (dc, id) => GetName((await EducationPhasesGetAllAsync()), id) },
                { "AdmissionsPolicyId", async (dc, id) => GetName((await AdmissionsPoliciesGetAllAsync()), id) },
                { "StatusId", async (dc, id) => GetName((await EstablishmentStatusesGetAllAsync()), id) },
                { "ReasonEstablishmentOpenedId", async (dc, id) => GetName((await ReasonEstablishmentOpenedGetAllAsync()), id) },
                { "ReasonEstablishmentClosedId", async (dc, id) => GetName((await ReasonEstablishmentClosedGetAllAsync()), id) },
                { "ProvisionBoardingId", async (dc, id) => GetName((await ProvisionBoardingGetAllAsync()), id) },
                { "ProvisionNurseryId", async (dc, id) => GetName((await ProvisionNurseriesGetAllAsync()), id) },
                { "ProvisionOfficialSixthFormId", async (dc, id) => GetName((await ProvisionOfficialSixthFormsGetAllAsync()), id) },
                { "ReligiousCharacterId", async (dc, id) => GetName((await ReligiousCharactersGetAllAsync()), id) },
                { "ReligiousEthosId", async (dc, id) => GetName((await ReligiousEthosGetAllAsync()), id) },
                { "DioceseId", async (dc, id) => GetName((await DiocesesGetAllAsync()), id) },
                { "ProvisionSpecialClassesId", async (dc, id) => GetName((await ProvisionSpecialClassesGetAllAsync()), id) },
                { "TypeId", async (dc, id) => GetName((await EstablishmentTypesGetAllAsync()), id) }
            };
        }

        public async Task<IEnumerable<LookupDto>> LocalAuthorityGetAllAsync() => await Cacher.AutoAsync(_svc.LocalAuthorityGetAllAsync);
        public IEnumerable<LookupDto> LocalAuthorityGetAll() => Cacher.Auto(_svc.LocalAuthorityGetAll);
        public async Task<IEnumerable<LookupDto>> AdmissionsPoliciesGetAllAsync() => await Cacher.AutoAsync(_svc.AdmissionsPoliciesGetAllAsync);
        public IEnumerable<LookupDto> AdmissionsPoliciesGetAll() => Cacher.Auto(_svc.AdmissionsPoliciesGetAll);
        public async Task<IEnumerable<LookupDto>> EducationPhasesGetAllAsync() => await Cacher.AutoAsync(_svc.EducationPhasesGetAllAsync);
        public IEnumerable<LookupDto> EducationPhasesGetAll() => Cacher.Auto(_svc.EducationPhasesGetAll);
        public async Task<IEnumerable<LookupDto>> EstablishmentStatusesGetAllAsync() => await Cacher.AutoAsync(_svc.EstablishmentStatusesGetAllAsync);
        public IEnumerable<LookupDto> EstablishmentStatusesGetAll() => Cacher.Auto(_svc.EstablishmentStatusesGetAll);
        public async Task<IEnumerable<LookupDto>> EstablishmentTypesGetAllAsync() => await Cacher.AutoAsync(_svc.EstablishmentTypesGetAllAsync);
        public IEnumerable<LookupDto> EstablishmentTypesGetAll() => Cacher.Auto(_svc.EstablishmentTypesGetAll);
        public async Task<IEnumerable<LookupDto>> GendersGetAllAsync() => await Cacher.AutoAsync(_svc.GendersGetAllAsync);
        public IEnumerable<LookupDto> GendersGetAll() => Cacher.Auto(_svc.GendersGetAll);
        public async Task<IEnumerable<LookupDto>> GroupTypesGetAllAsync() => await Cacher.AutoAsync(_svc.GroupTypesGetAllAsync);
        public IEnumerable<LookupDto> GroupTypesGetAll() => Cacher.Auto(_svc.GroupTypesGetAll);
        public async Task<IEnumerable<LookupDto>> HeadTitlesGetAllAsync() => await Cacher.AutoAsync(_svc.HeadTitlesGetAllAsync);
        public IEnumerable<LookupDto> HeadTitlesGetAll() => Cacher.Auto(_svc.HeadTitlesGetAll);
        public async Task<IEnumerable<LookupDto>> ProvisionBoardingGetAllAsync() => await Cacher.AutoAsync(_svc.ProvisionBoardingGetAllAsync);
        public IEnumerable<LookupDto> ProvisionBoardingGetAll() => Cacher.Auto(_svc.ProvisionBoardingGetAll);
        public async Task<IEnumerable<LookupDto>> ProvisionNurseriesGetAllAsync() => await Cacher.AutoAsync(_svc.ProvisionNurseriesGetAllAsync);
        public IEnumerable<LookupDto> ProvisionNurseriesGetAll() => Cacher.Auto(_svc.ProvisionNurseriesGetAll);
        public async Task<IEnumerable<LookupDto>> ProvisionOfficialSixthFormsGetAllAsync() => await Cacher.AutoAsync(_svc.ProvisionOfficialSixthFormsGetAllAsync);
        public IEnumerable<LookupDto> ProvisionOfficialSixthFormsGetAll() => Cacher.Auto(_svc.ProvisionOfficialSixthFormsGetAll);
        public async Task<IEnumerable<LookupDto>> ProvisionSpecialClassesGetAllAsync() => await Cacher.AutoAsync(_svc.ProvisionSpecialClassesGetAllAsync);
        public IEnumerable<LookupDto> ProvisionSpecialClassesGetAll() => Cacher.Auto(_svc.ProvisionSpecialClassesGetAll);
        public async Task<IEnumerable<LookupDto>> ReasonEstablishmentClosedGetAllAsync() => await Cacher.AutoAsync(_svc.ReasonEstablishmentClosedGetAllAsync);
        public IEnumerable<LookupDto> ReasonEstablishmentClosedGetAll() => Cacher.Auto(_svc.ReasonEstablishmentClosedGetAll);
        public async Task<IEnumerable<LookupDto>> ReasonEstablishmentOpenedGetAllAsync() => await Cacher.AutoAsync(_svc.ReasonEstablishmentOpenedGetAllAsync);
        public IEnumerable<LookupDto> ReasonEstablishmentOpenedGetAll() => Cacher.Auto(_svc.ReasonEstablishmentOpenedGetAll);
        public async Task<IEnumerable<LookupDto>> ReligiousCharactersGetAllAsync() => await Cacher.AutoAsync(_svc.ReligiousCharactersGetAllAsync);
        public IEnumerable<LookupDto> ReligiousCharactersGetAll() => Cacher.Auto(_svc.ReligiousCharactersGetAll);
        public async Task<IEnumerable<LookupDto>> ReligiousEthosGetAllAsync() => await Cacher.AutoAsync(_svc.ReligiousEthosGetAllAsync);
        public IEnumerable<LookupDto> ReligiousEthosGetAll() => Cacher.Auto(_svc.ReligiousEthosGetAll);
        public async Task<IEnumerable<LookupDto>> GovernorRolesGetAllAsync() => await Cacher.AutoAsync(_svc.GovernorRolesGetAllAsync);
        public IEnumerable<LookupDto> GovernorRolesGetAll() => Cacher.Auto(_svc.GovernorRolesGetAll);
        public async Task<IEnumerable<LookupDto>> GovernorAppointingBodiesGetAllAsync() => await Cacher.AutoAsync(_svc.GovernorAppointingBodiesGetAllAsync);
        public IEnumerable<LookupDto> GovernorAppointingBodiesGetAll() => Cacher.Auto(_svc.GovernorAppointingBodiesGetAll);
        public async Task<IEnumerable<LookupDto>> AccommodationChangedGetAllAsync() => await Cacher.AutoAsync(_svc.AccommodationChangedGetAllAsync);
        public IEnumerable<LookupDto> AccommodationChangedGetAll() => Cacher.Auto(_svc.AccommodationChangedGetAll);
        public async Task<IEnumerable<LookupDto>> BoardingEstablishmentGetAllAsync() => await Cacher.AutoAsync(_svc.BoardingEstablishmentGetAllAsync);
        public IEnumerable<LookupDto> BoardingEstablishmentGetAll() => Cacher.Auto(_svc.BoardingEstablishmentGetAll);
        public async Task<IEnumerable<LookupDto>> CCGovernanceGetAllAsync() => await Cacher.AutoAsync(_svc.CCGovernanceGetAllAsync);
        public IEnumerable<LookupDto> CCGovernanceGetAll() => Cacher.Auto(_svc.CCGovernanceGetAll);
        public async Task<IEnumerable<LookupDto>> CCOperationalHoursGetAllAsync() => await Cacher.AutoAsync(_svc.CCOperationalHoursGetAllAsync);
        public IEnumerable<LookupDto> CCOperationalHoursGetAll() => Cacher.Auto(_svc.CCOperationalHoursGetAll);
        public async Task<IEnumerable<LookupDto>> CCPhaseTypesGetAllAsync() => await Cacher.AutoAsync(_svc.CCPhaseTypesGetAllAsync);
        public IEnumerable<LookupDto> CCPhaseTypesGetAll() => Cacher.Auto(_svc.CCPhaseTypesGetAll);
        public async Task<IEnumerable<LookupDto>> DiocesesGetAllAsync() => await Cacher.AutoAsync(_svc.DiocesesGetAllAsync);
        public IEnumerable<LookupDto> DiocesesGetAll() => Cacher.Auto(_svc.DiocesesGetAll);
        public async Task<IEnumerable<LookupDto>> ChildcareFacilitiesGetAllAsync() => await Cacher.AutoAsync(_svc.ChildcareFacilitiesGetAllAsync);
        public IEnumerable<LookupDto> ChildcareFacilitiesGetAll() => Cacher.Auto(_svc.ChildcareFacilitiesGetAll);
        public async Task<IEnumerable<LookupDto>> DirectProvisionOfEarlyYearsGetAllAsync() => await Cacher.AutoAsync(_svc.DirectProvisionOfEarlyYearsGetAllAsync);
        public IEnumerable<LookupDto> DirectProvisionOfEarlyYearsGetAll() => Cacher.Auto(_svc.DirectProvisionOfEarlyYearsGetAll);
        public async Task<IEnumerable<LookupDto>> FurtherEducationTypesGetAllAsync() => await Cacher.AutoAsync(_svc.FurtherEducationTypesGetAllAsync);
        public IEnumerable<LookupDto> FurtherEducationTypesGetAll() => Cacher.Auto(_svc.FurtherEducationTypesGetAll);
        public async Task<IEnumerable<LookupDto>> IndependentSchoolTypesGetAllAsync() => await Cacher.AutoAsync(_svc.IndependentSchoolTypesGetAllAsync);
        public IEnumerable<LookupDto> IndependentSchoolTypesGetAll() => Cacher.Auto(_svc.IndependentSchoolTypesGetAll);
        public async Task<IEnumerable<LookupDto>> InspectoratesGetAllAsync() => await Cacher.AutoAsync(_svc.InspectoratesGetAllAsync);
        public IEnumerable<LookupDto> InspectoratesGetAll() => Cacher.Auto(_svc.InspectoratesGetAll);
        public async Task<IEnumerable<LookupDto>> InspectorateNamesGetAllAsync() => await Cacher.AutoAsync(_svc.InspectorateNamesGetAllAsync);
        public IEnumerable<LookupDto> InspectorateNamesGetAll() => Cacher.Auto(_svc.InspectorateNamesGetAll);
        public async Task<IEnumerable<LookupDto>> LocalGovernorsGetAllAsync() => await Cacher.AutoAsync(_svc.LocalGovernorsGetAllAsync);
        public IEnumerable<LookupDto> LocalGovernorsGetAll() => Cacher.Auto(_svc.LocalGovernorsGetAll);
        public async Task<IEnumerable<LookupDto>> NationalitiesGetAllAsync() => await Cacher.AutoAsync(_svc.NationalitiesGetAllAsync);
        public IEnumerable<LookupDto> NationalitiesGetAll() => Cacher.Auto(_svc.NationalitiesGetAll);
        public async Task<IEnumerable<LookupDto>> PRUEBDsGetAllAsync() => await Cacher.AutoAsync(_svc.PRUEBDsGetAllAsync);
        public IEnumerable<LookupDto> PRUEBDsGetAll() => Cacher.Auto(_svc.PRUEBDsGetAll);
        public async Task<IEnumerable<LookupDto>> PruEducatedByOthersGetAllAsync() => await Cacher.AutoAsync(_svc.PruEducatedByOthersGetAllAsync);
        public IEnumerable<LookupDto> PruEducatedByOthersGetAll() => Cacher.Auto(_svc.PruEducatedByOthersGetAll);
        public async Task<IEnumerable<LookupDto>> PruFulltimeProvisionsGetAllAsync() => await Cacher.AutoAsync(_svc.PruFulltimeProvisionsGetAllAsync);
        public IEnumerable<LookupDto> PruFulltimeProvisionsGetAll() => Cacher.Auto(_svc.PruFulltimeProvisionsGetAll);
        public async Task<IEnumerable<LookupDto>> PRUSENsGetAllAsync() => await Cacher.AutoAsync(_svc.PRUSENsGetAllAsync);
        public IEnumerable<LookupDto> PRUSENsGetAll() => Cacher.Auto(_svc.PRUSENsGetAll);
        public async Task<IEnumerable<LookupDto>> ResourcedProvisionsGetAllAsync() => await Cacher.AutoAsync(_svc.ResourcedProvisionsGetAllAsync);
        public IEnumerable<LookupDto> ResourcedProvisionsGetAll() => Cacher.Auto(_svc.ResourcedProvisionsGetAll);
        public async Task<IEnumerable<LookupDto>> Section41ApprovedGetAllAsync() => await Cacher.AutoAsync(_svc.Section41ApprovedGetAllAsync);
        public IEnumerable<LookupDto> Section41ApprovedGetAll() => Cacher.Auto(_svc.Section41ApprovedGetAll);
        public async Task<IEnumerable<LookupDto>> SpecialEducationNeedsGetAllAsync() => await Cacher.AutoAsync(_svc.SpecialEducationNeedsGetAllAsync);
        public IEnumerable<LookupDto> SpecialEducationNeedsGetAll() => Cacher.Auto(_svc.SpecialEducationNeedsGetAll);
        public async Task<IEnumerable<LookupDto>> TeenageMothersProvisionsGetAllAsync() => await Cacher.AutoAsync(_svc.TeenageMothersProvisionsGetAllAsync);
        public IEnumerable<LookupDto> TeenageMothersProvisionsGetAll() => Cacher.Auto(_svc.TeenageMothersProvisionsGetAll);
        public async Task<IEnumerable<LookupDto>> TypeOfResourcedProvisionsGetAllAsync() => await Cacher.AutoAsync(_svc.TypeOfResourcedProvisionsGetAllAsync);
        public IEnumerable<LookupDto> TypeOfResourcedProvisionsGetAll() => Cacher.Auto(_svc.TypeOfResourcedProvisionsGetAll);
        public async Task<IEnumerable<LookupDto>> EstablishmentLinkTypesGetAllAsync() => await Cacher.AutoAsync(_svc.EstablishmentLinkTypesGetAllAsync);
        public IEnumerable<LookupDto> EstablishmentLinkTypesGetAll() => Cacher.Auto(_svc.EstablishmentLinkTypesGetAll);
        public async Task<IEnumerable<LookupDto>> EstablishmentGroupTypesGetAllAsync() => await Cacher.AutoAsync(_svc.EstablishmentGroupTypesGetAllAsync);
        public IEnumerable<LookupDto> EstablishmentGroupTypesGetAll() => Cacher.Auto(_svc.EstablishmentGroupTypesGetAll);


        public async Task<string> GetNameAsync(string lookupName, int id) => 
            await ApplicationDbContext.OperationAsync(async dc => await _mapping.Get(lookupName)?.Invoke(dc, id));

        public bool IsLookupField(string name) => _mapping.ContainsKey(name);

        private string GetName(IEnumerable<LookupDto> items, int id) => items.FirstOrDefault(x => x.Id == id)?.Name;

        public void Dispose() => _svc.Dispose();
    }
}
