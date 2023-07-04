using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
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
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Models;
using Edubase.Web.UIUnitTests;
using Moq;
using Xunit;

namespace Edubase.Web.UI.Areas.Establishments.Controllers.UnitTests
{
    public class EstablishmentControllerTests: IDisposable
    {
        private readonly EstablishmentController controller;
        private readonly Mock<ICachedLookupService> mockCachedLookupService;
        private readonly Mock<IUserDependentLookupService> mockLookupService;
        private readonly Mock<IEstablishmentReadService> mockEstablishmentReadService = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
        private readonly Mock<IGroupReadService> mockGroupReadService = new Mock<IGroupReadService>(MockBehavior.Strict);
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>(MockBehavior.Strict);
        private readonly Mock<IEstablishmentWriteService> mockEstablishmentWriteService = new Mock<IEstablishmentWriteService>(MockBehavior.Strict);
        private readonly Mock<IResourcesHelper> mockResourcesHelper = new Mock<IResourcesHelper>(MockBehavior.Strict);
        private readonly Mock<ISecurityService> mockSecurityService = new Mock<ISecurityService>(MockBehavior.Strict);
        private readonly Mock<IExternalLookupService> mockExternalLookupService = new Mock<IExternalLookupService>(MockBehavior.Strict);
        private readonly Mock<HttpRequestBase> mockHttpRequestBase = new Mock<HttpRequestBase>(MockBehavior.Strict);
        private readonly Mock<HttpContextBase> mockHttpContextBase = new Mock<HttpContextBase>(MockBehavior.Strict);
        private readonly Mock<IPrincipal> mockPrincipal = new Mock<IPrincipal>(MockBehavior.Strict);
        private readonly Mock<IIdentity> mockIdentity = new Mock<IIdentity>(MockBehavior.Strict);
        private readonly Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
        private readonly Mock<UrlHelper> mockUrlHelper = new Mock<UrlHelper>(MockBehavior.Loose);

        private readonly Mock<IFSCPDService> mockFscpdService = new Mock<IFSCPDService>(MockBehavior.Strict);
        private readonly Mock<IFBService> mockFbService = new Mock<IFBService>(MockBehavior.Strict);
        private bool disposedValue;

        public EstablishmentControllerTests()
        {
            mockCachedLookupService = MockHelper.SetupCachedLookupService();

            mockLookupService = MockHelper.SetupLookupService(mockPrincipal.Object);

            mockEstablishmentReadService.Setup(e => e.GetEstabType2EducationPhaseMap())
               .Returns(new Dictionary<eLookupEstablishmentType, eLookupEducationPhase[]>());

            mockUrlHelper.Setup(u => u.RouteUrl(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("fake url");

            controller = new EstablishmentController(
                mockEstablishmentReadService.Object,
                mockGroupReadService.Object,
                mockMapper.Object,
                mockEstablishmentWriteService.Object,
                mockCachedLookupService.Object,
                mockResourcesHelper.Object,
                mockSecurityService.Object,
                mockExternalLookupService.Object,
                mockLookupService.Object);

            SetupController();
        }


        private void SetupHttpRequest()
        {
            mockHttpRequestBase.SetupGet(x => x.QueryString)
                .Returns(HttpUtility.ParseQueryString(string.Empty));
            mockHttpContextBase.SetupGet(x => x.Request)
                .Returns(mockHttpRequestBase.Object);
            mockHttpContextBase.SetupGet(x => x.User)
                .Returns(mockPrincipal.Object);
            mockControllerContext.SetupGet(x => x.HttpContext)
                .Returns(mockHttpContextBase.Object);
            mockControllerContext.SetupGet(x => x.IsChildAction)
                .Returns(false);
            mockControllerContext.SetupGet(x => x.RouteData)
                .Returns(new System.Web.Routing.RouteData());
            mockPrincipal.SetupGet(x => x.Identity)
                .Returns(mockIdentity.Object);
        }

        protected void SetupController()
        {
            SetupHttpRequest();
            controller.ControllerContext = mockControllerContext.Object;
            mockControllerContext.SetupGet(c => c.Controller).Returns(controller);
            controller.Url = mockUrlHelper.Object;
        }

        [Fact()]
        public async Task Estab_Edit_Helpdesk_Id_NotFound()
        {
            mockEstablishmentReadService.Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => controller.EditHelpdesk(4));
        }

        [Fact]
        public async Task Estab_Edit_Helpdesk_Null_Id()
        {
            var response = await controller.EditHelpdesk(null as int?);

            Assert.IsType<HttpNotFoundResult>(response);
        }

        [Fact]
        public async Task Estab_Edit_IEBT_Id_NotFound()
        {
            mockEstablishmentReadService.Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => controller.EditIEBT(4));
        }

        [Fact]
        public async Task Estab_Edit_IEBT_Null_Id()
        {
            var response = await controller.EditIEBT(null as int?);

            Assert.IsType<HttpNotFoundResult>(response);
        }

        [Fact]
        public async Task Estab_Edit_Location_Id_NotFound()
        {
            mockEstablishmentReadService.Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => controller.EditLocation(4));
        }

        [Fact]
        public async Task Estab_Edit_Location_Null_Id()
        {
            var response = await controller.EditLocation(null as int?);

            Assert.IsType<HttpNotFoundResult>(response);
        }

        [Fact]
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
                Easting = 4,
                ChairOfProprietor = new ProprietorViewModel()
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

            mockGroupReadService.Setup(x => x.GetAllByEstablishmentUrnAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new[] { new GroupModel { Name = "Group 1", GroupUId = 1000 } });
            mockEstablishmentReadService.Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(establishment));
            mockMapper.Setup(m => m.Map<EditEstablishmentModel>(establishment))
                .Returns(editEstabModel);
            mockMapper.Setup(m => m.Map(It.IsAny<IEBTModel>(), editEstabModel))
                .Returns(editEstabModel);
            mockEstablishmentReadService.Setup(e => e.GetDisplayPolicyAsync(establishment, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new EstablishmentDisplayEditPolicy());
            mockEstablishmentReadService.Setup(e => e.GetEditPolicyAsync(establishment, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new EstablishmentEditPolicyEnvelope
                {
                    EditPolicy = new EstablishmentDisplayEditPolicy { IEBTDetail = new IEBTDetailDisplayEditPolicy() }
                });
            mockPrincipal.Setup(p => p.IsInRole(It.IsAny<string>()))
                .Returns(true);
            mockFscpdService.Setup(x => x.CheckExists(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(true);
            mockFscpdService.Setup(x => x.PublicURL(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns("https://cscp.azurewebsites.net/school/123456");
            mockFbService.Setup(x => x.CheckExists(It.IsAny<int>(), It.IsAny<FbType>()))
                .ReturnsAsync(true);
            mockFbService.Setup(x => x.PublicURL(It.IsAny<int>(), It.IsAny<FbType>()))
                .Returns("https://sfb.azurewebsites.net/school/detail?urn=123456");


            var response = await controller.EditDetails(4, address);

            Assert.IsType<ViewResult>(response);
            var viewResult = response as ViewResult;
            Assert.IsType<EditEstablishmentModel>(viewResult.Model);
            var model = viewResult.Model as EditEstablishmentModel;
            Assert.Equal(replacementAddress.Town, model.Address_CityOrTown);
            Assert.Equal(replacementAddress.CountryId, model.Address_CountryId);
            Assert.Equal(replacementAddress.CountyId, model.Address_CountyId);
            Assert.Equal(replacementAddress.Street, model.Address_Line1);
            Assert.Equal(replacementAddress.Locality, model.Address_Locality);
            Assert.Equal(replacementAddress.Address3, model.Address_Line3);
            Assert.Equal(replacementAddress.PostCode, model.Address_PostCode);
            Assert.Equal(replacementAddress.SelectedUPRN, model.Address_UPRN);
            Assert.Equal(replacementAddress.Northing, model.Northing);
            Assert.Equal(replacementAddress.Easting, model.Easting);
        }

        [Fact]
        public async Task Estab_EditDetails_Id_NotFound()
        {
            mockEstablishmentReadService.Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => controller.EditDetails(4, null));
        }

        [Fact]
        public async Task Estab_EditDetails_Null_Id()
        {
            var response = await controller.EditDetails(null, null);

            Assert.IsType<HttpNotFoundResult>(response);
        }

        [Fact]
        public async Task Estab_EditLinks_Id_NotFound()
        {
            mockEstablishmentReadService.Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => controller.EditLinks(4));
        }

        [Fact]
        public async Task Estab_EditLinks_Null_Id()
        {
            var response = await controller.EditLinks(null);

            Assert.IsType<HttpNotFoundResult>(response);
        }

        [Fact]
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

            mockGroupReadService.Setup(x => x.GetAllByEstablishmentUrnAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new[] { new GroupModel { Name = "Group 1", GroupUId = 1000 } });
            mockCachedLookupService.Setup(c => c.NationalitiesGetAllAsync())
                .ReturnsAsync(() => nationalities);
            mockCachedLookupService.Setup(c => c.CountiesGetAllAsync())
                .ReturnsAsync(() => counties);
            mockEstablishmentReadService.Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(establishment));
            mockEstablishmentReadService.Setup(e => e.GetEditPolicyAsync(It.IsAny<EstablishmentModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new EstablishmentEditPolicyEnvelope
                {
                    EditPolicy = new EstablishmentDisplayEditPolicy { IEBTDetail = new IEBTDetailDisplayEditPolicy { AccommodationChangedId = true } }
                });

            var result = await controller.AddOrReplaceEstablishmentAddressAsync(5, "test");

            Assert.NotNull(result);
            //The above assert was added when converted to XUnit, before this the test asserted nothing
        }

        [Fact]
        public async Task Estab_SearchForEstablishment_Id_NotFound()
        {
            mockEstablishmentReadService.Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => controller.SearchForEstablishment(4, null));
        }

        [Fact]
        public async Task Estab_SearchForEstablishment_Null_Id()
        {
            var response = await controller.SearchForEstablishment(null, null);

            Assert.IsType<HttpNotFoundResult>(response);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    controller.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
