using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Services.Enums;
using System.Runtime.Caching;
using Edubase.Common;
using Edubase.Services.Domain;

namespace Edubase.Services
{
    public class CachedLookupService : ICachedLookupService
    {
        private Dictionary<string, Func<ApplicationDbContext, int, Task<string>>> _mappingAsync = null;
        private Dictionary<string, Func<ApplicationDbContext, int, string>> _mapping = null;

        private LookupService _svc = new LookupService();

        public CachedLookupService()
        {
            _mappingAsync = new Dictionary<string, Func<ApplicationDbContext, int, Task<string>>>()
            {
                { "LocalAuthorityId", async (dc, id) => (await LocalAuthorityGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "RSCRegionId", async (dc, id) => (await LocalAuthorityGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "TypeId", async (dc, id) => (await EstablishmentTypesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "StatusId", async (dc, id) => (await EstablishmentStatusesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReasonEstablishmentOpenedId", async (dc, id) => (await ReasonEstablishmentOpenedGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReasonEstablishmentClosedId", async (dc, id) => (await ReasonEstablishmentClosedGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "EducationPhaseId", async (dc, id) => (await EducationPhasesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionBoardingId", async (dc, id) => (await ProvisionBoardingGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionNurseryId", async (dc, id) => (await ProvisionNurseriesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionOfficialSixthFormId", async (dc, id) => (await ProvisionOfficialSixthFormsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "GenderId", async (dc, id) => (await GendersGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReligiousCharacterId", async (dc, id) => (await ReligiousCharactersGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReligiousEthosId", async (dc, id) => (await ReligiousEthosGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "DioceseId", async (dc, id) => (await DiocesesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "AdmissionsPolicyId", async (dc, id) => (await AdmissionsPoliciesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionSpecialClassesId", async (dc, id) => (await ProvisionSpecialClassesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "HeadTitleId", async (dc, id) => (await HeadTitlesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "EstablishmentTypeGroupId", async (dc, id) => (await EstablishmentTypeGroupsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "InspectorateId", async (dc, id) => (await InspectoratesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "Section41ApprovedId", async (dc, id) => (await Section41ApprovedGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "SEN1Id", async (dc, id) => (await SpecialEducationNeedsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "SEN2Id", async (dc, id) => (await SpecialEducationNeedsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "SEN3Id", async (dc, id) => (await SpecialEducationNeedsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "SEN4Id", async (dc, id) => (await SpecialEducationNeedsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "TeenageMothersProvisionId", async (dc, id) => (await TeenageMothersProvisionsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ChildcareFacilitiesId", async (dc, id) => (await ChildcareFacilitiesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "PRUSENId", async (dc, id) => (await PRUSENsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "PRUEBDId", async (dc, id) => (await PRUEBDsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "PruFulltimeProvisionId", async (dc, id) => (await PruFulltimeProvisionsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "PruEducatedByOthersId", async (dc, id) => (await PruEducatedByOthersGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "TypeOfResourcedProvisionId", async (dc, id) => (await TypeOfResourcedProvisionsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "GovernmentOfficeRegionId", async (dc, id) => (await GovernmentOfficeRegionsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "AdministrativeDistrictId", async (dc, id) => (await AdministrativeDistrictsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "AdministrativeWardId", async (dc, id) => (await AdministrativeWardsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ParliamentaryConstituencyId", async (dc, id) => (await ParliamentaryConstituenciesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "UrbanRuralId", async (dc, id) => (await UrbanRuralGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "GSSLAId", async (dc, id) => (await GSSLAGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CASWardId", async (dc, id) => (await CASWardsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "MSOAId", async (dc, id) => (await MSOAsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "LSOAId", async (dc, id) => (await LSOAsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "FurtherEducationTypeId", async (dc, id) => (await FurtherEducationTypesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "InspectorateNameId", async (dc, id) => (await InspectorateNamesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCOperationalHoursId", async (dc, id) => (await CCOperationalHoursGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCGovernanceId", async (dc, id) => (await CCGovernanceGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCPhaseTypeId", async (dc, id) => (await CCPhaseTypesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCDisadvantagedAreaId", async (dc, id) => (await CCDisadvantagedAreasGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCDirectProvisionOfEarlyYearsId", async (dc, id) => (await DirectProvisionOfEarlyYearsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
            };

            _mapping = new Dictionary<string, Func<ApplicationDbContext, int, string>>()
            {
                { "LocalAuthorityId",  (dc, id) => ( LocalAuthorityGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "RSCRegionId",  (dc, id) => ( LocalAuthorityGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "TypeId",  (dc, id) => ( EstablishmentTypesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "StatusId",  (dc, id) => ( EstablishmentStatusesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReasonEstablishmentOpenedId",  (dc, id) => ( ReasonEstablishmentOpenedGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReasonEstablishmentClosedId",  (dc, id) => ( ReasonEstablishmentClosedGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "EducationPhaseId",  (dc, id) => ( EducationPhasesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionBoardingId",  (dc, id) => ( ProvisionBoardingGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionNurseryId",  (dc, id) => ( ProvisionNurseriesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionOfficialSixthFormId",  (dc, id) => ( ProvisionOfficialSixthFormsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "GenderId",  (dc, id) => ( GendersGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReligiousCharacterId",  (dc, id) => ( ReligiousCharactersGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReligiousEthosId",  (dc, id) => ( ReligiousEthosGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "DioceseId",  (dc, id) => ( DiocesesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "AdmissionsPolicyId",  (dc, id) => ( AdmissionsPoliciesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionSpecialClassesId",  (dc, id) => ( ProvisionSpecialClassesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "HeadTitleId",  (dc, id) => ( HeadTitlesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "EstablishmentTypeGroupId",  (dc, id) => ( EstablishmentTypeGroupsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "InspectorateId",  (dc, id) => ( InspectoratesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "Section41ApprovedId",  (dc, id) => ( Section41ApprovedGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "SEN1Id",  (dc, id) => ( SpecialEducationNeedsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "SEN2Id",  (dc, id) => ( SpecialEducationNeedsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "SEN3Id",  (dc, id) => ( SpecialEducationNeedsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "SEN4Id",  (dc, id) => ( SpecialEducationNeedsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "TeenageMothersProvisionId",  (dc, id) => ( TeenageMothersProvisionsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ChildcareFacilitiesId",  (dc, id) => ( ChildcareFacilitiesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "PRUSENId",  (dc, id) => ( PRUSENsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "PRUEBDId",  (dc, id) => ( PRUEBDsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "PruFulltimeProvisionId",  (dc, id) => ( PruFulltimeProvisionsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "PruEducatedByOthersId",  (dc, id) => ( PruEducatedByOthersGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "TypeOfResourcedProvisionId",  (dc, id) => ( TypeOfResourcedProvisionsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "GovernmentOfficeRegionId",  (dc, id) => ( GovernmentOfficeRegionsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "AdministrativeDistrictId",  (dc, id) => ( AdministrativeDistrictsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "AdministrativeWardId",  (dc, id) => ( AdministrativeWardsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ParliamentaryConstituencyId",  (dc, id) => ( ParliamentaryConstituenciesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "UrbanRuralId",  (dc, id) => ( UrbanRuralGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "GSSLAId",  (dc, id) => ( GSSLAGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CASWardId",  (dc, id) => ( CASWardsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "MSOAId",  (dc, id) => ( MSOAsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "LSOAId",  (dc, id) => ( LSOAsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "FurtherEducationTypeId",  (dc, id) => ( FurtherEducationTypesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "InspectorateNameId",  (dc, id) => ( InspectorateNamesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCOperationalHoursId",  (dc, id) => ( CCOperationalHoursGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCGovernanceId",  (dc, id) => ( CCGovernanceGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCPhaseTypeId",  (dc, id) => ( CCPhaseTypesGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCDisadvantagedAreaId",  (dc, id) => ( CCDisadvantagedAreasGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCDirectProvisionOfEarlyYearsId",  (dc, id) => ( DirectProvisionOfEarlyYearsGetAll()).FirstOrDefault(x=>x.Id == id)?.Name },
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
        public async Task<IEnumerable<LookupDto>> CCDisadvantagedAreasGetAllAsync() => await Cacher.AutoAsync(_svc.CCDisadvantagedAreasGetAllAsync);
        public IEnumerable<LookupDto> CCDisadvantagedAreasGetAll() => Cacher.Auto(_svc.CCDisadvantagedAreasGetAll);
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
        public async Task<IEnumerable<LookupDto>> EstablishmentTypeGroupsGetAllAsync() => await Cacher.AutoAsync(_svc.EstablishmentTypeGroupsGetAllAsync);
        public IEnumerable<LookupDto> EstablishmentTypeGroupsGetAll() => Cacher.Auto(_svc.EstablishmentTypeGroupsGetAll);

        public async Task<IEnumerable<LookupDto>> GovernmentOfficeRegionsGetAllAsync() => await Cacher.AutoAsync(_svc.GovernmentOfficeRegionsGetAllAsync);
        public IEnumerable<LookupDto> GovernmentOfficeRegionsGetAll() => Cacher.Auto(_svc.GovernmentOfficeRegionsGetAll);

        public async Task<IEnumerable<LookupDto>> AdministrativeDistrictsGetAllAsync() => await Cacher.AutoAsync(_svc.AdministrativeDistrictsGetAllAsync);
        public IEnumerable<LookupDto> AdministrativeDistrictsGetAll() => Cacher.Auto(_svc.AdministrativeDistrictsGetAll);

        public async Task<IEnumerable<LookupDto>> AdministrativeWardsGetAllAsync() => await Cacher.AutoAsync(_svc.AdministrativeWardsGetAllAsync);
        public IEnumerable<LookupDto> AdministrativeWardsGetAll() => Cacher.Auto(_svc.AdministrativeWardsGetAll);


        public async Task<IEnumerable<LookupDto>> ParliamentaryConstituenciesGetAllAsync() => await Cacher.AutoAsync(_svc.ParliamentaryConstituenciesGetAllAsync);
        public IEnumerable<LookupDto> ParliamentaryConstituenciesGetAll() => Cacher.Auto(_svc.ParliamentaryConstituenciesGetAll);


        public async Task<IEnumerable<LookupDto>> UrbanRuralGetAllAsync() => await Cacher.AutoAsync(_svc.UrbanRuralGetAllAsync);
        public IEnumerable<LookupDto> UrbanRuralGetAll() => Cacher.Auto(_svc.UrbanRuralGetAll);

        public async Task<IEnumerable<LookupDto>> GSSLAGetAllAsync() => await Cacher.AutoAsync(_svc.GSSLAGetAllAsync);
        public IEnumerable<LookupDto> GSSLAGetAll() => Cacher.Auto(_svc.GSSLAGetAll);
        public async Task<IEnumerable<LookupDto>> CASWardsGetAllAsync() => await Cacher.AutoAsync(_svc.CASWardsGetAllAsync);
        public IEnumerable<LookupDto> CASWardsGetAll() => Cacher.Auto(_svc.CASWardsGetAll);


        public async Task<IEnumerable<LookupDto>> MSOAsGetAllAsync() => await Cacher.AutoAsync(_svc.MSOAsGetAllAsync);
        public IEnumerable<LookupDto> MSOAsGetAll() => Cacher.Auto(_svc.MSOAsGetAll);

        public async Task<IEnumerable<LookupDto>> LSOAsGetAllAsync() => await Cacher.AutoAsync(_svc.LSOAsGetAllAsync);
        public IEnumerable<LookupDto> LSOAsGetAll() => Cacher.Auto(_svc.LSOAsGetAll);

        public async Task<IEnumerable<LookupDto>> GroupStatusesGetAllAsync() => await Cacher.AutoAsync(_svc.GroupStatusesGetAllAsync);
        public IEnumerable<LookupDto> GroupStatusesGetAll() => Cacher.Auto(_svc.GroupStatusesGetAll);
        
        public async Task<string> GetNameAsync(string lookupName, int id) => 
            await ApplicationDbContext.OperationAsync(async dc => await _mappingAsync.Get(lookupName)?.Invoke(dc, id));
        public string GetName(string lookupName, int id) => ApplicationDbContext.Operation(dc => _mapping.Get(lookupName)?.Invoke(dc, id));

        public bool IsLookupField(string name) => _mapping.ContainsKey(name);

        private string GetName(IEnumerable<LookupDto> items, int id) => items.FirstOrDefault(x => x.Id == id)?.Name;

        public void Dispose() => _svc.Dispose();
    }
}
