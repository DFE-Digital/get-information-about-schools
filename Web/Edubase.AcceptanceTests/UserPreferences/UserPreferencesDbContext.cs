using Microsoft.EntityFrameworkCore;

namespace Edubase.AcceptanceTests.UserPreferences
{
    public sealed class UserPreferencesDbContext : DbContext
    {
        public UserPreferencesDbContext(DbContextOptions<UserPreferencesDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserPreference> UserPreferences => Set<UserPreference>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.ToTable("UserPreferences", "FrontEnd");
                entity.HasKey(preference => new { preference.PartitionKey, preference.RowKey });
                entity.Property(preference => preference.PartitionKey).HasMaxLength(64).IsUnicode(false);
                entity.Property(preference => preference.RowKey).HasMaxLength(64).IsUnicode(false);
                entity.Property(preference => preference.SavedSearchToken).HasMaxLength(256).IsUnicode(false);
            });
        }
    }

    public sealed class UserPreference
    {
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public string? SavedSearchToken { get; set; }
    }
}
