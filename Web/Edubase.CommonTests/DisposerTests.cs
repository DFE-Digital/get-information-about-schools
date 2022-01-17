using Xunit;
using Edubase.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common.Tests
{
    public class DisposerTests
    {
        [Fact]
        public void ObjectIsDisposed()
        {
            var subject = new Disposable();
            using (Disposer.Capture(subject)) { }
            Assert.True(subject.disposedValue);
        }

        [Fact]
        public void ObjectsAreDisposed()
        {
            var subjects = new[] { new Disposable(), new Disposable(), new Disposable(), new Disposable(), new Disposable() };
            using (Disposer.Capture(subjects)) { }

            foreach(var subject in subjects)
            {
                Assert.True(subject.disposedValue);
            }
        }

        [Fact]
        public void UsingInvokesMethodsInRightOrder()
        {
            var test = "";
            Disposer.Using(() => new Disposable(), x => test += "1", x => test += "3", x => test += "2");
            Assert.Equal("123", test);
        }
    }

    public class Disposable : IDisposable
    {
        public bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);
    }
}
