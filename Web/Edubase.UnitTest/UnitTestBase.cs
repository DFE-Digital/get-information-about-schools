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
        private ILifetimeScope containerLifetime;

        internal T ObjectUnderTest { get; private set; }
        
        internal Mock<TMock> AddMock<TMock>() where TMock: class
        {
            var mock = new Mock<TMock>(MockBehavior.Strict);
            mocks.Add(mock);
            return mock;
        }

        internal Mock<TMock> GetMock<TMock>() where TMock : class
        {
            return mocks.SingleOrDefault(m => m is Mock<TMock>) as Mock<TMock>;
        }

        internal void SetupObjectUnderTest()
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

            var container = containerBuilder.Build();
            containerLifetime = container.BeginLifetimeScope();
            ObjectUnderTest = containerLifetime.Resolve<T>();

            var controller = ObjectUnderTest as Controller;
            if (controller != null)
            {
                controller.ControllerContext = GetMock<ControllerContext>().Object;
            }
        }

        private void SetupHttpRequest()
        {
            AddMock<HttpRequestBase>();
            AddMock<HttpContextBase>();
            AddMock<IPrincipal>();
            AddMock<ControllerContext>();

            GetMock<HttpRequestBase>().SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            GetMock<HttpContextBase>().SetupGet(x => x.Request).Returns(GetMock<HttpRequestBase>().Object);
            GetMock<HttpContextBase>().SetupGet(x => x.User).Returns(GetMock<IPrincipal>().Object);
            GetMock<ControllerContext>().SetupGet(x => x.HttpContext).Returns(GetMock<HttpContextBase>().Object);
            GetMock<ControllerContext>().SetupGet(x => x.IsChildAction).Returns(false);
        }

        public void Dispose()
        {
            containerLifetime?.Dispose();
        }
    }
}