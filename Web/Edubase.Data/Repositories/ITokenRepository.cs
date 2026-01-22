using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories;

public interface ITokenRepository
{
    Task CreateAsync(Token message);
    Task<Token> GetAsync(string id);
}
