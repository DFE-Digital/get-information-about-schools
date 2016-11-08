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

namespace Edubase.Services
{
    public class CachedLookupService
    {
        private Dictionary<string, Func<ApplicationDbContext, object, Task<string>>> _mapping = null;

        private LookupService _svc = new LookupService();

        public CachedLookupService()
        {
            _mapping = new Dictionary<string, Func<ApplicationDbContext, object, Task<string>>>()
            {
                { "LocalAuthorityId", async (dc, id) => (await LocalAuthorityGetAllAsync()).FirstOrDefault(x=>x.Id == (int) id)?.Name },
                { "HeadTitleId", async (dc, id) => GetName((await HeadTitlesGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "GenderId", async (dc, id) => GetName((await GendersGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "EducationPhaseId", async (dc, id) => GetName((await EducationPhasesGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "AdmissionsPolicyId", async (dc, id) => GetName((await AdmissionsPoliciesGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "StatusId", async (dc, id) => GetName((await EstablishmentStatusesGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ReasonEstablishmentOpenedId", async (dc, id) => GetName((await ReasonEstablishmentOpenedGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ReasonEstablishmentClosedId", async (dc, id) => GetName((await ReasonEstablishmentClosedGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ProvisionBoardingId", async (dc, id) => GetName((await ProvisionBoardingGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ProvisionNurseryId", async (dc, id) => GetName((await ProvisionNurseriesGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ProvisionOfficialSixthFormId", async (dc, id) => GetName((await ProvisionOfficialSixthFormsGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ReligiousCharacterId", async (dc, id) => GetName((await ReligiousCharactersGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ReligiousEthosId", async (dc, id) => GetName((await ReligiousEthosGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "DioceseId", async (dc, id) => GetName((await DiocesesGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ProvisionSpecialClassesId", async (dc, id) => GetName((await ProvisionSpecialClassesGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "TypeId", async (dc, id) => GetName((await EstablishmentTypesGetAllAsync()).Cast<LookupBase>(), (int) id) }
            };
        }

        public async Task<List<LookupAdmissionsPolicy>> AdmissionsPoliciesGetAllAsync() => await Cacher.AutoAsync(_svc.AdmissionsPoliciesGetAllAsync);
        public List<LookupAdmissionsPolicy> AdmissionsPoliciesGetAll() => Cacher.Auto(_svc.AdmissionsPoliciesGetAll);

        public async Task<List<LocalAuthority>> LocalAuthorityGetAllAsync() => await Cacher.AutoAsync(_svc.LocalAuthorityGetAllAsync);
        public List<LocalAuthority> LocalAuthorityGetAll() => Cacher.Auto(_svc.LocalAuthorityGetAll);

        public async Task<List<LookupEducationPhase>> EducationPhasesGetAllAsync() => await Cacher.AutoAsync(_svc.EducationPhasesGetAllAsync);
        public List<LookupEducationPhase> EducationPhasesGetAll() => Cacher.Auto(_svc.EducationPhasesGetAll);

        public async Task<List<LookupEstablishmentStatus>> EstablishmentStatusesGetAllAsync() => await Cacher.AutoAsync(_svc.EstablishmentStatusesGetAllAsync);
        public List<LookupEstablishmentStatus> EstablishmentStatusesGetAll() => Cacher.Auto(_svc.EstablishmentStatusesGetAll);

        public async Task<List<LookupEstablishmentType>> EstablishmentTypesGetAllAsync() => await Cacher.AutoAsync(_svc.EstablishmentTypesGetAllAsync);
        public List<LookupEstablishmentType> EstablishmentTypesGetAll() => Cacher.Auto(_svc.EstablishmentTypesGetAll);

        public async Task<List<LookupGender>> GendersGetAllAsync() => await Cacher.AutoAsync(_svc.GendersGetAllAsync);
        public List<LookupGender> GendersGetAll() => Cacher.Auto(_svc.GendersGetAll);

        public async Task<List<LookupGroupType>> GroupTypesGetAllAsync() => await Cacher.AutoAsync(_svc.GroupTypesGetAllAsync);
        public List<LookupGroupType> GroupTypesGetAll() => Cacher.Auto(_svc.GroupTypesGetAll);

        public async Task<List<LookupHeadTitle>> HeadTitlesGetAllAsync() => await Cacher.AutoAsync(_svc.HeadTitlesGetAllAsync);
        public List<LookupHeadTitle> HeadTitlesGetAll() => Cacher.Auto(_svc.HeadTitlesGetAll);

        public async Task<List<LookupProvisionBoarding>> ProvisionBoardingGetAllAsync() => await Cacher.AutoAsync(_svc.ProvisionBoardingGetAllAsync);
        public List<LookupProvisionBoarding> ProvisionBoardingGetAll() => Cacher.Auto(_svc.ProvisionBoardingGetAll);

        public async Task<List<LookupProvisionNursery>> ProvisionNurseriesGetAllAsync() => await Cacher.AutoAsync(_svc.ProvisionNurseriesGetAllAsync);
        public List<LookupProvisionNursery> ProvisionNurseriesGetAll() => Cacher.Auto(_svc.ProvisionNurseriesGetAll);

        public async Task<List<LookupProvisionOfficialSixthForm>> ProvisionOfficialSixthFormsGetAllAsync() => await Cacher.AutoAsync(_svc.ProvisionOfficialSixthFormsGetAllAsync);
        public List<LookupProvisionOfficialSixthForm> ProvisionOfficialSixthFormsGetAll() => Cacher.Auto(_svc.ProvisionOfficialSixthFormsGetAll);

        public async Task<List<LookupProvisionSpecialClasses>> ProvisionSpecialClassesGetAllAsync() => await Cacher.AutoAsync(_svc.ProvisionSpecialClassesGetAllAsync);
        public List<LookupProvisionSpecialClasses> ProvisionSpecialClassesGetAll() => Cacher.Auto(_svc.ProvisionSpecialClassesGetAll);

        public async Task<List<LookupReasonEstablishmentClosed>> ReasonEstablishmentClosedGetAllAsync() => await Cacher.AutoAsync(_svc.ReasonEstablishmentClosedGetAllAsync);
        public List<LookupReasonEstablishmentClosed> ReasonEstablishmentClosedGetAll() => Cacher.Auto(_svc.ReasonEstablishmentClosedGetAll);

        public async Task<List<LookupReasonEstablishmentOpened>> ReasonEstablishmentOpenedGetAllAsync() => await Cacher.AutoAsync(_svc.ReasonEstablishmentOpenedGetAllAsync);
        public List<LookupReasonEstablishmentOpened> ReasonEstablishmentOpenedGetAll() => Cacher.Auto(_svc.ReasonEstablishmentOpenedGetAll);

        public async Task<List<LookupReligiousCharacter>> ReligiousCharactersGetAllAsync() => await Cacher.AutoAsync(_svc.ReligiousCharactersGetAllAsync);
        public List<LookupReligiousCharacter> ReligiousCharactersGetAll() => Cacher.Auto(_svc.ReligiousCharactersGetAll);

        public async Task<List<LookupReligiousEthos>> ReligiousEthosGetAllAsync() => await Cacher.AutoAsync(_svc.ReligiousEthosGetAllAsync);
        public List<LookupReligiousEthos> ReligiousEthosGetAll() => Cacher.Auto(_svc.ReligiousEthosGetAll);

        public async Task<List<LookupGovernorRole>> GovernorRolesGetAllAsync() => await Cacher.AutoAsync(_svc.GovernorRolesGetAllAsync);
        public List<LookupGovernorRole> GovernorRolesGetAll() => Cacher.Auto(_svc.GovernorRolesGetAll);

        public async Task<List<LookupGovernorAppointingBody>> GovernorAppointingBodiesGetAllAsync() => await Cacher.AutoAsync(_svc.GovernorAppointingBodiesGetAllAsync);
        public List<LookupGovernorAppointingBody> GovernorAppointingBodiesGetAll() => Cacher.Auto(_svc.GovernorAppointingBodiesGetAll);

        public async Task<List<LookupAccommodationChanged>> AccommodationChangedGetAllAsync() => await Cacher.AutoAsync(_svc.AccommodationChangedGetAllAsync);
        public List<LookupAccommodationChanged> AccommodationChangedGetAll() => Cacher.Auto(_svc.AccommodationChangedGetAll);

        public async Task<List<LookupBoardingEstablishment>> BoardingEstablishmentGetAllAsync() => await Cacher.AutoAsync(_svc.BoardingEstablishmentGetAllAsync);
        public List<LookupBoardingEstablishment> BoardingEstablishmentGetAll() => Cacher.Auto(_svc.BoardingEstablishmentGetAll);

        public async Task<List<LookupCCGovernance>> CCGovernanceGetAllAsync() => await Cacher.AutoAsync(_svc.CCGovernanceGetAllAsync);
        public List<LookupCCGovernance> CCGovernanceGetAll() => Cacher.Auto(_svc.CCGovernanceGetAll);

        public async Task<List<LookupCCOperationalHours>> CCOperationalHoursGetAllAsync() => await Cacher.AutoAsync(_svc.CCOperationalHoursGetAllAsync);
        public List<LookupCCOperationalHours> CCOperationalHoursGetAll() => Cacher.Auto(_svc.CCOperationalHoursGetAll);

        public async Task<List<LookupCCPhaseType>> CCPhaseTypesGetAllAsync() => await Cacher.AutoAsync(_svc.CCPhaseTypesGetAllAsync);
        public List<LookupCCPhaseType> CCPhaseTypesGetAll() => Cacher.Auto(_svc.CCPhaseTypesGetAll);

        public async Task<List<LookupDiocese>> DiocesesGetAllAsync() => await Cacher.AutoAsync(_svc.DiocesesGetAllAsync);
        public List<LookupDiocese> DiocesesGetAll() => Cacher.Auto(_svc.DiocesesGetAll);

        public async Task<List<LookupChildcareFacilities>> ChildcareFacilitiesGetAllAsync() => await Cacher.AutoAsync(_svc.ChildcareFacilitiesGetAllAsync);
        public List<LookupChildcareFacilities> ChildcareFacilitiesGetAll() => Cacher.Auto(_svc.ChildcareFacilitiesGetAll);

        public async Task<List<LookupDirectProvisionOfEarlyYears>> DirectProvisionOfEarlyYearsGetAllAsync() => await Cacher.AutoAsync(_svc.DirectProvisionOfEarlyYearsGetAllAsync);
        public List<LookupDirectProvisionOfEarlyYears> DirectProvisionOfEarlyYearsGetAll() => Cacher.Auto(_svc.DirectProvisionOfEarlyYearsGetAll);

        public async Task<List<LookupFurtherEducationType>> FurtherEducationTypesGetAllAsync() => await Cacher.AutoAsync(_svc.FurtherEducationTypesGetAllAsync);
        public List<LookupFurtherEducationType> FurtherEducationTypesGetAll() => Cacher.Auto(_svc.FurtherEducationTypesGetAll);

        public async Task<List<LookupIndependentSchoolType>> IndependentSchoolTypesGetAllAsync() => await Cacher.AutoAsync(_svc.IndependentSchoolTypesGetAllAsync);
        public List<LookupIndependentSchoolType> IndependentSchoolTypesGetAll() => Cacher.Auto(_svc.IndependentSchoolTypesGetAll);

        public async Task<List<LookupInspectorate>> InspectoratesGetAllAsync() => await Cacher.AutoAsync(_svc.InspectoratesGetAllAsync);
        public List<LookupInspectorate> InspectoratesGetAll() => Cacher.Auto(_svc.InspectoratesGetAll);

        public async Task<List<LookupInspectorateName>> InspectorateNamesGetAllAsync() => await Cacher.AutoAsync(_svc.InspectorateNamesGetAllAsync);
        public List<LookupInspectorateName> InspectorateNamesGetAll() => Cacher.Auto(_svc.InspectorateNamesGetAll);

        public async Task<List<LookupLocalGovernors>> LocalGovernorsGetAllAsync() => await Cacher.AutoAsync(_svc.LocalGovernorsGetAllAsync);
        public List<LookupLocalGovernors> LocalGovernorsGetAll() => Cacher.Auto(_svc.LocalGovernorsGetAll);

        public async Task<List<LookupNationality>> NationalitysGetAllAsync() => await Cacher.AutoAsync(_svc.NationalitysGetAllAsync);
        public List<LookupNationality> NationalitysGetAll() => Cacher.Auto(_svc.NationalitysGetAll);

        public async Task<List<LookupPRUEBD>> PRUEBDsGetAllAsync() => await Cacher.AutoAsync(_svc.PRUEBDsGetAllAsync);
        public List<LookupPRUEBD> PRUEBDsGetAll() => Cacher.Auto(_svc.PRUEBDsGetAll);

        public async Task<List<LookupPruEducatedByOthers>> PruEducatedByOtherssGetAllAsync() => await Cacher.AutoAsync(_svc.PruEducatedByOtherssGetAllAsync);
        public List<LookupPruEducatedByOthers> PruEducatedByOtherssGetAll() => Cacher.Auto(_svc.PruEducatedByOtherssGetAll);

        public async Task<List<LookupPruFulltimeProvision>> PruFulltimeProvisionsGetAllAsync() => await Cacher.AutoAsync(_svc.PruFulltimeProvisionsGetAllAsync);
        public List<LookupPruFulltimeProvision> PruFulltimeProvisionsGetAll() => Cacher.Auto(_svc.PruFulltimeProvisionsGetAll);

        public async Task<List<LookupPRUSEN>> PRUSENsGetAllAsync() => await Cacher.AutoAsync(_svc.PRUSENsGetAllAsync);
        public List<LookupPRUSEN> PRUSENsGetAll() => Cacher.Auto(_svc.PRUSENsGetAll);

        public async Task<List<LookupResourcedProvision>> ResourcedProvisionsGetAllAsync() => await Cacher.AutoAsync(_svc.ResourcedProvisionsGetAllAsync);
        public List<LookupResourcedProvision> ResourcedProvisionsGetAll() => Cacher.Auto(_svc.ResourcedProvisionsGetAll);

        public async Task<List<LookupSection41Approved>> Section41ApprovedsGetAllAsync() => await Cacher.AutoAsync(_svc.Section41ApprovedsGetAllAsync);
        public List<LookupSection41Approved> Section41ApprovedsGetAll() => Cacher.Auto(_svc.Section41ApprovedsGetAll);

        public async Task<List<LookupSpecialEducationNeeds>> SpecialEducationNeedsGetAllAsync() => await Cacher.AutoAsync(_svc.SpecialEducationNeedsGetAllAsync);
        public List<LookupSpecialEducationNeeds> SpecialEducationNeedsGetAll() => Cacher.Auto(_svc.SpecialEducationNeedsGetAll);

        public async Task<List<LookupTeenageMothersProvision>> TeenageMothersProvisionsGetAllAsync() => await Cacher.AutoAsync(_svc.TeenageMothersProvisionsGetAllAsync);
        public List<LookupTeenageMothersProvision> TeenageMothersProvisionsGetAll() => Cacher.Auto(_svc.TeenageMothersProvisionsGetAll);

        public async Task<List<LookupTypeOfResourcedProvision>> TypeOfResourcedProvisionsGetAllAsync() => await Cacher.AutoAsync(_svc.TypeOfResourcedProvisionsGetAllAsync);
        public List<LookupTypeOfResourcedProvision> TypeOfResourcedProvisionsGetAll() => Cacher.Auto(_svc.TypeOfResourcedProvisionsGetAll);


        public async Task<string> GetNameAsync(string lookupName, int id) => 
            await ApplicationDbContext.OperationAsync(async dc => await _mapping.Get(lookupName)?.Invoke(dc, id));

        public bool IsLookupField(string name) => _mapping.ContainsKey(name);

        private string GetName(IEnumerable<LookupBase> items, int id) => items.FirstOrDefault(x => x.Id == id)?.Name;

        public void Dispose() => _svc.Dispose();
    }
}
