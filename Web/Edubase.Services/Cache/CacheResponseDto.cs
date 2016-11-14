using System;
using System.Diagnostics;

namespace Edubase.Services.Cache
{
    /// <summary>
    /// Represents a DTO for meta-data around a [Get] operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheResponseDto<T>
    {
        private Stopwatch _sw;

        public T Data { get; set; }
        public bool IsFromInMemoryCache { get; set; }
        public bool IsFromCentralCacheServer { get; set; }
        public bool DidDeserialize { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public Exception Exception { get; set; }

        public CacheResponseDto<T> StartTiming()
        {
            _sw = Stopwatch.StartNew();
            return this;
        }

        public CacheResponseDto<T> StopTiming()
        {
            _sw.Stop();
            ElapsedMilliseconds = _sw.ElapsedMilliseconds;
            _sw = null;
            return this;
        }
    }
}
