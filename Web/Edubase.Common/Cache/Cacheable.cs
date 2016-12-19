using System;

namespace Edubase.Common.Cache
{
    /// <summary>
    /// Wrapper for all cacheable items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Cacheable<T>
    {
        public T Payload { get; set; }
        public DateTime? ExpirationUtc { get; set; }

        public Cacheable(T obj)
        {
            Payload = obj;
        }
    }
}
