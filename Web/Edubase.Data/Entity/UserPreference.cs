using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Entity
{
    public class UserPreference : TableEntity
    {
        public UserPreference()
        {
            
        }

        public UserPreference(string userId)
        {
            PartitionKey = string.Empty;
            UserId = userId;
        }

        [IgnoreProperty]
        public string UserId { get => RowKey; set => RowKey = value; }

        public string SavedSearchToken { get; set; }
    }
}
