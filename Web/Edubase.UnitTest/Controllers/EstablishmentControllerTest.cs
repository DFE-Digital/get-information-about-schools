using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
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
using Moq;
using NUnit.Framework;

namespace Edubase.UnitTest.Controllers
{
    [TestFixture]
    public class EstablishmentControllerTest : UnitTestBase<EstablishmentController>
    {
        public EstablishmentControllerTest()
        {
            AddMock<IEstablishmentReadService>();
            AddMock<IGroupReadService>();
            AddMock<IMapper>();
            AddMock<IEstablishmentWriteService>();
            AddMock<ICachedLookupService>();
            AddMock<IResourcesHelper>();
            AddMock<ISecurityService>();

            SetupObjectUnderTest();
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

            Assert.That(async () => await ObjectUnderTest.EditDetails(4, null), 
                Throws.TypeOf<EntityNotFoundException>(), "Expected exception of type EntityNotFoundException");
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
            var expectedExceptionThrown = false;
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            try
            {
                await ObjectUnderTest.EditHelpdesk(4);
            }
            catch (Exception e)
            {
                expectedExceptionThrown = ExceptionContains<EntityNotFoundException>(e);
            }

            Assert.IsTrue(expectedExceptionThrown, "Expected exception of type EntityNotFoundException");
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
            var expectedExceptionThrown = false;
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            try
            {
                await ObjectUnderTest.EditLocation(4);
            }
            catch (Exception e)
            {
                expectedExceptionThrown = ExceptionContains<EntityNotFoundException>(e);
            }

            Assert.IsTrue(expectedExceptionThrown, "Expected exception of type EntityNotFoundException");
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
            var expectedExceptionThrown = false;
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            try
            {
                await ObjectUnderTest.EditIEBT(4);
            }
            catch (Exception e)
            {
                expectedExceptionThrown = ExceptionContains<EntityNotFoundException>(e);
            }

            Assert.IsTrue(expectedExceptionThrown, "Expected exception of type EntityNotFoundException");
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
            var expectedExceptionThrown = false;
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            try
            {
                await ObjectUnderTest.SearchForEstablishment(4, null);
            }
            catch (Exception e)
            {
                expectedExceptionThrown = ExceptionContains<EntityNotFoundException>(e);
            }

            Assert.IsTrue(expectedExceptionThrown, "Expected exception of type EntityNotFoundException");
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
    }
}
