using System.Data.Common;
using System.Data.Entity;

namespace Edubase.Web.UI.Models
{
    public class UserPreferencesDbContext : DbContext
    {
        public UserPreferencesDbContext(DbConnection connection) : base(connection, true)
        {
        }
        public DbSet<SqlUserPreference> UserPreferences { get; set;}
    }
}
