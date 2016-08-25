using Web.UI.Models;
using System.Data.Entity.Migrations;

namespace Web.UI.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            UserAccountSeeder.Seed(context);
        }
    }
}
