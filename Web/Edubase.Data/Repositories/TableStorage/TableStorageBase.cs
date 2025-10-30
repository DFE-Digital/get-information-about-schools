using System;
using Azure.Core;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using PluralizeService.Core;

namespace Edubase.Data.Repositories.TableStorage;

public class TableStorageBase<T> where T : class, ITableEntity, new()
{
    public TableClient Table { get; }

    public TableStorageBase(IConfiguration configuration, string connectionStringName, string? tableName = null)
    {
        if (string.IsNullOrWhiteSpace(connectionStringName))
            throw new ArgumentNullException(nameof(connectionStringName));

        string connectionString = configuration.GetConnectionString(connectionStringName)?.Trim()
            ?? throw new ArgumentException($"The connection string for '{connectionStringName}' is missing or empty");

        string tsTableName = string.IsNullOrWhiteSpace(tableName)
            ? PluralizationProvider.Pluralize(typeof(T).Name)
            : tableName;

        var options = new TableClientOptions
        {
            Retry =
            {
                Mode = RetryMode.Exponential,
                Delay = TimeSpan.FromSeconds(2),
                MaxRetries = 10,
                MaxDelay = TimeSpan.FromSeconds(30)
            }
        };

        var serviceClient = new TableServiceClient(connectionString, options);
        Table = serviceClient.GetTableClient(tsTableName);
        Table.CreateIfNotExists();
    }
}
