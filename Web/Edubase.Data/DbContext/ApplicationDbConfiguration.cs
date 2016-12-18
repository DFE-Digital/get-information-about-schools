using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace Edubase.Data.DbContext
{
    public class ApplicationDbConfiguration : DbConfiguration
    {
        public ApplicationDbConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}
