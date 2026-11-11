namespace Edubase.Data.Repositories.TableStorage
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Configuration;
    using Common;

    public class TableStorageBase<T> where T : class
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

            var tsTableName = string.IsNullOrEmpty(tableName)
                ? GetDefaultTableName(typeof(T).Name)
                : tableName;

            Table = tableClient.GetTableReference(tsTableName);
            Table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        public CloudTable Table { get; }

        private static string GetDefaultTableName(string typeName)
        {
            if (typeName.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return typeName;
            }

            return string.Concat(typeName, "s");
        }
    }
}
