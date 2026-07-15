using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Web.UI.Models
{
    [Table("UserPreferences")]
    public class SqlUserPreference
    {
        public SqlUserPreference() { }

        public SqlUserPreference(string userId)
        {
                UserId =  userId;
        }

        [Key]
        public string UserId { get; set; }
        public string SavedSearchToken { get; set; }
    }
}
