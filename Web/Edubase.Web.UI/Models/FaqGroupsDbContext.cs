using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class FaqGroupsDbContext : DbContext
    {
        public FaqGroupsDbContext(DbConnection connection) : base(connection, true)
        {

        }
        public DbSet<SqlFaqGroup> FaqGroups { get; set; }
    }
}
