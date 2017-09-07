using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Integration.Mvc;
using Moq;

namespace Edubase.UnitTest
{
    public abstract class UnitTestBase<T> where T : class
    {
        private readonly List<Mock> mocks = new List<Mock>();

        internal T ObjectUnderTest { get; private set; }
        internal AutofacDependencyResolver DependencyResolver { get; private set; }

        internal Mock<TMock> AddMock<TMock>() where TMock: class
        {
            var mock = new Mock<TMock>(MockBehavior.Strict);
            mocks.Add(mock);
            return mock;
        }

        internal Mock<TMock> GetMock<TMock>() where TMock : Mock
        {
            return mocks.SingleOrDefault(m => m is Mock<TMock>) as Mock<TMock>;
        }

        internal void SetupObjectUnderTest()
        {
            var containerBuilder = new ContainerBuilder();
            foreach (var mock in mocks)
            {
                containerBuilder.RegisterInstance(mock.Object).AsImplementedInterfaces().AsSelf();
            }
            
            DependencyResolver = new AutofacDependencyResolver(containerBuilder.Build());
            ObjectUnderTest = DependencyResolver.GetService(typeof(T)) as T;
        }
    }
}