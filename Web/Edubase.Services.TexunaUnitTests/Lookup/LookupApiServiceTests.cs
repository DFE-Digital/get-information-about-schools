using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Security;
using Moq;
using Xunit;

namespace Edubase.Services.Texuna.Lookup.Tests
{
    public class LookupApiServiceTests
    {
        private readonly Mock<IHttpClientWrapper> _mockHttpClientWrapper;
        private readonly Mock<ISecurityService> _mockSecurityService;
        private readonly LookupApiService _lookupApiService;

        private readonly List<LookupDto> _lookupDtoList;

        public LookupApiServiceTests()
        {
            _mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
            _mockSecurityService = new Mock<ISecurityService>();
            _lookupApiService = new LookupApiService(_mockHttpClientWrapper.Object, _mockSecurityService.Object);

            _lookupDtoList = new List<LookupDto>()
            {
                new LookupDto() {Id = 1, Name = "TestLookup1", Code = "TL1", DisplayOrder = 1},
                new LookupDto() {Id = 2, Name = "TestLookup2", Code = "TL2", DisplayOrder = 2},
                new LookupDto() {Id = 3, Name = "TestLookup3", Code = "TL3", DisplayOrder = 3},
                new LookupDto() {Id = 4, Name = "TestLookup4", Code = "TL4", DisplayOrder = 4},
                new LookupDto() {Id = 5, Name = "TestLookup5", Code = "TL5", DisplayOrder = 5},
            };

            var apiReponse = new ApiResponse<List<LookupDto>>(true) { Response = _lookupDtoList };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<List<LookupDto>>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(apiReponse);
        }

        [Fact()]
        public void LookupApiService_ConstructorTest()
        {
            Assert.NotNull(_lookupApiService);
        }

        [Theory]
        [MemberData(nameof(GetAsyncLookupTypes))]
        public async Task AsyncLookupsTest(string lookupType)
        {
            _mockSecurityService.Setup(service => service.CreateAnonymousPrincipal()).Returns(Mock.Of<IPrincipal>);

            //use reflection to construct method name
            var method = _lookupApiService.GetType().GetMethod($"{lookupType}GetAllAsync",
                Type.EmptyTypes);
            var taskResult = method.Invoke(_lookupApiService, null) as Task<IEnumerable<LookupDto>>;
            var result = await taskResult;

            _mockHttpClientWrapper.Verify(x => x.GetAsync<List<LookupDto>>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once);
            Assert.Equal<IEnumerable<LookupDto>>(_lookupDtoList, result);
        }

        [Fact()]
        public async Task EstablishmentTypesGetAllAsyncTest()
        {
            var result = await _lookupApiService.EstablishmentTypesGetAllAsync();

            _mockHttpClientWrapper.Verify(x => x.GetAsync<List<LookupDto>>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once);
            Assert.Equal(_lookupDtoList.Count, result.Count());
        }

        public static IEnumerable<object[]> GetAsyncLookupTypes()
        {
            var allData = new List<object[]>
            {
                new object[] { "LocalAuthority" },
                new object[] { "GovernorRoles" },
                new object[] { "GovernorAppointingBodies" },
                new object[] { "GroupTypes" },
                new object[] { "EstablishmentTypeGroups" },
                new object[] { "EstablishmentStatuses" },
                new object[] { "GroupStatuses" },
                new object[] { "AccommodationChanged" },
                new object[] { "AdministrativeDistricts" },
                new object[] { "AdministrativeWards" },
                new object[] { "AdmissionsPolicies" },
                new object[] { "BoardingEstablishment" },
                new object[] { "CASWards" },
                new object[] { "CCDeliveryModels" },
                new object[] { "CCDisadvantagedAreas" },
                new object[] { "CCGovernance" },
                new object[] { "CCGroupLeads" },
                new object[] { "CCOperationalHours" },
                new object[] { "CCPhaseTypes" },
                new object[] { "ChildcareFacilities" },
                new object[] { "Dioceses" },
                new object[] { "DirectProvisionOfEarlyYears" },
                new object[] { "EstablishmentAccredited" },
                new object[] { "EducationPhases" },
                new object[] { "EstablishmentLinkTypes" },
                new object[] { "FurtherEducationTypes" },
                new object[] { "Genders" },
                new object[] { "GovernmentOfficeRegions" },
                new object[] { "GSSLA" },
                new object[] { "Titles" },
                new object[] { "IndependentSchoolTypes" },
                new object[] { "InspectorateNames" },
                new object[] { "InspectorateNames" },
                new object[] { "Inspectorates" },
                new object[] { "LocalGovernors" },
                new object[] { "LSOAs" },
                new object[] { "MSOAs" },
                new object[] { "Nationalities" },
                new object[] { "ParliamentaryConstituencies" },
                new object[] { "ProvisionBoarding" },
                new object[] { "ProvisionNurseries" },
                new object[] { "ProvisionOfficialSixthForms" },
                new object[] { "ProvisionSpecialClasses" },
                new object[] { "PRUEBDs" },
                new object[] { "PruEducatedByOthers" },
                new object[] { "PruFulltimeProvisions" },
                new object[] { "QualityAssuranceBodyName" },
                new object[] { "PRUSENs" },
                new object[] { "ReasonEstablishmentClosed" },
                new object[] { "ReasonEstablishmentOpened" },
                new object[] { "ReligiousCharacters" },
                new object[] { "ReligiousEthos" },
                new object[] { "ResourcedProvisions" },
                new object[] { "Section41Approved" },
                new object[] { "SpecialEducationNeeds" },
                new object[] { "TeenageMothersProvisions" },
                new object[] { "TypeOfResourcedProvisions" },
                new object[] { "UrbanRural" },
                new object[] { "Counties" },
                new object[] { "OfstedRatings" },
                new object[] { "RscRegions" },
            };
            return allData;
        }

        public static IEnumerable<object[]> GetSyncronousLookupTypes()
        {
            var allData = new List<object[]>
            {
                new object[] { "LocalAuthority" },
                new object[] { "AccommodationChanged" },
                new object[] { "AdministrativeDistricts" },
                new object[] { "AdministrativeWards" },
                new object[] { "AdmissionsPolicies" },
                new object[] { "BoardingEstablishment" },
                new object[] { "CASWards" },
                new object[] { "CCDeliveryModels" },
                new object[] { "CCDisadvantagedAreas" },
                new object[] { "CCGovernance" },
                new object[] { "CCGroupLeads" },
                new object[] { "CCOperationalHours" },
                new object[] { "CCPhaseTypes" },
                new object[] { "ChildcareFacilities" },
                new object[] { "Dioceses" },
                new object[] { "DirectProvisionOfEarlyYears" },
                new object[] { "EducationPhases" },
                new object[] { "EstablishmentAccredited" },
                new object[] { "EstablishmentLinkTypes" },
                new object[] { "EstablishmentStatuses" },
                new object[] { "EstablishmentTypeGroups" },
                new object[] { "EstablishmentTypes" },
                new object[] { "FurtherEducationTypes" },
                new object[] { "Genders" },
                new object[] { "GovernmentOfficeRegions" },
                new object[] { "GovernorAppointingBodies" },
                new object[] { "GovernorRoles" },
                new object[] { "GroupStatuses" },
                new object[] { "GroupTypes" },
                new object[] { "GSSLA" },
                new object[] { "HeadTitles" },
                new object[] { "IndependentSchoolTypes" },
                new object[] { "InspectorateNames" },
                new object[] { "Inspectorates" },
                new object[] { "LocalGovernors" },
                new object[] { "LSOAs" },
                new object[] { "MSOAs" },
                new object[] { "Nationalities" },
                new object[] { "ParliamentaryConstituencies" },
                new object[] { "ProvisionBoarding" },
                new object[] { "ProvisionNurseries" },
                new object[] { "ProvisionOfficialSixthForms" },
                new object[] { "ProvisionSpecialClasses" },
                new object[] { "PRUEBDs" },
                new object[] { "PruEducatedByOthers" },
                new object[] { "PruFulltimeProvisions" },
                new object[] { "PRUSENs" },
                new object[] { "QualityAssuranceBodyName" },
                new object[] { "ReasonEstablishmentClosed" },
                new object[] { "ReasonEstablishmentOpened" },
                new object[] { "ReligiousCharacters" },
                new object[] { "ReligiousEthos" },
                new object[] { "ResourcedProvisions" },
                new object[] { "Section41Approved" },
                new object[] { "SpecialEducationNeeds" },
                new object[] { "TeenageMothersProvisions" },
                new object[] { "TypeOfResourcedProvisions" },
                new object[] { "UrbanRural" },
            };
            return allData;
        }
    }
}
