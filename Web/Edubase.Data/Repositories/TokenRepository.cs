using System;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories;

public class TokenRepository : ITokenRepository
{
    private const string TableNameKey = "Tokens";

    private readonly TableClient _tokenRepositoryTableClient;

    public TokenRepository(TableServiceClient tableServiceClient)
    {
        _tokenRepositoryTableClient = tableServiceClient.GetTableClient(TableNameKey);
    }

    public async Task CreateAsync(Token message)
    {
        if (string.IsNullOrWhiteSpace(message.PartitionKey) || string.IsNullOrWhiteSpace(message.RowKey))
        {
            throw new ArgumentException("PartitionKey and RowKey must be set on the Token entity.");
        }

        await _tokenRepositoryTableClient.AddEntityAsync(message);
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
            var response = await _tokenRepositoryTableClient.GetEntityAsync<Token>(partitionKey, rowKey);
            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }
}
