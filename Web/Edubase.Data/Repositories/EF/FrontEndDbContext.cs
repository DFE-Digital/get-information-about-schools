using System.Data.Common;
using System.Data.Entity;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.EF
{
    public class FrontEndDbContext : DbContext
    {
        public FrontEndDbContext(DbConnection connection) : base(connection, true)
        {
            Database.SetInitializer<FrontEndDbContext>(null);
        }

        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<LocalAuthoritySet> LocalAuthoritySets { get; set; }
        public DbSet<GlossaryItem> GlossaryItems { get; set; }
        public DbSet<FaqGroup> FaqGroups { get; set; }
        public DbSet<FaqItem> FaqItems { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserPreferenceConfiguration());
            modelBuilder.Configurations.Add(new TokenConfiguration());
            modelBuilder.Configurations.Add(new LocalAuthoritySetConfiguration());
            modelBuilder.Configurations.Add(new GlossaryItemConfiguration());
            modelBuilder.Configurations.Add(new FaqGroupConfiguration());
            modelBuilder.Configurations.Add(new FaqItemConfiguration());
            modelBuilder.Configurations.Add(new NotificationTemplateConfiguration());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
