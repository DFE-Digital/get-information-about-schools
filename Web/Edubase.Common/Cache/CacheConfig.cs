using System;
using System.Configuration;

namespace Edubase.Common.Cache
{
    public class CacheConfig
    {   
        public string ConnectionString { get; set; }
        
        public string Name { set; get; }

        /// <summary>
        /// Disables the central/redis cache
        /// </summary>
        public bool IsCentralCacheEnabled { get; set; }

        /// <summary>
        /// Whether to distribute the cache into the memory of all the nodes; when disabled, cache operations will operate on the central redis server 
        /// (no in-memory cache)
        /// </summary>
        public bool IsDistributedCachingEnabled { get; set; }

        public bool IsAuditingEnabled { get; set; }

        /// <summary>
        /// When there's a cache set/get event and an exception occurs;
        /// whether to propagate those exceptions.
        /// </summary>
        public bool IsExceptionPropagationEnabled { get; set; }

        public bool IsPayloadCompressionEnabled { get; set; } = true;

        public string CacheDomain { get; set; }

        public CacheConfig()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["Redis"]?.ConnectionString;
            Name = "Cache_" + Guid.NewGuid().ToString("N").ToUpper();
            IsDistributedCachingEnabled = true;
            IsAuditingEnabled = false;
            IsCentralCacheEnabled = true;
            CacheDomain = ConfigurationManager.AppSettings["Environment"] ?? string.Empty;

            if (CacheDomain.Equals("localdev", StringComparison.OrdinalIgnoreCase))
            {
                CacheDomain += "_" + Environment.MachineName;
            }

            CacheDomain = CacheDomain?.ToLower();
        }

        internal string ProcessKey(string key)
        {
            return CacheDomain.Clean() != null && key.Clean() != null ? string.Concat(CacheDomain, "_", key) : key;
        }
    }
}
