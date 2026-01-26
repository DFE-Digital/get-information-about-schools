using System;
using Azure.Core;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using PluralizeService.Core;

namespace Edubase.Data.Repositories.TableStorage;

/// <summary>
/// Provides a strongly-typed base class for working with Azure Table Storage.
/// Automatically resolves the table name, configures retry policies,
/// and ensures the table exists on initialization.
/// </summary>
/// <typeparam name="T">
/// The entity type stored in the table. Must implement <see cref="ITableEntity"/>
/// and have a parameterless constructor.
/// </typeparam>
public class TableStorageBase<T> where T : class, ITableEntity, new()
{
    /// <summary>
    /// Gets the <see cref="TableClient"/> used to perform operations
    /// against the Azure Table Storage table.
    /// </summary>
    public TableClient Table { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableStorageBase{T}"/> class.
    /// Configures the table client, applies retry policies, resolves the table name,
    /// and ensures the table exists.
    /// </summary>
    /// <param name="configuration">
    /// The application configuration used to retrieve the connection string.
    /// </param>
    /// <param name="connectionStringName">
    /// The name of the connection string in configuration. Must not be null or empty.
    /// </param>
    /// <param name="tableName">
    /// Optional explicit table name. If omitted, the table name is derived by pluralizing
    /// the entity type name.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="connectionStringName"/> is null or whitespace.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the connection string cannot be found or is empty.
    /// </exception>
    public TableStorageBase(
        IConfiguration configuration,
        string connectionStringName,
        string? tableName = null)
    {
        if (string.IsNullOrWhiteSpace(connectionStringName))
        {
            throw new ArgumentNullException(nameof(connectionStringName));
        }

        string connectionString = configuration.GetConnectionString(connectionStringName)?.Trim()
            ?? throw new ArgumentException(
                $"The connection string for '{connectionStringName}' is missing or empty");

        // Determine table name: explicit override or pluralized entity type name
        string tsTableName = string.IsNullOrWhiteSpace(tableName)
            ? PluralizationProvider.Pluralize(typeof(T).Name)
            : tableName;

        // Configure retry behavior for transient faults
        TableClientOptions options = new()
        {
            Retry =
            {
                Mode = RetryMode.Exponential,
                Delay = TimeSpan.FromSeconds(2),
                MaxRetries = 10,
                MaxDelay = TimeSpan.FromSeconds(30)
            }
        };

        // Create the service client and table client
        TableServiceClient serviceClient = new(connectionString, options);
        Table = serviceClient.GetTableClient(tsTableName);

        // Ensure the table exists before use
        Table.CreateIfNotExists();
    }
}
