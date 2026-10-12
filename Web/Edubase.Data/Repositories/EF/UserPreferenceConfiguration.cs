using System.Data.Entity.ModelConfiguration;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.EF
{
    /// <summary>
    ///  EF configuration class including ignoring table storage fields, so as to minimise change to existing model
    ///  classes for migration to SQL storage. Can be revisited after migration
    /// </summary>
    public class UserPreferenceConfiguration : EntityTypeConfiguration<UserPreference>
    {
        public UserPreferenceConfiguration()
        {
            ToTable("UserPreferences", "FrontEnd");

            HasKey(x => new
            {
                x.PartitionKey,
                x.RowKey
            });

            Ignore(x => x.UserId);
            Ignore(x => x.ETag);
            Ignore(x => x.Timestamp);
        }
    }
}
