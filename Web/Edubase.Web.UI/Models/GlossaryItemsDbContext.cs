using System.Data.Common;
using System.Data.Entity;


namespace Edubase.Web.UI.Models
{
    public class GlossaryItemsDbContext : DbContext
    {
        public GlossaryItemsDbContext(DbConnection connection) : base(connection, true)
        {

        }
        public DbSet<SqlGlossaryItem> GlossaryItems { get; set; }
    }
}
