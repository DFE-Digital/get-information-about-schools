using System.Threading.Tasks;

namespace Edubase.Data.Repositories.TableStorage
{
    using System.Globalization;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using System.Data.Entity.Design.PluralizationServices;
    using System.Configuration;
    using Common;
    using System;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    public class TableStorageBase<T> where T : class, ITableEntity, new()
    {
        public TableStorageBase(string connectionStringName, string tableName = "")
        {
            if (connectionStringName.Clean() == null)
                throw new ArgumentNullException(nameof(connectionStringName));

            var connString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            if (connString.Clean() == null)
                throw new Exception($"The connection string for '{connectionStringName}' is empty");

            var cloudStorageAccount = CloudStorageAccount.Parse(connString);
            var tableClient = cloudStorageAccount.CreateCloudTableClient();

            tableClient.DefaultRequestOptions = new TableRequestOptions(tableClient.DefaultRequestOptions)
            {
                RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(2), 10)
            };

            var pluralizationService = PluralizationService.CreateService(new CultureInfo("en-GB"));
            var tsTableName = string.IsNullOrEmpty(tableName)
                ? pluralizationService.Pluralize(typeof(T).Name)
                : tableName;

            Table = tableClient.GetTableReference(tsTableName);
            Table.CreateIfNotExists();
        }

        public async Task InsertOrUpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var insertOrReplace = TableOperation.InsertOrReplace(entity);
            await Table.ExecuteAsync(insertOrReplace);
        }

        public CloudTable Table { get; }
    }
}
