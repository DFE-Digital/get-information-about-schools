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
using Edubase.Data.DbContext;
using Edubase.Common.Cache;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace Edubase.Services.Lookup
{
    /// <summary>
    /// TODO: TEXCHANGE: Establish which lookups are immutable.  May need to remove mutable lookups, or develop a way to enable cache invalidation from the back-end via messaging etc.
    /// </summary>
    public class CachedLookupService : ICachedLookupService
    {
        Dictionary<string, Func<int, Task<string>>> _mappingAsync = null;
        Dictionary<string, Func<int, string>> _mapping = null;
        ILookupService _lookupService;
        ICacheAccessor _cacheAccessor;

        public CachedLookupService(ILookupService lookupService, ICacheAccessor cacheAccessor)
        {
            _lookupService = lookupService;
            _cacheAccessor = cacheAccessor;
            _mappingAsync = new Dictionary<string, Func<int, Task<string>>>()
            {
                { "LocalAuthorityId", async id => (await LocalAuthorityGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "RSCRegionId", async id => (await LocalAuthorityGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "TypeId", async id => (await EstablishmentTypesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "GroupTypeId", async id => (await GroupTypesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "Group.StatusId", async id => (await GroupStatusesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "StatusId", async id => (await EstablishmentStatusesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReasonEstablishmentOpenedId", async id => (await ReasonEstablishmentOpenedGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReasonEstablishmentClosedId", async id => (await ReasonEstablishmentClosedGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "IndependentSchoolTypeId", async id => (await IndependentSchoolTypesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "EducationPhaseId", async id => (await EducationPhasesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionBoardingId", async id => (await ProvisionBoardingGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionNurseryId", async id => (await ProvisionNurseriesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionOfficialSixthFormId", async id => (await ProvisionOfficialSixthFormsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "GenderId", async id => (await GendersGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReligiousCharacterId", async id => (await ReligiousCharactersGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReligiousEthosId", async id => (await ReligiousEthosGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "DioceseId", async id => (await DiocesesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "AdmissionsPolicyId", async id => (await AdmissionsPoliciesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionSpecialClassesId", async id => (await ProvisionSpecialClassesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "HeadTitleId", async id => (await HeadTitlesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "EstablishmentTypeGroupId", async id => (await EstablishmentTypeGroupsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "InspectorateId", async id => (await InspectoratesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "Section41ApprovedId", async id => (await Section41ApprovedGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "SEN1Id", async id => (await SpecialEducationNeedsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "SEN2Id", async id => (await SpecialEducationNeedsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "SEN3Id", async id => (await SpecialEducationNeedsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "SEN4Id", async id => (await SpecialEducationNeedsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "TeenageMothersProvisionId", async id => (await TeenageMothersProvisionsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ChildcareFacilitiesId", async id => (await ChildcareFacilitiesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "PRUSENId", async id => (await PRUSENsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "PRUEBDId", async id => (await PRUEBDsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "PruFulltimeProvisionId", async id => (await PruFulltimeProvisionsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "PruEducatedByOthersId", async id => (await PruEducatedByOthersGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "TypeOfResourcedProvisionId", async id => (await TypeOfResourcedProvisionsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "GovernmentOfficeRegionId", async id => (await GovernmentOfficeRegionsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "AdministrativeDistrictId", async id => (await AdministrativeDistrictsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "AdministrativeWardId", async id => (await AdministrativeWardsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ParliamentaryConstituencyId", async id => (await ParliamentaryConstituenciesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "UrbanRuralId", async id => (await UrbanRuralGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "GSSLAId", async id => (await GSSLAGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CASWardId", async id => (await CASWardsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "MSOAId", async id => (await MSOAsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "LSOAId", async id => (await LSOAsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "FurtherEducationTypeId", async id => (await FurtherEducationTypesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "InspectorateNameId", async id => (await InspectorateNamesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "BSOInspectorateId", async id => (await InspectorateNamesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCOperationalHoursId", async id => (await CCOperationalHoursGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCGovernanceId", async id => (await CCGovernanceGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCPhaseTypeId", async id => (await CCPhaseTypesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCDisadvantagedAreaId", async id => (await CCDisadvantagedAreasGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCDirectProvisionOfEarlyYearsId", async id => (await DirectProvisionOfEarlyYearsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCDeliveryModelId", async id => (await CCDeliveryModelsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CCGroupLeadId", async id => (await CCGroupLeadsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "LinkTypeId", async id => (await EstablishmentLinkTypesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
            };

            _mapping = new Dictionary<string, Func<int, string>>()
            {
                { "LocalAuthorityId",  id => LocalAuthorityGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "RSCRegionId",  id => LocalAuthorityGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "TypeId",  id => EstablishmentTypesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "GroupTypeId",  id => GroupTypesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "Group.StatusId",  id => GroupStatusesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "StatusId",  id => EstablishmentStatusesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "ReasonEstablishmentOpenedId",  id => ReasonEstablishmentOpenedGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "ReasonEstablishmentClosedId",  id => ReasonEstablishmentClosedGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "EducationPhaseId",  id => EducationPhasesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "IndependentSchoolTypeId",  id => IndependentSchoolTypesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "ProvisionBoardingId",  id => ProvisionBoardingGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "ProvisionNurseryId",  id => ProvisionNurseriesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "ProvisionOfficialSixthFormId",  id => ProvisionOfficialSixthFormsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "GenderId",  id => GendersGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "ReligiousCharacterId",  id => ReligiousCharactersGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "ReligiousEthosId",  id => ReligiousEthosGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "DioceseId",  id => DiocesesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "AdmissionsPolicyId",  id => AdmissionsPoliciesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "ProvisionSpecialClassesId",  id => ProvisionSpecialClassesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "HeadTitleId",  id => HeadTitlesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "EstablishmentTypeGroupId",  id => EstablishmentTypeGroupsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "InspectorateId",  id => InspectoratesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "Section41ApprovedId",  id => Section41ApprovedGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "SEN1Id",  id => SpecialEducationNeedsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "SEN2Id",  id => SpecialEducationNeedsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "SEN3Id",  id => SpecialEducationNeedsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "SEN4Id",  id => SpecialEducationNeedsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "TeenageMothersProvisionId",  id => TeenageMothersProvisionsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "ChildcareFacilitiesId",  id => ChildcareFacilitiesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "PRUSENId",  id => PRUSENsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "PRUEBDId",  id => PRUEBDsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "PruFulltimeProvisionId",  id => PruFulltimeProvisionsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "PruEducatedByOthersId",  id => PruEducatedByOthersGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "TypeOfResourcedProvisionId",  id => TypeOfResourcedProvisionsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "GovernmentOfficeRegionId",  id => GovernmentOfficeRegionsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "AdministrativeDistrictId",  id => AdministrativeDistrictsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "AdministrativeWardId",  id => AdministrativeWardsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "ParliamentaryConstituencyId",  id => ParliamentaryConstituenciesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "UrbanRuralId",  id => UrbanRuralGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "GSSLAId",  id => GSSLAGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "CASWardId",  id => CASWardsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "MSOAId",  id => MSOAsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "LSOAId",  id => LSOAsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "FurtherEducationTypeId",  id => FurtherEducationTypesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "InspectorateNameId",  id => InspectorateNamesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "BSOInspectorateId",  id => InspectorateNamesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "CCOperationalHoursId",  id => CCOperationalHoursGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "CCGovernanceId",  id => CCGovernanceGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "CCPhaseTypeId",  id => CCPhaseTypesGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "CCDisadvantagedAreaId",  id => CCDisadvantagedAreasGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "CCDirectProvisionOfEarlyYearsId",  id => DirectProvisionOfEarlyYearsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "CCDeliveryModelId",  id => CCDeliveryModelsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
                { "CCGroupLeadId",  id => CCGroupLeadsGetAll().FirstOrDefault(x => x.Id == id)?.Name },
            };
        }

        public async Task<IEnumerable<LookupDto>> LocalAuthorityGetAllAsync() => await AutoAsync(_lookupService.LocalAuthorityGetAllAsync);
        public IEnumerable<LookupDto> LocalAuthorityGetAll() => Auto(_lookupService.LocalAuthorityGetAll);
        public async Task<IEnumerable<LookupDto>> AdmissionsPoliciesGetAllAsync() => await AutoAsync(_lookupService.AdmissionsPoliciesGetAllAsync);
        public IEnumerable<LookupDto> AdmissionsPoliciesGetAll() => Auto(_lookupService.AdmissionsPoliciesGetAll);
        public async Task<IEnumerable<LookupDto>> EducationPhasesGetAllAsync() => await AutoAsync(_lookupService.EducationPhasesGetAllAsync);
        public IEnumerable<LookupDto> EducationPhasesGetAll() => Auto(_lookupService.EducationPhasesGetAll);
        public async Task<IEnumerable<LookupDto>> EstablishmentStatusesGetAllAsync() => await AutoAsync(_lookupService.EstablishmentStatusesGetAllAsync);
        public IEnumerable<LookupDto> EstablishmentStatusesGetAll() => Auto(_lookupService.EstablishmentStatusesGetAll);
        public async Task<IEnumerable<LookupDto>> EstablishmentTypesGetAllAsync() => await AutoAsync(_lookupService.EstablishmentTypesGetAllAsync);
        public IEnumerable<LookupDto> EstablishmentTypesGetAll() => Auto(_lookupService.EstablishmentTypesGetAll);
        public async Task<IEnumerable<LookupDto>> GendersGetAllAsync() => await AutoAsync(_lookupService.GendersGetAllAsync);
        public IEnumerable<LookupDto> GendersGetAll() => Auto(_lookupService.GendersGetAll);
        public async Task<IEnumerable<LookupDto>> GroupTypesGetAllAsync() => await AutoAsync(_lookupService.GroupTypesGetAllAsync);
        public IEnumerable<LookupDto> GroupTypesGetAll() => Auto(_lookupService.GroupTypesGetAll);
        public async Task<IEnumerable<LookupDto>> HeadTitlesGetAllAsync() => await AutoAsync(_lookupService.HeadTitlesGetAllAsync);
        public IEnumerable<LookupDto> HeadTitlesGetAll() => Auto(_lookupService.HeadTitlesGetAll);
        public async Task<IEnumerable<LookupDto>> ProvisionBoardingGetAllAsync() => await AutoAsync(_lookupService.ProvisionBoardingGetAllAsync);
        public IEnumerable<LookupDto> ProvisionBoardingGetAll() => Auto(_lookupService.ProvisionBoardingGetAll);
        public async Task<IEnumerable<LookupDto>> ProvisionNurseriesGetAllAsync() => await AutoAsync(_lookupService.ProvisionNurseriesGetAllAsync);
        public IEnumerable<LookupDto> ProvisionNurseriesGetAll() => Auto(_lookupService.ProvisionNurseriesGetAll);
        public async Task<IEnumerable<LookupDto>> ProvisionOfficialSixthFormsGetAllAsync() => await AutoAsync(_lookupService.ProvisionOfficialSixthFormsGetAllAsync);
        public IEnumerable<LookupDto> ProvisionOfficialSixthFormsGetAll() => Auto(_lookupService.ProvisionOfficialSixthFormsGetAll);
        public async Task<IEnumerable<LookupDto>> ProvisionSpecialClassesGetAllAsync() => await AutoAsync(_lookupService.ProvisionSpecialClassesGetAllAsync);
        public IEnumerable<LookupDto> ProvisionSpecialClassesGetAll() => Auto(_lookupService.ProvisionSpecialClassesGetAll);
        public async Task<IEnumerable<LookupDto>> ReasonEstablishmentClosedGetAllAsync() => await AutoAsync(_lookupService.ReasonEstablishmentClosedGetAllAsync);
        public IEnumerable<LookupDto> ReasonEstablishmentClosedGetAll() => Auto(_lookupService.ReasonEstablishmentClosedGetAll);
        public async Task<IEnumerable<LookupDto>> ReasonEstablishmentOpenedGetAllAsync() => await AutoAsync(_lookupService.ReasonEstablishmentOpenedGetAllAsync);
        public IEnumerable<LookupDto> ReasonEstablishmentOpenedGetAll() => Auto(_lookupService.ReasonEstablishmentOpenedGetAll);
        public async Task<IEnumerable<LookupDto>> ReligiousCharactersGetAllAsync() => await AutoAsync(_lookupService.ReligiousCharactersGetAllAsync);
        public IEnumerable<LookupDto> ReligiousCharactersGetAll() => Auto(_lookupService.ReligiousCharactersGetAll);
        public async Task<IEnumerable<LookupDto>> ReligiousEthosGetAllAsync() => await AutoAsync(_lookupService.ReligiousEthosGetAllAsync);
        public IEnumerable<LookupDto> ReligiousEthosGetAll() => Auto(_lookupService.ReligiousEthosGetAll);
        public async Task<IEnumerable<LookupDto>> GovernorRolesGetAllAsync() => await AutoAsync(_lookupService.GovernorRolesGetAllAsync);
        public IEnumerable<LookupDto> GovernorRolesGetAll() => Auto(_lookupService.GovernorRolesGetAll);
        public async Task<IEnumerable<LookupDto>> GovernorAppointingBodiesGetAllAsync() => await AutoAsync(_lookupService.GovernorAppointingBodiesGetAllAsync);
        public IEnumerable<LookupDto> GovernorAppointingBodiesGetAll() => Auto(_lookupService.GovernorAppointingBodiesGetAll);
        public async Task<IEnumerable<LookupDto>> AccommodationChangedGetAllAsync() => await AutoAsync(_lookupService.AccommodationChangedGetAllAsync);
        public IEnumerable<LookupDto> AccommodationChangedGetAll() => Auto(_lookupService.AccommodationChangedGetAll);
        public async Task<IEnumerable<LookupDto>> BoardingEstablishmentGetAllAsync() => await AutoAsync(_lookupService.BoardingEstablishmentGetAllAsync);
        public IEnumerable<LookupDto> BoardingEstablishmentGetAll() => Auto(_lookupService.BoardingEstablishmentGetAll);
        public async Task<IEnumerable<LookupDto>> CCGovernanceGetAllAsync() => await AutoAsync(_lookupService.CCGovernanceGetAllAsync);
        public IEnumerable<LookupDto> CCGovernanceGetAll() => Auto(_lookupService.CCGovernanceGetAll);
        public async Task<IEnumerable<LookupDto>> CCOperationalHoursGetAllAsync() => await AutoAsync(_lookupService.CCOperationalHoursGetAllAsync);
        public IEnumerable<LookupDto> CCOperationalHoursGetAll() => Auto(_lookupService.CCOperationalHoursGetAll);
        public async Task<IEnumerable<LookupDto>> CCDisadvantagedAreasGetAllAsync() => await AutoAsync(_lookupService.CCDisadvantagedAreasGetAllAsync);
        public IEnumerable<LookupDto> CCDisadvantagedAreasGetAll() => Auto(_lookupService.CCDisadvantagedAreasGetAll);
        public async Task<IEnumerable<LookupDto>> CCPhaseTypesGetAllAsync() => await AutoAsync(_lookupService.CCPhaseTypesGetAllAsync);
        public IEnumerable<LookupDto> CCPhaseTypesGetAll() => Auto(_lookupService.CCPhaseTypesGetAll);
        public async Task<IEnumerable<LookupDto>> CCGroupLeadsGetAllAsync() => await AutoAsync(_lookupService.CCGroupLeadsGetAllAsync);
        public IEnumerable<LookupDto> CCGroupLeadsGetAll() => Auto(_lookupService.CCGroupLeadsGetAll);
        public async Task<IEnumerable<LookupDto>> CCDeliveryModelsGetAllAsync() => await AutoAsync(_lookupService.CCDeliveryModelsGetAllAsync);
        public IEnumerable<LookupDto> CCDeliveryModelsGetAll() => Auto(_lookupService.CCDeliveryModelsGetAll);
        public async Task<IEnumerable<LookupDto>> DiocesesGetAllAsync() => await AutoAsync(_lookupService.DiocesesGetAllAsync);
        public IEnumerable<LookupDto> DiocesesGetAll() => Auto(_lookupService.DiocesesGetAll);
        public async Task<IEnumerable<LookupDto>> ChildcareFacilitiesGetAllAsync() => await AutoAsync(_lookupService.ChildcareFacilitiesGetAllAsync);
        public IEnumerable<LookupDto> ChildcareFacilitiesGetAll() => Auto(_lookupService.ChildcareFacilitiesGetAll);
        public async Task<IEnumerable<LookupDto>> DirectProvisionOfEarlyYearsGetAllAsync() => await AutoAsync(_lookupService.DirectProvisionOfEarlyYearsGetAllAsync);
        public IEnumerable<LookupDto> DirectProvisionOfEarlyYearsGetAll() => Auto(_lookupService.DirectProvisionOfEarlyYearsGetAll);
        public async Task<IEnumerable<LookupDto>> FurtherEducationTypesGetAllAsync() => await AutoAsync(_lookupService.FurtherEducationTypesGetAllAsync);
        public IEnumerable<LookupDto> FurtherEducationTypesGetAll() => Auto(_lookupService.FurtherEducationTypesGetAll);
        public async Task<IEnumerable<LookupDto>> IndependentSchoolTypesGetAllAsync() => await AutoAsync(_lookupService.IndependentSchoolTypesGetAllAsync);
        public IEnumerable<LookupDto> IndependentSchoolTypesGetAll() => Auto(_lookupService.IndependentSchoolTypesGetAll);
        public async Task<IEnumerable<LookupDto>> InspectoratesGetAllAsync() => await AutoAsync(_lookupService.InspectoratesGetAllAsync);
        public IEnumerable<LookupDto> InspectoratesGetAll() => Auto(_lookupService.InspectoratesGetAll);
        public async Task<IEnumerable<LookupDto>> InspectorateNamesGetAllAsync() => await AutoAsync(_lookupService.InspectorateNamesGetAllAsync);
        public IEnumerable<LookupDto> InspectorateNamesGetAll() => Auto(_lookupService.InspectorateNamesGetAll);
        public async Task<IEnumerable<LookupDto>> LocalGovernorsGetAllAsync() => await AutoAsync(_lookupService.LocalGovernorsGetAllAsync);
        public IEnumerable<LookupDto> LocalGovernorsGetAll() => Auto(_lookupService.LocalGovernorsGetAll);
        public async Task<IEnumerable<LookupDto>> NationalitiesGetAllAsync() => await AutoAsync(_lookupService.NationalitiesGetAllAsync);
        public IEnumerable<LookupDto> NationalitiesGetAll() => Auto(_lookupService.NationalitiesGetAll);
        public async Task<IEnumerable<LookupDto>> PRUEBDsGetAllAsync() => await AutoAsync(_lookupService.PRUEBDsGetAllAsync);
        public IEnumerable<LookupDto> PRUEBDsGetAll() => Auto(_lookupService.PRUEBDsGetAll);
        public async Task<IEnumerable<LookupDto>> PruEducatedByOthersGetAllAsync() => await AutoAsync(_lookupService.PruEducatedByOthersGetAllAsync);
        public IEnumerable<LookupDto> PruEducatedByOthersGetAll() => Auto(_lookupService.PruEducatedByOthersGetAll);
        public async Task<IEnumerable<LookupDto>> PruFulltimeProvisionsGetAllAsync() => await AutoAsync(_lookupService.PruFulltimeProvisionsGetAllAsync);
        public IEnumerable<LookupDto> PruFulltimeProvisionsGetAll() => Auto(_lookupService.PruFulltimeProvisionsGetAll);
        public async Task<IEnumerable<LookupDto>> PRUSENsGetAllAsync() => await AutoAsync(_lookupService.PRUSENsGetAllAsync);
        public IEnumerable<LookupDto> PRUSENsGetAll() => Auto(_lookupService.PRUSENsGetAll);
        public async Task<IEnumerable<LookupDto>> ResourcedProvisionsGetAllAsync() => await AutoAsync(_lookupService.ResourcedProvisionsGetAllAsync);
        public IEnumerable<LookupDto> ResourcedProvisionsGetAll() => Auto(_lookupService.ResourcedProvisionsGetAll);
        public async Task<IEnumerable<LookupDto>> Section41ApprovedGetAllAsync() => await AutoAsync(_lookupService.Section41ApprovedGetAllAsync);
        public IEnumerable<LookupDto> Section41ApprovedGetAll() => Auto(_lookupService.Section41ApprovedGetAll);
        public async Task<IEnumerable<LookupDto>> SpecialEducationNeedsGetAllAsync() => await AutoAsync(_lookupService.SpecialEducationNeedsGetAllAsync);
        public IEnumerable<LookupDto> SpecialEducationNeedsGetAll() => Auto(_lookupService.SpecialEducationNeedsGetAll);
        public async Task<IEnumerable<LookupDto>> TeenageMothersProvisionsGetAllAsync() => await AutoAsync(_lookupService.TeenageMothersProvisionsGetAllAsync);
        public IEnumerable<LookupDto> TeenageMothersProvisionsGetAll() => Auto(_lookupService.TeenageMothersProvisionsGetAll);
        public async Task<IEnumerable<LookupDto>> TypeOfResourcedProvisionsGetAllAsync() => await AutoAsync(_lookupService.TypeOfResourcedProvisionsGetAllAsync);
        public IEnumerable<LookupDto> TypeOfResourcedProvisionsGetAll() => Auto(_lookupService.TypeOfResourcedProvisionsGetAll);
        public async Task<IEnumerable<LookupDto>> EstablishmentLinkTypesGetAllAsync() => await AutoAsync(_lookupService.EstablishmentLinkTypesGetAllAsync);
        public IEnumerable<LookupDto> EstablishmentLinkTypesGetAll() => Auto(_lookupService.EstablishmentLinkTypesGetAll);
        public async Task<IEnumerable<LookupDto>> EstablishmentTypeGroupsGetAllAsync() => await AutoAsync(_lookupService.EstablishmentTypeGroupsGetAllAsync);
        public IEnumerable<LookupDto> EstablishmentTypeGroupsGetAll() => Auto(_lookupService.EstablishmentTypeGroupsGetAll);

        public async Task<IEnumerable<LookupDto>> GovernmentOfficeRegionsGetAllAsync() => await AutoAsync(_lookupService.GovernmentOfficeRegionsGetAllAsync);
        public IEnumerable<LookupDto> GovernmentOfficeRegionsGetAll() => Auto(_lookupService.GovernmentOfficeRegionsGetAll);

        public async Task<IEnumerable<LookupDto>> AdministrativeDistrictsGetAllAsync() => await AutoAsync(_lookupService.AdministrativeDistrictsGetAllAsync);
        public IEnumerable<LookupDto> AdministrativeDistrictsGetAll() => Auto(_lookupService.AdministrativeDistrictsGetAll);

        public async Task<IEnumerable<LookupDto>> AdministrativeWardsGetAllAsync() => await AutoAsync(_lookupService.AdministrativeWardsGetAllAsync);
        public IEnumerable<LookupDto> AdministrativeWardsGetAll() => Auto(_lookupService.AdministrativeWardsGetAll);


        public async Task<IEnumerable<LookupDto>> ParliamentaryConstituenciesGetAllAsync() => await AutoAsync(_lookupService.ParliamentaryConstituenciesGetAllAsync);
        public IEnumerable<LookupDto> ParliamentaryConstituenciesGetAll() => Auto(_lookupService.ParliamentaryConstituenciesGetAll);


        public async Task<IEnumerable<LookupDto>> UrbanRuralGetAllAsync() => await AutoAsync(_lookupService.UrbanRuralGetAllAsync);
        public IEnumerable<LookupDto> UrbanRuralGetAll() => Auto(_lookupService.UrbanRuralGetAll);

        public async Task<IEnumerable<LookupDto>> GSSLAGetAllAsync() => await AutoAsync(_lookupService.GSSLAGetAllAsync);
        public IEnumerable<LookupDto> GSSLAGetAll() => Auto(_lookupService.GSSLAGetAll);
        public async Task<IEnumerable<LookupDto>> CASWardsGetAllAsync() => await AutoAsync(_lookupService.CASWardsGetAllAsync);
        public IEnumerable<LookupDto> CASWardsGetAll() => Auto(_lookupService.CASWardsGetAll);


        public async Task<IEnumerable<LookupDto>> MSOAsGetAllAsync() => await AutoAsync(_lookupService.MSOAsGetAllAsync);

        public IEnumerable<LookupDto> MSOAsGetAll() => Auto(_lookupService.MSOAsGetAll);

        public async Task<IEnumerable<LookupDto>> LSOAsGetAllAsync() => await AutoAsync(_lookupService.LSOAsGetAllAsync);

        public IEnumerable<LookupDto> LSOAsGetAll() => Auto(_lookupService.LSOAsGetAll);

        public async Task<IEnumerable<LookupDto>> GroupStatusesGetAllAsync() => await AutoAsync(_lookupService.GroupStatusesGetAllAsync);

        public IEnumerable<LookupDto> GroupStatusesGetAll() => Auto(_lookupService.GroupStatusesGetAll);

        public async Task<string> GetNameAsync(string lookupName, int? id, string domain = null)
        {
            if (id.HasValue)
            {
                var key = StringUtil.ConcatNonEmpties(".", domain, lookupName);
                if (_mappingAsync.ContainsKey(key)) return await _mappingAsync.Get(key)?.Invoke(id.Value);
                else if (_mappingAsync.ContainsKey(lookupName)) return await _mappingAsync.Get(lookupName)?.Invoke(id.Value);
            }
            return null;
        }

        public async Task<string> GetNameAsync(Expression<Func<int?>> expression, string domain = null) 
            => await GetNameAsync(((MemberExpression)expression.Body).Member.Name, expression.Compile()(), domain);

        public string GetName(string lookupName, int? id) 
            => id.HasValue ? _mapping.Get(lookupName)?.Invoke(id.Value) : null;

        public bool IsLookupField(string name) => _mapping.ContainsKey(name);

        private async Task<T> AutoAsync<T>(Func<Task<T>> asyncFactory, [CallerMemberName] string callerName = null)
        {
            return await _cacheAccessor.AutoAsync(asyncFactory, string.Empty, GetType().Name, callerFuncName: callerName);
        }

        private async Task<T> AutoAsync<T>(Func<T> factory, [CallerMemberName] string callerName = null)
        {
            return await _cacheAccessor.AutoAsync(factory, string.Empty, GetType().Name, callerFuncName: callerName);
        }

        private T Auto<T>(Func<T> factory, [CallerMemberName] string callerName = null)
        {
            return _cacheAccessor.Auto(factory, string.Empty, GetType().Name, callerFuncName: callerName);
        }

        public void Dispose() => _lookupService.Dispose();
    }
}
