using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Services.Domain;
using Edubase.Services.Enums;

namespace Edubase.Services.Lookup
{
    public class CachedLookupService : ICachedLookupService
    {
        private readonly Dictionary<string, Func<int, Task<string>>> _mappingAsync;
        private readonly ILookupService _lookupService;
        private readonly ICacheAccessor _cacheAccessor;

        public CachedLookupService(ILookupService lookupService, ICacheAccessor cacheAccessor)
        {
            _lookupService = lookupService;
            _cacheAccessor = cacheAccessor;
            _mappingAsync = new Dictionary<string, Func<int, Task<string>>>
            {
                { "LocalAuthorityId", async id => (await LocalAuthorityGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "RSCRegionId", async id => (await RscRegionsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
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
                { "IEBTModel.BoardingEstablishmentId", async id => (await BoardingEstablishmentGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "IEBTModel.AccommodationChangedId", async id => (await AccommodationChangedGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "QualityAssuranceBodyNameId", async id => (await QualityAssuranceBodyNameGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "EstablishmentAccreditedId", async id => (await EstablishmentAccreditedGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionOfficialSixthFormId", async id => (await ProvisionOfficialSixthFormsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "GenderId", async id => (await GendersGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReligiousCharacterId", async id => (await ReligiousCharactersGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ReligiousEthosId", async id => (await ReligiousEthosGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "DioceseId", async id => (await DiocesesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "AdmissionsPolicyId", async id => (await AdmissionsPoliciesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "ProvisionSpecialClassesId", async id => (await ProvisionSpecialClassesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "HeadTitleId", async id => (await TitlesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "EstablishmentTypeGroupId", async id => (await EstablishmentTypeGroupsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "InspectorateId", async id => (await InspectoratesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "Section41ApprovedId", async id => (await Section41ApprovedGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "SEN1Id", async id => (await SpecialEducationNeedsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
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
                { "CountryId", async id => (await NationalitiesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "CountyId", async id => (await CountiesGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "OfstedRatingId", async id => (await OfstedRatingsGetAllAsync()).FirstOrDefault(x=>x.Id == id)?.Name },
                { "IEBTModel.ProprietorTypeId", id =>
                {
                    // a little bit overkill, but making use of the existing pattern to display enum choice
                    var pt = (eProprietorType)id;
                    var type = pt.GetType();
                    var member = type.GetMember(pt.ToString());
                    var displayName = pt.ToString();

                    var attributes = member.Select(e => e.GetCustomAttributes(typeof(DisplayAttribute), false)).FirstOrDefault();
                    if (attributes != null && attributes.Length > 0)
                    {
                        displayName= ((DisplayAttribute) attributes[0]).Name;
                    }

                    return Task.FromResult(displayName);
                }
            }
        };
    }

        public async Task<IEnumerable<LookupDto>> LocalAuthorityGetAllAsync() => await AutoAsync(_lookupService.LocalAuthorityGetAllAsync);
        public async Task<IEnumerable<LookupDto>> AdmissionsPoliciesGetAllAsync() => await AutoAsync(_lookupService.AdmissionsPoliciesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> EducationPhasesGetAllAsync() => await AutoAsync(_lookupService.EducationPhasesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> EstablishmentStatusesGetAllAsync() => await AutoAsync(_lookupService.EstablishmentStatusesGetAllAsync);
        public async Task<IEnumerable<EstablishmentLookupDto>> EstablishmentTypesGetAllAsync() => await AutoAsync(_lookupService.EstablishmentTypesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> GendersGetAllAsync() => await AutoAsync(_lookupService.GendersGetAllAsync);
        public async Task<IEnumerable<LookupDto>> GroupTypesGetAllAsync() => await AutoAsync(_lookupService.GroupTypesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> TitlesGetAllAsync() => await AutoAsync(_lookupService.TitlesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> CountiesGetAllAsync() => await AutoAsync(_lookupService.CountiesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> OfstedRatingsGetAllAsync() => await AutoAsync(_lookupService.OfstedRatingsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> RscRegionsGetAllAsync() => await AutoAsync(_lookupService.RscRegionsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> ProvisionBoardingGetAllAsync() => await AutoAsync(_lookupService.ProvisionBoardingGetAllAsync);
        public async Task<IEnumerable<LookupDto>> ProvisionNurseriesGetAllAsync() => await AutoAsync(_lookupService.ProvisionNurseriesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> ProvisionOfficialSixthFormsGetAllAsync() => await AutoAsync(_lookupService.ProvisionOfficialSixthFormsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> ProvisionSpecialClassesGetAllAsync() => await AutoAsync(_lookupService.ProvisionSpecialClassesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> ReasonEstablishmentClosedGetAllAsync() => await AutoAsync(_lookupService.ReasonEstablishmentClosedGetAllAsync);
        public async Task<IEnumerable<LookupDto>> ReasonEstablishmentOpenedGetAllAsync() => await AutoAsync(_lookupService.ReasonEstablishmentOpenedGetAllAsync);
        public async Task<IEnumerable<LookupDto>> ReligiousCharactersGetAllAsync() => await AutoAsync(_lookupService.ReligiousCharactersGetAllAsync);
        public async Task<IEnumerable<LookupDto>> ReligiousEthosGetAllAsync() => await AutoAsync(_lookupService.ReligiousEthosGetAllAsync);
        public async Task<IEnumerable<LookupDto>> GovernorRolesGetAllAsync() => await AutoAsync(_lookupService.GovernorRolesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> GovernorAppointingBodiesGetAllAsync() => await AutoAsync(_lookupService.GovernorAppointingBodiesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> AccommodationChangedGetAllAsync() => await AutoAsync(_lookupService.AccommodationChangedGetAllAsync);
        public async Task<IEnumerable<LookupDto>> BoardingEstablishmentGetAllAsync() => await AutoAsync(_lookupService.BoardingEstablishmentGetAllAsync);
        public async Task<IEnumerable<LookupDto>> QualityAssuranceBodyNameGetAllAsync() => await AutoAsync(_lookupService.QualityAssuranceBodyNameGetAllAsync);
        public async Task<IEnumerable<LookupDto>> EstablishmentAccreditedGetAllAsync() => await AutoAsync(_lookupService.EstablishmentAccreditedGetAllAsync);
        public async Task<IEnumerable<LookupDto>> CCGovernanceGetAllAsync() => await AutoAsync(_lookupService.CCGovernanceGetAllAsync);
        public async Task<IEnumerable<LookupDto>> CCOperationalHoursGetAllAsync() => await AutoAsync(_lookupService.CCOperationalHoursGetAllAsync);
        public async Task<IEnumerable<LookupDto>> CCDisadvantagedAreasGetAllAsync() => await AutoAsync(_lookupService.CCDisadvantagedAreasGetAllAsync);
        public async Task<IEnumerable<LookupDto>> CCPhaseTypesGetAllAsync() => await AutoAsync(_lookupService.CCPhaseTypesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> CCGroupLeadsGetAllAsync() => await AutoAsync(_lookupService.CCGroupLeadsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> CCDeliveryModelsGetAllAsync() => await AutoAsync(_lookupService.CCDeliveryModelsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> DiocesesGetAllAsync() => await AutoAsync(_lookupService.DiocesesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> ChildcareFacilitiesGetAllAsync() => await AutoAsync(_lookupService.ChildcareFacilitiesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> DirectProvisionOfEarlyYearsGetAllAsync() => await AutoAsync(_lookupService.DirectProvisionOfEarlyYearsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> FurtherEducationTypesGetAllAsync() => await AutoAsync(_lookupService.FurtherEducationTypesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> IndependentSchoolTypesGetAllAsync() => await AutoAsync(_lookupService.IndependentSchoolTypesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> InspectoratesGetAllAsync() => await AutoAsync(_lookupService.InspectoratesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> InspectorateNamesGetAllAsync() => await AutoAsync(_lookupService.InspectorateNamesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> LocalGovernorsGetAllAsync() => await AutoAsync(_lookupService.LocalGovernorsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> NationalitiesGetAllAsync() => await AutoAsync(_lookupService.NationalitiesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> PRUEBDsGetAllAsync() => await AutoAsync(_lookupService.PRUEBDsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> PruEducatedByOthersGetAllAsync() => await AutoAsync(_lookupService.PruEducatedByOthersGetAllAsync);
        public async Task<IEnumerable<LookupDto>> PruFulltimeProvisionsGetAllAsync() => await AutoAsync(_lookupService.PruFulltimeProvisionsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> PRUSENsGetAllAsync() => await AutoAsync(_lookupService.PRUSENsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> ResourcedProvisionsGetAllAsync() => await AutoAsync(_lookupService.ResourcedProvisionsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> Section41ApprovedGetAllAsync() => await AutoAsync(_lookupService.Section41ApprovedGetAllAsync);
        public async Task<IEnumerable<LookupDto>> SpecialEducationNeedsGetAllAsync() => await AutoAsync(_lookupService.SpecialEducationNeedsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> TeenageMothersProvisionsGetAllAsync() => await AutoAsync(_lookupService.TeenageMothersProvisionsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> TypeOfResourcedProvisionsGetAllAsync() => await AutoAsync(_lookupService.TypeOfResourcedProvisionsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> EstablishmentLinkTypesGetAllAsync() => await AutoAsync(_lookupService.EstablishmentLinkTypesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> EstablishmentTypeGroupsGetAllAsync() => await AutoAsync(_lookupService.EstablishmentTypeGroupsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> GovernmentOfficeRegionsGetAllAsync() => await AutoAsync(_lookupService.GovernmentOfficeRegionsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> AdministrativeDistrictsGetAllAsync() => await AutoAsync(_lookupService.AdministrativeDistrictsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> AdministrativeWardsGetAllAsync() => await AutoAsync(_lookupService.AdministrativeWardsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> ParliamentaryConstituenciesGetAllAsync() => await AutoAsync(_lookupService.ParliamentaryConstituenciesGetAllAsync);
        public async Task<IEnumerable<LookupDto>> UrbanRuralGetAllAsync() => await AutoAsync(_lookupService.UrbanRuralGetAllAsync);
        public async Task<IEnumerable<LookupDto>> GSSLAGetAllAsync() => await AutoAsync(_lookupService.GSSLAGetAllAsync);
        public async Task<IEnumerable<LookupDto>> MSOAsGetAllAsync() => await AutoAsync(_lookupService.MSOAsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> LSOAsGetAllAsync() => await AutoAsync(_lookupService.LSOAsGetAllAsync);
        public async Task<IEnumerable<LookupDto>> GroupStatusesGetAllAsync() => await AutoAsync(_lookupService.GroupStatusesGetAllAsync);
        public async Task<string> GetNameAsync(string lookupName, int? id, string domain = null)
        {
            lookupName = ProcessLookupName(lookupName);
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
        public bool IsLookupField(string name) => _mappingAsync.ContainsKey(ProcessLookupName(name));
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
        private string ProcessLookupName(string name)
        {
            Guard.IsNotNull(name, () => new ArgumentNullException(nameof(name)));
            if (name.EndsWith("CountryId")) name = "CountryId";
            else if (name.EndsWith("CountyId")) name = "CountyId";
            return name;
        }
    }
}
