using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class NotificationBannersDbContext : DbContext
    {
        public NotificationBannersDbContext(DbConnection connection) : base(connection, true)
        {
        }
        public DbSet<SqlNotificationBanner> NotificationBanners { get; set; }
    }
}
