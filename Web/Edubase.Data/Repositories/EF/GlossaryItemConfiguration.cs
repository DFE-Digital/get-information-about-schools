using System.Data.Entity.ModelConfiguration;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.EF
{
    public class GlossaryItemConfiguration : EntityTypeConfiguration<GlossaryItem>
    {
        public GlossaryItemConfiguration()
        {
            ToTable("GlossaryItems", "FrontEnd");

            HasKey(x => new
            {
                x.PartitionKey,
                x.RowKey
            });

            Ignore(x => x.ETag);
            Ignore(x => x.Timestamp);
        }
    }
}
