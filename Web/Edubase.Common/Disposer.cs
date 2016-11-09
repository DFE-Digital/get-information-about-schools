using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using System.Diagnostics;

namespace Edubase.Common
{
    public class Disposer : IDisposable
    {
        private IDisposable[] _disposables = null;
        private Action<long> _after;
        private Stopwatch _sw;

        /// <summary>
        /// Captures objects to be disposed later
        /// </summary>
        /// <param name="disposables"></param>
        /// <returns></returns>
        public static IDisposable Capture(params IDisposable[] disposables) => new Disposer(disposables);

        private Disposer(IDisposable[] disposables)
        {
            _disposables = disposables;
        }

        private Disposer(Action<long> after)
        {
            _after = after;
            _sw = Stopwatch.StartNew();
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
                _after?.Invoke(_sw.ElapsedMilliseconds);
            }
        }

        public static IDisposable Timed(Action before, Action<long> after)
        {
            before();
            return new Disposer(after);
        }

        public static void Using<T>(Func<T> factory, Action<T> before, Action<T> after, Action<T> meat) where T : IDisposable
        {
            var obj = factory();
            using (obj)
            {
                before(obj);
                meat(obj);
                after(obj);
            }
        }
        

        public void Dispose() => Dispose(true);
    }
}
