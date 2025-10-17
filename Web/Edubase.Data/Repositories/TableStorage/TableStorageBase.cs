using System;
using System.Configuration;
using Azure.Core;
using Azure.Data.Tables;
using Edubase.Common;
using PluralizeService.Core;

namespace Edubase.Data.Repositories.TableStorage;

public class TableStorageBase<T> where T : class, ITableEntity, new()
{
    public TableClient Table { get; }

    public TableStorageBase(string connectionStringName, string tableName = "")
    {
        string sanitisedConnectionStringName = (connectionStringName?.Clean()) ?? throw new ArgumentNullException(nameof(connectionStringName));

        string connectionString =
            (ConfigurationManager.ConnectionStrings[sanitisedConnectionStringName]?.ConnectionString?.Clean())
                ?? throw new ArgumentException($"The connection string for '{connectionStringName}' is empty");

        string tsTableName = string.IsNullOrEmpty(tableName) ?
            PluralizationProvider.Pluralize(typeof(T).Name) :
                tableName;

        TableClientOptions options = new();

        options.Retry.Mode = RetryMode.Exponential;
        options.Retry.Delay = TimeSpan.FromSeconds(2);
        options.Retry.MaxRetries = 10;
        options.Retry.MaxDelay = TimeSpan.FromSeconds(30);

        var serviceClient = new TableServiceClient(connectionString, options);
        Table = serviceClient.GetTableClient(tableName);

        Table.CreateIfNotExists();
    }
}
