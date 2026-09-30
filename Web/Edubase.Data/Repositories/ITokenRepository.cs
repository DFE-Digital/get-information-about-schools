using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Repositories
{
    public interface ITokenRepository
    {
        Task CreateAsync(Token message);
        Task DeleteAsync(string id);
        Task<Token> GetAsync(string id);
        Task UpdateAsync(Token item);
        Token Get(string id);
    }
}
