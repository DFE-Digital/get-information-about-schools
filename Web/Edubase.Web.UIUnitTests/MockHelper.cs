using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Principal;
using Edubase.Services.Domain;
using Edubase.Services.Lookup;
using Moq;

namespace Edubase.Web.UIUnitTests
{
    internal static class MockHelper
    {
        public static Mock<ICachedLookupService> SetupCachedLookupService()
        {
            var cls = new Mock<ICachedLookupService>(MockBehavior.Strict);
            cls.Setup(c => c.AccommodationChangedGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.FurtherEducationTypesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.GendersGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.LocalAuthorityGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.EstablishmentTypesGetAllAsync()).ReturnsAsync(() => new List<EstablishmentLookupDto> { new EstablishmentLookupDto() });
            cls.Setup(c => c.TitlesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.EstablishmentStatusesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.AdmissionsPoliciesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.InspectoratesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.IndependentSchoolTypesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.InspectorateNamesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.ReligiousCharactersGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.ReligiousEthosGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.DiocesesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.ProvisionBoardingGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.ProvisionNurseriesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.ProvisionOfficialSixthFormsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.Section41ApprovedGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.EducationPhasesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.ReasonEstablishmentOpenedGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.ReasonEstablishmentClosedGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.ProvisionSpecialClassesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.SpecialEducationNeedsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.TypeOfResourcedProvisionsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.TeenageMothersProvisionsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.ChildcareFacilitiesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.RscRegionsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.GovernmentOfficeRegionsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.AdministrativeDistrictsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.AdministrativeWardsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.ParliamentaryConstituenciesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.UrbanRuralGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.GSSLAGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.PruFulltimeProvisionsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.PruEducatedByOthersGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.PRUEBDsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.PRUSENsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.CountiesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.NationalitiesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.OfstedRatingsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.MSOAsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.LSOAsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.GetNameAsync(It.IsAny<Expression<Func<int?>>>(), null)).ReturnsAsync("");
            cls.Setup(c => c.GovernorAppointingBodiesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.BoardingEstablishmentGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.EstablishmentAccreditedGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.QualityAssuranceBodyNameGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.CCOperationalHoursGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.CCGovernanceGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.CCDeliveryModelsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.CCGroupLeadsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.CCPhaseTypesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.CCDisadvantagedAreasGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            cls.Setup(c => c.DirectProvisionOfEarlyYearsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });

            return cls;
        }

        public static Mock<IUserDependentLookupService> SetupLookupService(IPrincipal user)
        {
            var ls = new Mock<IUserDependentLookupService>(MockBehavior.Strict);
            ls.Setup(c => c.EstablishmentStatusesGetAllAsync(user)).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });

            return ls;
        }
    }
}
