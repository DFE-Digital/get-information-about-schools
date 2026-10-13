using System.Data.Entity.ModelConfiguration;

namespace Edubase.Data.Repositories.EF
{
    public class NotificationBannerConfiguration : EntityTypeConfiguration<SqlNotificationBanner>
    {
        public NotificationBannerConfiguration()
        {
            ToTable("NotificationBanners", "FrontEnd");

            HasKey(x => new
            {
                x.PartitionKey,
                x.RowKey
            });
        }
    }
}
