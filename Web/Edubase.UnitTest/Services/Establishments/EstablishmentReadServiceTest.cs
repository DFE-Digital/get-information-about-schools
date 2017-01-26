using AutoMapper;
using Edubase.Data.Entity;
using Edubase.Services;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Exceptions;
using Edubase.Services.IntegrationEndPoints.AzureSearch;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using Edubase.Services.Lookup;
using Edubase.Services.Mapping;
using Edubase.Services.Security;
using Edubase.UnitTest.Mocks.IntegrationEndPoints;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.UnitTest.Services.Establishments
{
    [TestFixture]
    public class EstablishmentReadServiceTest
    {
        [Test]
        public async Task GetEstablishment_ReturnsDetail()
        {
            // Arrange
            var mockDbContext = new MockApplicationDbContext();
            mockDbContext.Establishments.Add(new Establishment { Urn = 1, StatusId = (int)eLookupEstablishmentStatus.Quarantine });
            var fullAccessRoles = new[] { EdubaseRoles.EFA, EdubaseRoles.AOS, EdubaseRoles.FSG, EdubaseRoles.IEBT, EdubaseRoles.School, EdubaseRoles.PRU };

            var subject = new EstablishmentReadService(mockDbContext, CreateMapper(), new Mock<ICachedLookupService>().Object, 
                new Mock<IAzureSearchEndPoint>().Object, null, null, null, null);

            foreach (var role in fullAccessRoles)
            {
                var user = new GenericPrincipal(new GenericIdentity("test.user"), new[] { role });
                var model = await subject.GetAsync(1, user);
                Assert.IsNotNull(model);
            }

            var restrictedUser = new GenericPrincipal(new GenericIdentity("test.user"), new[] { EdubaseRoles.Establishment });
            var model2 = await subject.GetAsync(1, restrictedUser);
            Assert.IsNull(model2);
        }

        [Test]
        public async Task GetEstablishmentReturnsDetailForAllUsersWhenStatusIsOpen()
        {
            // Arrange
            var mockDbContext = new MockApplicationDbContext();
            mockDbContext.Establishments.Add(new Establishment { Urn = 1, StatusId = (int)eLookupEstablishmentStatus.Open });

            var subject = new EstablishmentReadService(mockDbContext, CreateMapper(), new Mock<ICachedLookupService>().Object, 
                new Mock<IAzureSearchEndPoint>().Object, null, null, null, null);

            foreach (var role in EdubaseRoles.AllRoles)
            {
                var user = new GenericPrincipal(new GenericIdentity("test.user"), new[] { role });
                var model = await subject.GetAsync(1, user);
                Assert.IsNotNull(model);
            }
        }

        [Test]
        public async Task GetEstablishment_DoesNotReturnsDetailWhenIsDeleted()
        {
            // Arrange
            var mockDbContext = new MockApplicationDbContext();
            mockDbContext.Establishments.Add(new Establishment { Urn = 1, StatusId = (int)eLookupEstablishmentStatus.Open, IsDeleted = true });
            var fullAccessRoles = new[] { EdubaseRoles.EFA, EdubaseRoles.AOS, EdubaseRoles.FSG, EdubaseRoles.IEBT, EdubaseRoles.School, EdubaseRoles.PRU };

            var subject = new EstablishmentReadService(mockDbContext, CreateMapper(), 
                new Mock<ICachedLookupService>().Object, 
                new Mock<IAzureSearchEndPoint>().Object,
                null,
                null, null, null);

            foreach (var role in fullAccessRoles)
            {
                var user = new GenericPrincipal(new GenericIdentity("test.user"), new[] { role });
                var model = await subject.GetAsync(1, user);
                Assert.IsNull(model);
            }

            var restrictedUser = new GenericPrincipal(new GenericIdentity("test.user"), new[] { EdubaseRoles.Establishment });
            var model2 = await subject.GetAsync(1, restrictedUser);
            Assert.IsNull(model2);
        }

        [Test]
        public void GetEstablishmentChangeHistory_DoesNotReturnsDetailWhenUserNotAuthenticated()
        {
            var mockDbContext = new MockApplicationDbContext();
            var user = new GenericPrincipal(new GenericIdentity(string.Empty), new string[0]);
            var subject = new EstablishmentReadService(mockDbContext, CreateMapper(), 
                new Mock<ICachedLookupService>().Object, new Mock<IAzureSearchEndPoint>().Object,
                null, null, null, null);
            Assert.ThrowsAsync<PermissionDeniedException>(async () 
                => await subject.GetChangeHistoryAsync(1, 1, user));
        }

        [Test]
        public async Task GetEstablishmentChangeHistory_ReturnsDetail()
        {
            var mockDbContext = new MockApplicationDbContext();
            mockDbContext.EstablishmentChangeHistories.Add(new EstablishmentChangeHistory
            {
                ApproverUserId = "550489809582430983",
                Name = "LocalAuthorityId",
                OldValue = ((int)eLocalAuthority.Westminster).ToString(),
                NewValue = ((int)eLocalAuthority.BarkingAndDagenham).ToString(),
                Urn = 1,
                ApproverUser = new ApplicationUser(),
                OriginatorUser = new ApplicationUser()
            });

            var cacheLookupSvc = new Mock<ICachedLookupService>();
            cacheLookupSvc.Setup(x => x.IsLookupField("LocalAuthorityId")).Returns(true);
            cacheLookupSvc.Setup(x => x.GetNameAsync(It.IsAny<string>(), 213)).Returns(Task.FromResult("Westminster"));
            cacheLookupSvc.Setup(x => x.GetNameAsync(It.IsAny<string>(), 301)).Returns(Task.FromResult("BarkingAndDagenham"));
            
            var user = new GenericPrincipal(new GenericIdentity("test.user"), new string[0]);

            

            var subject = new EstablishmentReadService(mockDbContext, CreateMapper(), 
                cacheLookupSvc.Object, new Mock<IAzureSearchEndPoint>().Object,
                null, null, null, null);

            var set = await subject.GetChangeHistoryAsync(1, 1, user);
            var model = set.First();

            Assert.AreEqual(eLocalAuthority.Westminster.ToString(), model.OldValue);
            Assert.AreEqual(eLocalAuthority.BarkingAndDagenham.ToString(), model.NewValue);
        }


        [Test]
        public async Task SearchEstablishment_AnonUserIsProvidedRestrictedResults()
        {
            var cacheLookupSvc = new Mock<ICachedLookupService>();
            var azs = new Mock<IAzureSearchEndPoint>(MockBehavior.Strict);
            var user = new GenericPrincipal(new GenericIdentity(""), new string[0]);
            azs.Setup(x => x.SearchAsync<SearchEstablishmentDocument>(It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IList<string>>(), 
                It.IsAny<IList<string>>())).Returns(Task.FromResult(null as AzureSearchResult<SearchEstablishmentDocument>));
                

            var subject = new EstablishmentReadService(new MockApplicationDbContext(), CreateMapper(), 
                cacheLookupSvc.Object, azs.Object, null, null, null, null);

            var result = await subject.SearchAsync(new EstablishmentSearchPayload(10, 20), user);

            var p = new EstablishmentSearchPayload();
            p.Filters.StatusIds = new[]
            {
                eLookupEstablishmentStatus.Closed,
                eLookupEstablishmentStatus.Open,
                eLookupEstablishmentStatus.OpenButProposedToClose,
                eLookupEstablishmentStatus.ProposedToOpen
            }.Select(x => (int)x).ToArray();

            azs.Verify(x => x.SearchAsync<SearchEstablishmentDocument>(EstablishmentsSearchIndex.INDEX_NAME, null,
                It.Is<string>(y => y.Contains(p.Filters.ToString())), 10, 20, 
                It.Is<IList<string>>(y => y.Contains("Name")), 
                It.Is<IList<string>>(y => y.Contains("Name"))));

            Assert.IsNull(result);
        }

        [Test]
        public async Task SearchEstablishment_FSGUserIsProvidedUnrestrictedResults()
        {
            var cacheLookupSvc = new Mock<ICachedLookupService>();
            var azs = new Mock<IAzureSearchEndPoint>(MockBehavior.Strict);
            var user = new GenericPrincipal(new GenericIdentity("fsg"), new [] { EdubaseRoles.FSG });
            azs.Setup(x => x.SearchAsync<SearchEstablishmentDocument>(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IList<string>>(),
                It.IsAny<IList<string>>())).Returns(Task.FromResult(null as AzureSearchResult<SearchEstablishmentDocument>));

            var subject = new EstablishmentReadService(new MockApplicationDbContext(), CreateMapper(), 
                cacheLookupSvc.Object, azs.Object, null, null, null, null);
            var result = await subject.SearchAsync(new EstablishmentSearchPayload(10, 20), user);

            azs.Verify(x => x.SearchAsync<SearchEstablishmentDocument>(EstablishmentsSearchIndex.INDEX_NAME, null,
                It.Is<string>(y => y.Equals(AzureSearchEndPoint.ODATA_FILTER_DELETED)), 10, 20,
                It.Is<IList<string>>(y => y.Contains("Name")),
                It.Is<IList<string>>(y => y.Contains("Name"))));

            Assert.IsNull(result);
        }

        private IMapper CreateMapper()=> new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperServicesProfile>()).CreateMapper();
    }
}
