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

    public class TableStorageBase<T> where T : class
    {
        private readonly CloudTable _table;

        public TableStorageBase(string connectionStringName, string tableName = "")
        {
            if (connectionStringName.Clean() == null) throw new ArgumentNullException(nameof(connectionStringName));
            var connString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            if (connString.Clean() == null) throw new Exception($"The connection string for '{connectionStringName}' is empty"); 
            var cloudStorageAccount = CloudStorageAccount.Parse(connString);
            var tableClient = cloudStorageAccount.CreateCloudTableClient();

            tableClient.DefaultRequestOptions = new TableRequestOptions(tableClient.DefaultRequestOptions)
            {
                RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(2), 10)
            };

            var pluralizationService = PluralizationService.CreateService(new CultureInfo("en-GB"));
            var tsTableName = (string.IsNullOrEmpty(tableName))
                ? pluralizationService.Pluralize(typeof(T).Name)
                : tableName;

            _table = tableClient.GetTableReference(tsTableName);
            _table.CreateIfNotExists();

        }

        public CloudTable Table
        {
            get { return _table; }
        }
    }
}
