using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;

namespace Edubase.Common
{
    public class Disposer : IDisposable
    {
        private IDisposable[] _disposables = null;

        /// <summary>
        /// Captures objects to be disposed later
        /// </summary>
        /// <param name="disposables"></param>
        /// <returns></returns>
        public static Disposer Capture(params IDisposable[] disposables) => new Disposer(disposables);

        private Disposer(IDisposable[] disposables)
        {
            _disposables = disposables;
        }
        
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_disposables != null)
                    {
                        _disposables.Where(x => x != null).ForEach(x => x.Dispose());
                    }
                }
                _disposables = null;
                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);
    }
}
