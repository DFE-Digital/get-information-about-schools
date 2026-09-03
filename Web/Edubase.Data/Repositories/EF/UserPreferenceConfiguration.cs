using System.Data.Entity.ModelConfiguration;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.EF
{
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
