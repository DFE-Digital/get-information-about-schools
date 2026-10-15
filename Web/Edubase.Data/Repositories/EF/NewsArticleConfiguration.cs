using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.EF
{
    public class NewsArticleConfiguration : EntityTypeConfiguration<NewsArticle>
    {
        public NewsArticleConfiguration()
        {
            ToTable("NewsArticles", "FrontEnd");

            HasKey(x => new
            {
                x.PartitionKey,
                x.RowKey
            });

            Ignore(x => x.ETag);
            Ignore(x => x.Timestamp);
            Property(x => x.SqlTimestamp)
                .HasColumnName("Timestamp")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
        }
    }
}
