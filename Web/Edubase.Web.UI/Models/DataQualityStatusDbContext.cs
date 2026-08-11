using System.Data.Common;
using System.Data.Entity;

namespace Edubase.Web.UI.Models
{
    public class DataQualityStatusDbContext : DbContext
    {
        public DataQualityStatusDbContext(DbConnection connection) : base(connection, true)
        {
        }
        public DbSet<SqlDataQualityStatus> DataQualityStatus { get; set; }
    }
}
