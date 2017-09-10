using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Moq;

namespace Edubase.UnitTest
{
    public abstract class UnitTestBase<T> : IDisposable where T : class
    {
        private readonly List<Mock> mocks = new List<Mock>();
        public List<object> RealObjects { get; set; } = new List<object>();
        private ILifetimeScope containerLifetime;

        protected T ObjectUnderTest { get; private set; }

        protected Mock<TMock> AddMock<TMock>() where TMock: class
        {
            var mock = new Mock<TMock>(MockBehavior.Strict);
            mocks.Add(mock);
            return mock;
        }

        protected Mock<TMock> GetMock<TMock>() where TMock : class
        {
            return mocks.SingleOrDefault(m => m is Mock<TMock>) as Mock<TMock>;
        }

        protected void SetupObjectUnderTest()
        {
            if (typeof(T).IsAssignableTo<Controller>())
            {
                SetupHttpRequest();
            }

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<T>().AsImplementedInterfaces().AsSelf();

            foreach (var mock in mocks)
            {
                containerBuilder.RegisterInstance(mock.Object).AsImplementedInterfaces().AsSelf();
            }

            RealObjects.ForEach(x => containerBuilder.RegisterInstance(x).AsImplementedInterfaces().AsSelf());

            var container = containerBuilder.Build();
            containerLifetime = container.BeginLifetimeScope();
            ObjectUnderTest = containerLifetime.Resolve<T>();

            var controller = ObjectUnderTest as Controller;
            if (controller != null)
            {
                controller.ControllerContext = GetMock<ControllerContext>().Object;
            }
        }

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

        protected void ResetMocks() => mocks.ForEach(x => x.Reset());

        public void Dispose()
        {
            containerLifetime?.Dispose();
        }
    }
}