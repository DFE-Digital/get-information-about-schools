using System;
using MemoryCache = System.Runtime.Caching.MemoryCache;

namespace Edubase.Services.IntegrationEndPoints
{
    public static class TimeoutTracker
    {
        private static MemoryCache _cache = MemoryCache.Default;

        public static int IncrementTimeoutCount()
        {
            var currentCount = _cache["TimeoutCount"] as int? ?? 0;
            currentCount++;
            _cache.Set("TimeoutCount", currentCount, DateTimeOffset.Now.AddHours(1));

            return currentCount;
        }

        public static void ResetTimeoutCount()
        {
            _cache.Remove(("TimeoutCount"));
        }

        public static int GetTimeoutCount()
        {
            var currentCount = _cache["TimeoutCount"] as int? ?? 0;
            return currentCount;
        }
    }
}
