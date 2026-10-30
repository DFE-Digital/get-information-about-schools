using System.Data.Common;
using System.Data.Entity;

namespace Edubase.Web.UI.Models
{
    public class TokensDbContext : DbContext
    {
        public TokensDbContext(DbConnection connection) : base(connection, true)
        {
        }
        public DbSet<SqlToken> Tokens { get; set; }
    }
}
