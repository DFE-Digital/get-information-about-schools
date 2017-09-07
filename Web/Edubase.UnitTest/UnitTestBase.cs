using System;
using System.Collections.Generic;
using System.Linq;
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
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<T>().AsImplementedInterfaces().AsSelf();

            foreach (var mock in mocks)
            {
                containerBuilder.RegisterInstance(mock.Object).AsImplementedInterfaces().AsSelf();
            }

            var container = containerBuilder.Build();
            containerLifetime = container.BeginLifetimeScope();
            ObjectUnderTest = containerLifetime.Resolve<T>();
        }

        internal bool ExceptionContains<TException>(Exception exception) where TException : Exception
        {
            while (exception != null)
            {
                if (exception is TException)
                {
                    return true;
                }

                exception = exception.InnerException;
            }

            return false;
        }

        public void Dispose()
        {
            containerLifetime?.Dispose();
        }
    }
}