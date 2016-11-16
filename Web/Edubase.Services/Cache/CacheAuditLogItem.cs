using System;

namespace Edubase.Services.Cache
{
    /// <summary>
    /// Encapsulates an audit log item
    /// </summary>
    public class CacheAuditLogItem
    {
        public eCacheEvent CachingEvent { get; set; }
        public string CallerFunctionName { get; set; }
        public DateTime DateTime { get; set; }
        public string Key { get; set; }

        public CacheAuditLogItem()
        {
            DateTime = DateTime.UtcNow;
        }
    }
}
