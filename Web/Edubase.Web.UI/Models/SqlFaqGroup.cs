using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Web.UI.Models
{
    [Table("FaqGroups", Schema = "FrontEnd")]
    public class SqlFaqGroup
    {
        public SqlFaqGroup() { }


        [Key, Column(Order = 0)]
        public string PartitionKey { get; set; }
        [Key, Column(Order = 1)]
        public string RowKey { get; set; }
        public string GroupName { get; set; }
        public int DisplayOrder { get; set; }
    }
}
