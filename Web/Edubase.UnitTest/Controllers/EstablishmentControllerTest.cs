using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Exceptions;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Web.Resources;
using Edubase.Web.UI.Areas.Establishments.Controllers;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Models;
using Moq;
using NUnit.Framework;

namespace Edubase.UnitTest.Controllers
{
    [TestFixture]
    public class EstablishmentControllerTest : UnitTestBase<EstablishmentController>
    {
        public EstablishmentControllerTest()
        {
            
        }

        [Test]
        public async Task Estab_EditDetails_Null_Id()
        {
            var response = await ObjectUnderTest.EditDetails(null, null);

            Assert.IsTrue(response is HttpNotFoundResult);
        }

        [Test]
        public async Task Estab_EditDetails_Id_NotFound()
        {
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            Assert.That(async () => await ObjectUnderTest.EditDetails(4, null), Throws.TypeOf<EntityNotFoundException>(), "Expected exception of type EntityNotFoundException");
        }

        [Test]
        public async Task Estab_EditDetails()
        {
            var establishment = new EstablishmentModel
            {
                IEBTModel = new IEBTModel()
            };

            var editEstabModel = new EditEstablishmentModel
            {
                Address_CityOrTown = "cityOrTown",
                Address_CountryId = 1,
                Address_CountyId = 2,
                Address_Line1 = "line1",
                Address_Locality = "locality",
                Address_Line3 = "line3",
                Address_PostCode = "postcode",
                Address_UPRN = "uprn",
                Northing = 3,
                Easting = 4
            };

            var replacementAddress = new ReplaceAddressViewModel
            {
                Target = "main",
                Town = "replacementTown",
                CountryId = 5,
                CountyId = 8,
                Street = "replacementStreet",
                Locality = "replacementLocality",
                Address3 = "replacementAddress3",
                PostCode = "replacementPostcode",
                SelectedUPRN = "1234",
                Northing = 7,
                Easting = 5
            };

            var address = UriHelper.SerializeToUrlToken(replacementAddress);

            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(establishment));
            GetMock<IMapper>().Setup(m => m.Map<EditEstablishmentModel>(establishment)).Returns(editEstabModel);
            GetMock<IMapper>().Setup(m => m.Map(It.IsAny<IEBTModel>(), editEstabModel)).Returns(editEstabModel);
            GetMock<IEstablishmentReadService>().Setup(e => e.GetEditPolicyAsync(establishment, It.IsAny<IPrincipal>())).ReturnsAsync(() => new EstablishmentDisplayEditPolicy {IEBTDetail = new IEBTDetailDisplayEditPolicy()});
            GetMock<IPrincipal>().Setup(p => p.IsInRole(It.IsAny<string>())).Returns(true);

            SetupMocksForSelectLists();

            var response = await ObjectUnderTest.EditDetails(4, address);

            Assert.That(response is ViewResult);
            var viewResult = (ViewResult) response;
            Assert.That(viewResult.Model is EditEstablishmentModel);
            var model = (EditEstablishmentModel) viewResult.Model;
            Assert.That(model.Address_CityOrTown == replacementAddress.Town);
            Assert.That(model.Address_CountryId == replacementAddress.CountryId);
            Assert.That(model.Address_CountyId == replacementAddress.CountyId);
            Assert.That(model.Address_Line1 == replacementAddress.Street);
            Assert.That(model.Address_Locality == replacementAddress.Locality);
            Assert.That(model.Address_Line3 == replacementAddress.Address3);
            Assert.That(model.Address_PostCode == replacementAddress.PostCode);
            Assert.That(model.Address_UPRN == replacementAddress.SelectedUPRN);
            Assert.That(model.Northing == replacementAddress.Northing);
            Assert.That(model.Easting == replacementAddress.Easting);
        }

        [Test]
        public async Task Estab_Edit_Helpdesk_Null_Id()
        {
            var response = await ObjectUnderTest.EditHelpdesk(null as int?);

            Assert.IsTrue(response is HttpNotFoundResult);
        }

        [Test]
        public async Task Estab_Edit_Helpdesk_Id_NotFound()
        {
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            Assert.That(async () => await ObjectUnderTest.EditHelpdesk(4), Throws.TypeOf<EntityNotFoundException>(), "Expected exception of type EntityNotFoundException");
        }

        [Test]
        public async Task Estab_Edit_Location_Null_Id()
        {
            var response = await ObjectUnderTest.EditLocation(null as int?);

            Assert.IsTrue(response is HttpNotFoundResult);
        }

        [Test]
        public async Task Estab_Edit_Location_Id_NotFound()
        {
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            Assert.That(async () => await ObjectUnderTest.EditLocation(4), Throws.TypeOf<EntityNotFoundException>(), "Expected exception of type EntityNotFoundException");
        }

        [Test]
        public async Task Estab_Edit_IEBT_Null_Id()
        {
            var response = await ObjectUnderTest.EditIEBT(null as int?);

            Assert.IsTrue(response is HttpNotFoundResult);
        }

        [Test]
        public async Task Estab_Edit_IEBT_Id_NotFound()
        {
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            Assert.That(async () => await ObjectUnderTest.EditIEBT(4), Throws.TypeOf<EntityNotFoundException>(), "Expected exception of type EntityNotFoundException");
        }

        [Test]
        public async Task Estab_SearchForEstablishment_Null_Id()
        {
            var response = await ObjectUnderTest.SearchForEstablishment(null, null);

            Assert.IsTrue(response is HttpNotFoundResult);
        }

        [Test]
        public async Task Estab_SearchForEstablishment_Id_NotFound()
        {
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            Assert.That(async () => await ObjectUnderTest.SearchForEstablishment(4, null), Throws.TypeOf<EntityNotFoundException>(), "Expected exception of type EntityNotFoundException");
        }

        [Test]
        public async Task Estab_ReplaceEstablishmentAddress()
        {
            var nationalities = new List<LookupDto>
            {
                new LookupDto {Code = "1", Id = 1, Name = "Nationality 1"},
                new LookupDto {Code = "2", Id = 2, Name = "Nationality 2"},
                new LookupDto {Code = "3", Id = 3, Name = "Nationality 3"}
            };

            var counties = new List<LookupDto>
            {
                new LookupDto {Code = "1", Id = 1, Name = "County 1"},
                new LookupDto {Code = "2", Id = 2, Name = "County 2"},
                new LookupDto {Code = "3", Id = 3, Name = "County 3"}
            };

            var establishment = new EstablishmentModel
            {
                Urn = 5,
                Name = "test"
            };

            GetMock<ICachedLookupService>().Setup(c => c.NationalitiesGetAllAsync()).ReturnsAsync(() => nationalities);
            GetMock<ICachedLookupService>().Setup(c => c.CountiesGetAllAsync()).ReturnsAsync(() => counties);
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(establishment));
            GetMock<IEstablishmentReadService>().Setup(e => e.GetEditPolicyAsync(It.IsAny<EstablishmentModel>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new EstablishmentDisplayEditPolicy { IEBTDetail = new IEBTDetailDisplayEditPolicy { AccommodationChangedId = true } });

            await ObjectUnderTest.ReplaceEstablishmentAddressAsync(5, "test");
        }

        [Test]
        public async Task Estab_EditLinks_Null_Id()
        {
            var response = await ObjectUnderTest.EditLinks(null);

            Assert.IsTrue(response is HttpNotFoundResult);
        }

        [Test]
        public async Task Estab_EditLinks_Id_NotFound()
        {
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            Assert.That(async () => await ObjectUnderTest.EditLinks(4), Throws.TypeOf<EntityNotFoundException>(), "Expected exception of type EntityNotFoundException");
        }

        private void SetupMocksForSelectLists()
        {
            GetMock<ICachedLookupService>().Setup(c => c.AccommodationChangedGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto()});
            GetMock<ICachedLookupService>().Setup(c => c.FurtherEducationTypesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.GendersGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.LocalAuthorityGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.EstablishmentTypesGetAllAsync()).ReturnsAsync(() => new List<EstablishmentLookupDto> { new EstablishmentLookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.TitlesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.EstablishmentStatusesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.AdmissionsPoliciesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.InspectoratesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.IndependentSchoolTypesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.InspectorateNamesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.ReligiousCharactersGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.ReligiousEthosGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.DiocesesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.ProvisionBoardingGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.ProvisionNurseriesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.ProvisionOfficialSixthFormsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.Section41ApprovedGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.EducationPhasesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.ReasonEstablishmentOpenedGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.ReasonEstablishmentClosedGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.ProvisionSpecialClassesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.SpecialEducationNeedsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.TypeOfResourcedProvisionsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.TeenageMothersProvisionsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.ChildcareFacilitiesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.RscRegionsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.GovernmentOfficeRegionsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.AdministrativeDistrictsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.AdministrativeWardsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.ParliamentaryConstituenciesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.UrbanRuralGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.GSSLAGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.CASWardsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.PruFulltimeProvisionsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.PruEducatedByOthersGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.PRUEBDsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.PRUSENsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.CountiesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.NationalitiesGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.OfstedRatingsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.MSOAsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<ICachedLookupService>().Setup(c => c.LSOAsGetAllAsync()).ReturnsAsync(() => new List<LookupDto> { new LookupDto() });
            GetMock<IEstablishmentReadService>().Setup(e => e.GetEstabType2EducationPhaseMap()).Returns(new Dictionary<eLookupEstablishmentType, eLookupEducationPhase[]>());
            GetMock<ICachedLookupService>().Setup(c => c.GetNameAsync(It.IsAny<Expression<Func<int?>>>(), null)).ReturnsAsync("");
        }


        [SetUp]
        public void SetUpTest() => SetupObjectUnderTest();

        [TearDown]
        public void TearDownTest() => ResetMocks();

        [OneTimeSetUp]
        protected override void InitialiseMocks()
        {
            AddMock<IEstablishmentReadService>();
            AddMock<IGroupReadService>();
            AddMock<IMapper>();
            AddMock<IEstablishmentWriteService>();
            AddMock<ICachedLookupService>();
            AddMock<IResourcesHelper>();
            AddMock<ISecurityService>();
            base.InitialiseMocks();
        }
    }
}
