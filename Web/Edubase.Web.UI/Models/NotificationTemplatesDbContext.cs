using System.Data.Common;
using System.Data.Entity;

namespace Edubase.Web.UI.Models
{
    public class NotificationTemplatesDbContext : DbContext
    {
        public NotificationTemplatesDbContext(DbConnection connection) : base(connection, true)
        {
        }
        public DbSet<SqlNotificationTemplate> NotificationTemplates { get; set;}
    }
}
