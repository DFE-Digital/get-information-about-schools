using System.Data.Common;
using System.Data.Entity;

namespace Edubase.Web.UI.Models
{
    public class ApiRecorderSessionItemsDbContext : DbContext
    {
        public ApiRecorderSessionItemsDbContext(DbConnection connection) : base(connection, true)
        {
        }
        public DbSet<SqlApiRecorderSessionItem> ApiRecorderSessionItems { get; set; }
    }
}
