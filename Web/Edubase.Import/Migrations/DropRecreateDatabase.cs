using Edubase.Common;
using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using Edubase.Data.Migrations;
using System.Data.Entity;

namespace Edubase.Import.Migrations
{
    public class DropRecreateDatabase : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        // DON'T SEED UNTIL AFTER IMPORT
        //protected override void Seed(ApplicationDbContext context) => new DbSeeder().Seed(context);

        public override void InitializeDatabase(ApplicationDbContext context)
        {
            Invoker.IgnoringException(() => context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction
            , string.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", context.Database.Connection.Database)));
            base.InitializeDatabase(context);
        }
    }
}
