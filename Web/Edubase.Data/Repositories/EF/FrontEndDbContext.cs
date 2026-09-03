using System.Data.Common;
using System.Data.Entity;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.EF;

namespace Edubase.Web.UI.Models
{
    public class FrontEndDbContext : DbContext
    {               
        public FrontEndDbContext(DbConnection connection) : base(connection, true)
        {
            Database.SetInitializer<FrontEndDbContext>(null);
        }

        public DbSet<UserPreference> UserPreferences { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserPreferenceConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
