using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Web.UI.Models
{
    [Table("DataQualityStatus", Schema = "FrontEnd")]
    public class SqlDataQualityStatus
    {
        public SqlDataQualityStatus() { }

        [Key, Column(Order = 0)]
        public string PartitionKey { get; set; }
        [Key, Column(Order = 1)]
        public string RowKey { get; set; }
        public DateTime LastUpdated { get; set; }
        public string DataOwner { get; set; }
        public string Email { get; set; }
    }
}
