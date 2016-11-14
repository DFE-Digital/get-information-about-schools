using System;

namespace Edubase.Services.Cache
{
    /// <summary>
    /// Wrapper for all cacheable items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    internal class Cacheable
    {
        public object Payload { get; set; }
        public DateTime? ExpirationUtc { get; set; }

        public Cacheable(object obj)
        {
            Payload = obj;
        }
    }
}
