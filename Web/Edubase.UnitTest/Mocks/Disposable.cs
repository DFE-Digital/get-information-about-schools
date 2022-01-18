using System;

namespace Edubase.UnitTest.Mocks
{
    public class Disposable : IDisposable
    {
        public bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) disposedValue = true;
        }

        public void Dispose() => Dispose(true);
    }
}
