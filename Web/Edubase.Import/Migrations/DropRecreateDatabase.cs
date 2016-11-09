using Edubase.Data.Entity;
using Edubase.Data.Migrations;
using System.Data.Entity;

namespace Edubase.Import.Migrations
{
    public class DropRecreateDatabase : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context) => new DbSeeder().Seed(context);
    }
}
