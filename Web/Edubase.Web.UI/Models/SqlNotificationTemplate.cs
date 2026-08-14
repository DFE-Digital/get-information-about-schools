using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Web.UI.Models
{
    [Table("NotificationTemplates", Schema = "FrontEnd")]
    public class SqlNotificationTemplate
    {
        public SqlNotificationTemplate() { }

        [Key, Column(Order = 0)]
        public string PartitionKey { get; set; }
        [Key, Column(Order = 1)]
        public string RowKey { get; set; }
        public string Content { get; set; }
    }
}
