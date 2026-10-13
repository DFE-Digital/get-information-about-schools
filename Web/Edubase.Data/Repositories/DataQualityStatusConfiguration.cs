using System.Data.Entity.ModelConfiguration;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.EF
{
    public class DataQualityStatusConfiguration : EntityTypeConfiguration<DataQualityStatus>
    {
        public DataQualityStatusConfiguration()
        {
            ToTable("DataQualityStatus", "FrontEnd");

            HasKey(x => new
            {
                x.PartitionKey,
                x.RowKey
            });

            Ignore(x => x.ETag);
            Ignore(x => x.Timestamp);
            Ignore(x => x.EstablishmentType);
        }
    }
}
