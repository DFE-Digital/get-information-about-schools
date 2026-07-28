using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Web.UI.Models
{
    [Table("NotificationTemplates", Schema = "FrontEnd")]
    public class SqlLocalAuthoritySet
    {
        public SqlLocalAuthoritySet() { }

        [Key, Column(Order = 0)]
        public string PartitionKey { get; set; }
        [Key, Column(Order = 1)]
        public string RowKey { get; set; }
        public string Title { get; set; }
        public byte[] IdData { get; set; }
    }
}
