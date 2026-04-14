using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Governors.Models;
using Edubase.Web.UI.Areas;
using global::Edubase.Services.Establishments;
using global::Edubase.Services.Governors;
using global::Edubase.Services.Groups;
using global::Edubase.Services.Lookup;
using global::Edubase.Web.UI.Areas.Governors.Controllers;
using global::Edubase.Web.UI.Helpers;
using global::Edubase.Web.UIUnitTests.Areas.Governors.Helpers;
using Moq;

namespace Edubase.Web.UIUnitTests.Areas.Governors.TestBase
{
    namespace Edubase.Web.UIUnitTests.Areas.Governors.TestBase
    {
        public abstract class GovernorControllerTestBase
        {
            protected Mock<IGovernorsReadService> mockGovernorsReadService;
            protected Mock<IGovernorsWriteService> mockGovernorsWriteService;
            protected Mock<ICachedLookupService> mockCachedLookupService;
            protected Mock<IGroupReadService> mockGroupReadService;
            protected Mock<IEstablishmentReadService> mockEstablishmentReadService;
            protected Mock<ILayoutHelper> mockLayoutHelper;
            protected Mock<IGovernorsGridViewModelFactory> mockGridViewModelFactory;

            protected IPrincipal testPrincipal;

            protected GovernorControllerTestBase()
            {
                mockGovernorsReadService = new Mock<IGovernorsReadService>(MockBehavior.Strict);
                mockGovernorsWriteService = new Mock<IGovernorsWriteService>(MockBehavior.Strict);
                mockCachedLookupService = new Mock<ICachedLookupService>(MockBehavior.Strict);
                mockGroupReadService = new Mock<IGroupReadService>(MockBehavior.Strict);
                mockEstablishmentReadService = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
                mockLayoutHelper = new Mock<ILayoutHelper>(MockBehavior.Strict);
                mockGridViewModelFactory = new Mock<IGovernorsGridViewModelFactory>(MockBehavior.Strict);

                testPrincipal = new GenericPrincipal(
                    new GenericIdentity("UnitTestUser"), new string[0]);
            }

            protected GovernorController BuildController()
            {
                return BuildController(new NameValueCollection());
            }

            protected GovernorController BuildController(NameValueCollection queryString)
            {
                var controller = new GovernorController(
                    mockGovernorsReadService.Object,
                    mockCachedLookupService.Object,
                    mockGovernorsWriteService.Object,
                    mockGroupReadService.Object,
                    mockEstablishmentReadService.Object,
                    mockLayoutHelper.Object);

                var requestContext = new RequestContext(
                    new FakeHttpContext(testPrincipal, queryString),
                    new RouteData());

                controller.ControllerContext = new ControllerContext(requestContext, controller);

                TestControllerBuilderHelper.RegisterRoutes();

                controller.Url = new FakeUrlHelper();

                return controller;
            }

            protected void SetupCommonLookupMocks()
            {
                mockCachedLookupService
                    .Setup(s => s.NationalitiesGetAllAsync())
                    .ReturnsAsync(new List<LookupDto>());

                mockCachedLookupService
                    .Setup(s => s.GovernorAppointingBodiesGetAllAsync())
                    .ReturnsAsync(new List<LookupDto>());

                mockCachedLookupService
                    .Setup(s => s.TitlesGetAllAsync())
                    .ReturnsAsync(new List<LookupDto>());

                mockCachedLookupService
                .Setup(s => s.GovernorRolesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            }

            protected GovernorModel CreateExistingChair(int id, DateTime? endDate = null)
            {
                return new GovernorModel
                {
                    Id = id,
                    RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody,
                    AppointmentEndDate = endDate ?? DateTime.Today
                };
            }

            protected GovernorModel CreateSharedChair(int id, int estabUrn, DateTime? start = null, DateTime? end = null)
            {
                var startDate = start ?? DateTime.Today.AddYears(-1);
                var endDate = end ?? DateTime.Today.AddYears(1);

                return new GovernorModel
                {
                    Id = id,
                    RoleId = (int) eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody,
                    AppointmentStartDate = startDate,
                    AppointmentEndDate = endDate,
                    Appointments = new[]
                    {
                        new GovernorAppointment
                        {
                            EstablishmentUrn = estabUrn,
                            AppointmentStartDate = startDate,
                            AppointmentEndDate = endDate
                        }
        }
                };
            }
        }
    }
}
