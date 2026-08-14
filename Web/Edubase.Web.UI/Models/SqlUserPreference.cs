using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Web.UI.Models
{
    [Table("UserPreferences", Schema = "FrontEnd")]
    public class SqlUserPreference
    {
        public SqlUserPreference() { }

        public SqlUserPreference(string userId)
        {
            PartitionKey = string.Empty;
        }

        [Key, Column(Order = 0)]
        public string PartitionKey { get; set; }
        [Key, Column(Order = 1)]
        public string RowKey { get; set; }
        public string SavedSearchToken { get; set; }
    }
}
