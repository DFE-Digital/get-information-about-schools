using Edubase.AcceptanceTests.UserPreferences;
using Microsoft.EntityFrameworkCore;

namespace Edubase.AcceptanceTests.Repositories
{
    public sealed class AcceptanceTestsDbContext : DbContext
    {
        public AcceptanceTestsDbContext(DbContextOptions<AcceptanceTestsDbContext> options)
            : base(options)
        {
        }

        public DbSet<FrontEndUserPreference> UserPreferences => Set<FrontEndUserPreference>();
        public DbSet<DboSystemUser> SystemUsers => Set<DboSystemUser>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FrontEndUserPreference>(entity =>
            {
                entity.ToTable("UserPreferences", "FrontEnd");
                entity.HasKey(preference => new { preference.PartitionKey, preference.RowKey });
                entity.Property(preference => preference.PartitionKey).HasMaxLength(64).IsUnicode(false);
                entity.Property(preference => preference.RowKey).HasMaxLength(64).IsUnicode(false);
                entity.Property(preference => preference.SavedSearchToken).HasMaxLength(256).IsUnicode(false);
            });
            modelBuilder.Entity<DboSystemUser>(entity =>
            {
                entity.ToTable("SystemUser", "dbo");
                entity.HasKey(user => new { user.username, user.saUserId });
                entity.Property(user => user.username).HasMaxLength(255).IsUnicode(false);
                entity.Property(user => user.saUserId).HasPrecision(19, 0);
                entity.Property(user => user.enabled).HasColumnName("enabled");
                entity.Property(user => user.UserGroupCode).HasColumnName("UserGroupCode");
                entity.Property(user => user.URN).HasColumnName("URN");
                entity.Property(user => user.UID).HasColumnName("UID");
                entity.Property(user => user.LocalAuthority).HasColumnName("LocalAuthority");
            });
        }
    }
}
