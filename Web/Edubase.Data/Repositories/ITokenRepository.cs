using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Repositories
{
    public interface ITokenRepository
    {
        Task CreateAsync(IEnumerable<Token> messages);
        Task CreateAsync(params Token[] messages);
        Task CreateAsync(Token message);
        Task DeleteAsync(string id);
        Task<Page<Token>> GetAllAsync(int? take = null, TableContinuationToken skip = null);
        Task<Token> GetAsync(string id);
        Task UpdateAsync(Token item);
        Token Get(string id);
    }
}