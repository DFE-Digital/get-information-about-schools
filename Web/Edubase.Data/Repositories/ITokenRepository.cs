using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories;
public interface ITokenRepository
{
    Task CreateAsync(Token message);
    Task DeleteAsync(string id);
    Task<Token> GetAsync(string id);
    Task UpdateAsync(Token item);
    Token Get(string id);
}
