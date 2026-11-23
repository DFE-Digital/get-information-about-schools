using Microsoft.Extensions.Configuration;
using System;
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
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            configuration["AppSettings:Environment"] = string.Empty;

            var config = new CacheConfig(configuration);

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
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            configuration["AppSettings:Environment"] = "dev";

            var config = new CacheConfig(configuration);

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
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            configuration["AppSettings:Environment"] = "localdev";

            var config = new CacheConfig(configuration);

            Assert.Null(config.ConnectionString);
            Assert.Equal($"localdev_{ Environment.MachineName.ToLower()}", config.CacheDomain);
            Assert.False(config.IsCentralCacheEnabled);
            Assert.True(config.IsDistributedCachingEnabled);
            Assert.True(config.IsPayloadCompressionEnabled);
            Assert.False(config.IsAuditingEnabled);
            Assert.False(config.IsExceptionPropagationEnabled);
            Assert.False(string.IsNullOrEmpty(config.Name));
            Assert.Equal($"localdev_{Environment.MachineName.ToLower()}_testkey", config.ProcessKey("testkey"));
        }
    }
}
