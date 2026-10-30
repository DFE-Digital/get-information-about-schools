using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Web.UI.Models
{
    [Table("FaqItems", Schema = "Frontend")]
    public class SqlFaqItem
    {
        public SqlFaqItem() { }

        [Key, Column(Order = 0)]
        public string PartitionKey { get; set; }
        [Key, Column(Order = 1)]
        public string RowKey { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int DisplayOrder { get; set; }
        public string GroupId { get; set; }
    }
}
