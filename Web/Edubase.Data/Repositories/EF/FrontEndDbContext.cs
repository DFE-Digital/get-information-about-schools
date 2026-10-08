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
        public DbSet<SqlNewsArticle> NewsArticles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserPreferenceConfiguration());
            modelBuilder.Configurations.Add(new TokenConfiguration());
            modelBuilder.Configurations.Add(new LocalAuthoritySetConfiguration());
            modelBuilder.Configurations.Add(new NewsArticleConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
