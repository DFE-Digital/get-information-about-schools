using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.EF
{
    public class NotificationBannerConfiguration : EntityTypeConfiguration<NotificationBanner>
    {
        public NotificationBannerConfiguration()
        {
            ToTable("NotificationBanners", "FrontEnd");

            HasKey(x => new
            {
                x.PartitionKey,
                x.RowKey
            });

            Ignore(x => x.ETag);
            Ignore(x => x.Timestamp);
            Ignore(x => x.LinkUrl1);
            Ignore(x => x.LinkText1);
            Ignore(x => x.LinkUrl2);
            Ignore(x => x.LinkText2);
            Property(x => x.SqlTimestamp)
                .HasColumnName("Timestamp")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
        }
    }
}
