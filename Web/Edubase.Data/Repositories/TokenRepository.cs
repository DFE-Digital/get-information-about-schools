using System;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using MoreLinq;

namespace Edubase.Data.Repositories;

public class TokenRepository : TableStorageBase<Token>, ITokenRepository
{
    public TokenRepository()
        : base("DataConnectionString")
    {
    }

    public async Task CreateAsync(Token message) => await Table.AddEntityAsync(message);

    
    public async Task<Token> GetAsync(string id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(
            value: id.Length,
            other: 5);

        try
        {
            var response = await Table.GetEntityAsync<Token>(partitionKey: id.Substring(0, 4), rowKey: id.Substring(4));
            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }
}
