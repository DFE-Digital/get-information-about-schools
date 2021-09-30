using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.ExternalLookup;
using Edubase.Services.Lookup;
using Moq;
using MoreLinq;

namespace Edubase.UnitTest
{
    public abstract class UnitTestBase<T> : IDisposable where T : class
    {
        private readonly Dictionary<Type, Mock> mocks = new Dictionary<Type, Mock>();
        public List<object> RealObjects { get; set; } = new List<object>();
        private ILifetimeScope containerLifetime;

        protected T ObjectUnderTest { get; private set; }

        protected Mock<TMock> AddMock<TMock>(bool strict = true) where TMock: class
        {
            var mock = strict ? new Mock<TMock>(MockBehavior.Strict) : new Mock<TMock>(MockBehavior.Loose);
            mocks.Add(typeof(TMock), mock);
            return mock;
        }

        protected Mock<TMock> GetMock<TMock>() where TMock : class
        {
            return mocks.SingleOrDefault(m => m.Value is Mock<TMock>).Value as Mock<TMock>;
        }

        protected void SetupObjectUnderTest()
        {
            if (typeof(T).IsAssignableTo<Controller>())
            {
                SetupHttpRequest();
            }

            var containerBuilder = new ContainerBuilder();
            //containerBuilder.RegisterModule<ValidationModule>();
            containerBuilder.RegisterType<T>().AsImplementedInterfaces().AsSelf();

            foreach (var mock in mocks)
            {
                containerBuilder.RegisterInstance(mock.Value.Object).AsImplementedInterfaces().AsSelf();
                if (!mock.Key.IsInterface)
                {
                    containerBuilder.RegisterInstance(mock.Value.Object).As(mock.Key);
                }
            }

            RealObjects.ForEach(x => containerBuilder.RegisterInstance(x).AsImplementedInterfaces().AsSelf());

            var container = containerBuilder.Build();
            containerLifetime = container.BeginLifetimeScope();
            ObjectUnderTest = containerLifetime.Resolve<T>();

            var controller = ObjectUnderTest as Controller;
            if (controller != null)
            {
                controller.ControllerContext = GetMock<ControllerContext>().Object;
                GetMock<ControllerContext>().SetupGet(c => c.Controller).Returns(controller);
                controller.Url = GetMock<UrlHelper>().Object;
            }
        }

        //protected void SetupFluentValidation()
        //{ 
        //    var fluentValidationModelValidatorProvider = new FluentValidationModelValidatorProvider(new AutofacValidatorFactory(IocConfig.AutofacDependencyResolver));
        //    DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
        //    fluentValidationModelValidatorProvider.AddImplicitRequiredValidator = false;
        //    ModelValidatorProviders.Providers.Add(fluentValidationModelValidatorProvider);
        //}

        /// <summary>
        /// Sets up mocks for HttpRequestBase, HttpContextBase, IPrincipal, IIdentity & ControllerContext
        /// </summary>
        protected virtual void InitialiseMocks()
        {
            AddMock<HttpRequestBase>();
            AddMock<HttpContextBase>();
            AddMock<IPrincipal>();
            AddMock<IIdentity>();
            AddMock<ControllerContext>();
            AddMock<UrlHelper>(false);
            GetMock<UrlHelper>()
                .Setup(u => u.RouteUrl(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("fake url");
            AddMock<ICSCPService>();
            AddMock<IFBService>();
        }

        private void SetupHttpRequest()
        {
            GetMock<HttpRequestBase>().SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            GetMock<HttpContextBase>().SetupGet(x => x.Request).Returns(GetMock<HttpRequestBase>().Object);
            GetMock<HttpContextBase>().SetupGet(x => x.User).Returns(GetMock<IPrincipal>().Object);
            GetMock<ControllerContext>().SetupGet(x => x.HttpContext).Returns(GetMock<HttpContextBase>().Object);
            GetMock<ControllerContext>().SetupGet(x => x.IsChildAction).Returns(false);
            GetMock<ControllerContext>().SetupGet(x => x.RouteData).Returns(new System.Web.Routing.RouteData());
            GetMock<IPrincipal>().SetupGet(x => x.Identity).Returns(GetMock<IIdentity>().Object);
        }

        protected void ResetMocks() => mocks.ForEach(x => x.Value.Reset());

        protected void SetupCachedLookupService()
        {
            var cls = GetMock<ICachedLookupService>();
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


            GetMock<IEstablishmentReadService>().Setup(e => e.GetEstabType2EducationPhaseMap()).Returns(new Dictionary<eLookupEstablishmentType, eLookupEducationPhase[]>());
        }

        public void Dispose()
        {
            containerLifetime?.Dispose();
        }
    }
}
