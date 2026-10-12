using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models
{
    [Table("vwLocalAuthorityGSSCode", Schema = "FrontEnd")]
    public class SqlLaNameCode
    {
        [Key, Column("GroupCode", Order = 0)]
        public string GroupCode { get; set; }

        [Key, Column("LaCode", Order = 1)]
        public string LaCode { get; set; }

        [Column("LocalAuthorityName")]
        public string LaName { get; set; }

        [Column("GSSCode")]
        public string GsLaCode { get; set; }
    }
}
