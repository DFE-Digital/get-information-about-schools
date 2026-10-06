using System.Data.Entity.ModelConfiguration;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.EF
{
    public class LocalAuthoritySetConfiguration : EntityTypeConfiguration<LocalAuthoritySet>
    {
        public LocalAuthoritySetConfiguration()
        {
            ToTable("LocalAuthoritySets", "FrontEnd");

            HasKey(x => new
            {
                x.PartitionKey,
                x.RowKey
            });

            Ignore(x => x.Ids);
            Ignore(x => x.ETag);
            Ignore(x => x.Timestamp);
        }
    }
}
