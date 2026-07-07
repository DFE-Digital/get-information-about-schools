namespace Edubase.Web.UI.Models
{
    public class SqlUserPreference
    {
        public SqlUserPreference() { }

        public SqlUserPreference(string userId)
        {
            UserId =  userId;
        }

        public string UserId { get; set; }
        public string SavedSearchToken { get; set; }
    }
}
