namespace Edubase.Data.Migrations
{
    using DbContext;
    using Entity;
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            new DbSeeder().Seed(context);
        }
    }
}
