using Azure.Data.Tables;
using Edubase.Common.Cache;
using Edubase.Data;
using Edubase.Data.Repositories;
using Edubase.Data.Repositories.EF;
using Edubase.Services;
using Edubase.Web.UI.Controllers.Api;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Edubase.Web.UI.App_Start
{
    public static class DataServiceRegistration
    {
        public static void AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ApiRecorderSessionItemRepository>();
            services.AddSingleton<WebLogItemRepository>();
            services.AddSingleton<ISqlLaNameCodeRepository, SqlLaNameCodeRepository>();
            services.AddSingleton<ILaNameCodeFileGenerator, LaNameCodeFileGenerator>();
            services.AddScoped<FrontEndDbContext>(sp => new FrontEndDbContext(new SqlConnection(sp.GetRequiredService<IConfiguration>().GetConnectionString("FrontEndDatabase"))));           
            services.AddScoped<IUserPreferenceRepository, SqlUserPreferenceRepository>();
            services.AddScoped<ITokenRepository, SqlTokenRepository>();
            services.AddScoped<ILocalAuthoritySetRepository, SqlLocalAuthoritySetRepository>();
            services.AddScoped<IGlossaryRepository, SqlGlossaryItemRepository>();
            services.AddScoped<IFaqGroupRepository, SqlFaqGroupRepository>();
            services.AddScoped<IFaqItemRepository, SqlFaqItemRepository>();
            services.AddScoped<INotificationTemplateRepository, SqlNotificationTemplateRepository>();
            services.AddScoped<IDataQualityStatusRepository, SqlDataQualityStatusRepository>();
            services.AddScoped<INewsArticleRepository, SqlNewsArticleRepository>();
            services.AddScoped<INotificationBannerRepository, SqlNotificationBannerRepository>();

            const string DataConnectionString = "DataConnectionString";
            const string RedisConnectionString = "Redis";
            var dataConnectionString = configuration.GetConnectionString(DataConnectionString);
            var redisConnectionString = configuration.GetConnectionString(RedisConnectionString);

            services.AddTransient<IBlobService>(sp =>
            {
                return new BlobService(dataConnectionString);
            });

            var tableServiceClient = new TableServiceClient(dataConnectionString);            
            services.AddSingleton(tableServiceClient);
            services.AddSingleton<ILoggingService>(provider =>
            {
                var tableService = provider.GetRequiredService<TableServiceClient>();
                const string tableName = "AZTLoggerMessages";
                return new LoggingService(tableService, tableName);
            });

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(redisConnectionString);
            });

            services.AddSingleton<ICacheAccessor>(provider =>
            {
                var converters = provider.GetRequiredService<JsonConverterCollection>();
                var redis = provider.GetRequiredService<IConnectionMultiplexer>();
                return new CacheAccessor(converters, redis);
            });

            DbGeographyConverter dbGeographyConverter = new();
            JsonConverterCollection jsonConverterCollection = [dbGeographyConverter];
            services.AddSingleton(jsonConverterCollection);
        }
    }
}
