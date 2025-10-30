using System;
using System.Threading.Tasks;
using Azure;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.Extensions.Configuration;

namespace Edubase.Data.Repositories;

public class TokenRepository : TableStorageBase<Token>, ITokenRepository
{
    public TokenRepository(IConfiguration configuration)
        : base(configuration, "DataConnectionString", "Tokens")
    {
    }

    public async Task CreateAsync(Token message)
    {
        if (string.IsNullOrWhiteSpace(message.PartitionKey) || string.IsNullOrWhiteSpace(message.RowKey))
        {
            throw new ArgumentException("PartitionKey and RowKey must be set on the Token entity.");
        }

        await Table.AddEntityAsync(message);
    }

    public async Task<Token?> GetAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Length < 5)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Token ID must be at least 5 characters long.");
        }

        var partitionKey = id.Substring(0, 4);
        var rowKey = id.Substring(4);

        try
        {
            var response = await Table.GetEntityAsync<Token>(partitionKey, rowKey);
            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }
}
