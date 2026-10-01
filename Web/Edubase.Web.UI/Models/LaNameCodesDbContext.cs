using System.Data.Common;
using System.Data.Entity;

namespace Edubase.Web.UI.Models
{
    public class LaNameCodesDbContext : DbContext
    {
        public LaNameCodesDbContext(DbConnection connection) : base(connection, true)
        {
        }
        public DbSet<SqlLaNameCode> LaNameCodes { get; set; }
    }
}
