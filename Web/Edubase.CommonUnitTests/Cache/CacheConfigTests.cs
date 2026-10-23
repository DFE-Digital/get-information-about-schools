using System;
using System.Configuration;
using System.IO;
using System.Security;
using Edubase.Common.Cache;
using Xunit;

namespace Edubase.CommonUnitTests.Cache
{
    public class CacheConfigTests
    {
        [Fact]
        public void ConstructorUsesExpectedDefaultsWhenSettingsAreMissing()
        {
            ConfigurationManager.AppSettings["Environment"] = string.Empty;

            var config = new CacheConfig();

            Assert.Null(config.ConnectionString);
            Assert.Equal(string.Empty, config.CacheDomain);
            Assert.True(config.IsCentralCacheEnabled);
            Assert.True(config.IsDistributedCachingEnabled);
            Assert.True(config.IsPayloadCompressionEnabled);
            Assert.False(config.IsAuditingEnabled);
            Assert.False(config.IsExceptionPropagationEnabled);
            Assert.False(string.IsNullOrEmpty(config.Name));
            Assert.Equal("testkey",config.ProcessKey("testkey"));
        }

        [Fact]
        public void ConstructorUsesDevSettings()
        {
            ConfigurationManager.AppSettings["Environment"] = "dev";

            var config = new CacheConfig();

            Assert.Null(config.ConnectionString);
            Assert.Equal("dev", config.CacheDomain);
            Assert.True(config.IsCentralCacheEnabled);
            Assert.True(config.IsDistributedCachingEnabled);
            Assert.True(config.IsPayloadCompressionEnabled);
            Assert.False(config.IsAuditingEnabled);
            Assert.False(config.IsExceptionPropagationEnabled);
            Assert.False(string.IsNullOrEmpty(config.Name));
            Assert.Equal("dev_testkey", config.ProcessKey("testkey"));
        }

        [Fact]
        public void ConstructorUsesLocalDevSettings()
        {
            ConfigurationManager.AppSettings["Environment"] = "localdev";

            var config = new CacheConfig();

            Assert.Null(config.ConnectionString);
            Assert.Equal("localdev" + "_" + Environment.MachineName.ToLower(), config.CacheDomain);
            Assert.False(config.IsCentralCacheEnabled);
            Assert.True(config.IsDistributedCachingEnabled);
            Assert.True(config.IsPayloadCompressionEnabled);
            Assert.False(config.IsAuditingEnabled);
            Assert.False(config.IsExceptionPropagationEnabled);
            Assert.False(string.IsNullOrEmpty(config.Name));
            Assert.Equal("localdev_sl635592_testkey", config.ProcessKey("testkey"));
        }
    }
}
