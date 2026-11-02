using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Security.Principal;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;

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
                new GenericIdentity("UnitTestUser"), Array.Empty<string>());
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

            var httpContext = new DefaultHttpContext
            {
                // ClaimsPrincipal(IPrincipal) preserves the existing test identity and roles.
                User = new ClaimsPrincipal(testPrincipal)
            };

            var queryDictionary = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
            foreach (string key in queryString.AllKeys)
            {
                if (key == null)
                    continue;

                queryDictionary[key] = new StringValues(queryString.GetValues(key) ?? Array.Empty<string>());
            }

            httpContext.Request.Query = new QueryCollection(queryDictionary);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
                RouteData = new RouteData()
            };

            // Minimal URL mock for tests not asserting generated URLs.
            // Configure specific routes in tests that check URL strings.
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper
                .Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
                .Returns((UrlRouteContext routeContext) => routeContext?.Values?.ToString() ?? "/");

            controller.Url = mockUrlHelper.Object;
            return controller;
        }

        protected void SetupCommonLookupMocks()
        {
            mockCachedLookupService.Setup(s => s.NationalitiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.GovernorAppointingBodiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.TitlesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.GovernorRolesGetAllAsync())
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

        protected GovernorModel CreateSharedChair(
            int id, int estabUrn, DateTime? start = null, DateTime? end = null)
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

