using System.Data.Entity.ModelConfiguration;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.EF
{
    public class FaqItemConfiguration : EntityTypeConfiguration<FaqItem>
    {
        public FaqItemConfiguration()
        {
            ToTable("FaqItems", "FrontEnd");
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
