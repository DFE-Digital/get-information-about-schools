using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.EditPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Exceptions;
using Edubase.Services.ExternalLookup;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
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
        public async Task Estab_Edit_Helpdesk_Id_NotFound()
        {
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            Assert.That(async () => await ObjectUnderTest.EditHelpdesk(4), Throws.TypeOf<EntityNotFoundException>(), "Expected exception of type EntityNotFoundException");
        }

        [Test]
        public async Task Estab_Edit_Helpdesk_Null_Id()
        {
            var response = await ObjectUnderTest.EditHelpdesk(null as int?);

            Assert.IsTrue(response is HttpNotFoundResult);
        }

        [Test]
        public async Task Estab_Edit_IEBT_Id_NotFound()
        {
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            Assert.That(async () => await ObjectUnderTest.EditIEBT(4), Throws.TypeOf<EntityNotFoundException>(), "Expected exception of type EntityNotFoundException");
        }

        [Test]
        public async Task Estab_Edit_IEBT_Null_Id()
        {
            var response = await ObjectUnderTest.EditIEBT(null as int?);

            Assert.IsTrue(response is HttpNotFoundResult);
        }

        [Test]
        public async Task Estab_Edit_Location_Id_NotFound()
        {
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            Assert.That(async () => await ObjectUnderTest.EditLocation(4), Throws.TypeOf<EntityNotFoundException>(), "Expected exception of type EntityNotFoundException");
        }

        [Test]
        public async Task Estab_Edit_Location_Null_Id()
        {
            var response = await ObjectUnderTest.EditLocation(null as int?);

            Assert.IsTrue(response is HttpNotFoundResult);
        }

        [Test]
        public async Task Estab_EditDetails()
        {
            var establishment = new EstablishmentModel
            {
                Urn = 100000,
                IEBTModel = new IEBTModel()
            };

            var editEstabModel = new EditEstablishmentModel
            {
                Urn = 100000,
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

            var replacementAddress = new AddOrReplaceAddressViewModel
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

            GetMock<IGroupReadService>().Setup(x => x.GetAllByEstablishmentUrnAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new[] { new GroupModel { Name = "Group 1", GroupUId = 1000 } });
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(establishment));
            GetMock<IMapper>().Setup(m => m.Map<EditEstablishmentModel>(establishment)).Returns(editEstabModel);
            GetMock<IMapper>().Setup(m => m.Map(It.IsAny<IEBTModel>(), editEstabModel)).Returns(editEstabModel);
            GetMock<IEstablishmentReadService>().Setup(e => e.GetEditPolicyAsync(establishment, It.IsAny<IPrincipal>())).ReturnsAsync(() => new EstablishmentEditPolicyEnvelope { EditPolicy = new EstablishmentDisplayEditPolicy { IEBTDetail = new IEBTDetailDisplayEditPolicy() } });
            GetMock<IPrincipal>().Setup(p => p.IsInRole(It.IsAny<string>())).Returns(true);
            GetMock<ICSCPService>().Setup(x => x.CheckExists(It.IsAny<int>(), It.IsAny<string>())).Returns(true);
            GetMock<ICSCPService>().Setup(x => x.SchoolURL(It.IsAny<int>(), It.IsAny<string>())).Returns("https://cscp.azurewebsites.net/school/123456");
            GetMock<IFBService>().Setup(x => x.CheckExists(It.IsAny<int>())).Returns(true);
            GetMock<IFBService>().Setup(x => x.SchoolURL(It.IsAny<int>())).Returns("https://sfb.azurewebsites.net/school/detail?urn=123456");

            SetupCachedLookupService();

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
        public async Task Estab_EditDetails_Id_NotFound()
        {
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            Assert.That(async () => await ObjectUnderTest.EditDetails(4, null), Throws.TypeOf<EntityNotFoundException>(), "Expected exception of type EntityNotFoundException");
        }

        [Test]
        public async Task Estab_EditDetails_Null_Id()
        {
            var response = await ObjectUnderTest.EditDetails(null, null);

            Assert.IsTrue(response is HttpNotFoundResult);
        }

        [Test]
        public async Task Estab_EditLinks_Id_NotFound()
        {
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            Assert.That(async () => await ObjectUnderTest.EditLinks(4), Throws.TypeOf<EntityNotFoundException>(), "Expected exception of type EntityNotFoundException");
        }

        [Test]
        public async Task Estab_EditLinks_Null_Id()
        {
            var response = await ObjectUnderTest.EditLinks(null);

            Assert.IsTrue(response is HttpNotFoundResult);
        }

        [Test]
        public async Task Estab_AddOrReplaceEstablishmentAddress()
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

            GetMock<IGroupReadService>().Setup(x => x.GetAllByEstablishmentUrnAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new[] { new GroupModel { Name = "Group 1", GroupUId = 1000 } });
            GetMock<ICachedLookupService>().Setup(c => c.NationalitiesGetAllAsync()).ReturnsAsync(() => nationalities);
            GetMock<ICachedLookupService>().Setup(c => c.CountiesGetAllAsync()).ReturnsAsync(() => counties);
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(establishment));
            GetMock<IEstablishmentReadService>().Setup(e => e.GetEditPolicyAsync(It.IsAny<EstablishmentModel>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new EstablishmentEditPolicyEnvelope { EditPolicy = new EstablishmentDisplayEditPolicy { IEBTDetail = new IEBTDetailDisplayEditPolicy { AccommodationChangedId = true } } });

            await ObjectUnderTest.AddOrReplaceEstablishmentAddressAsync(5, "test");
        }

        [Test]
        public async Task Estab_SearchForEstablishment_Id_NotFound()
        {
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            Assert.That(async () => await ObjectUnderTest.SearchForEstablishment(4, null), Throws.TypeOf<EntityNotFoundException>(), "Expected exception of type EntityNotFoundException");
        }

        [Test]
        public async Task Estab_SearchForEstablishment_Null_Id()
        {
            var response = await ObjectUnderTest.SearchForEstablishment(null, null);

            Assert.IsTrue(response is HttpNotFoundResult);
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
