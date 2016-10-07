namespace Edubase.Data.Migrations
{
    using Entity;
    using Entity.ComplexTypes;
    using Entity.Permissions;
    using Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity.Migrations;
    using System.Diagnostics;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            UserAccountSeeder.Seed(context);
            using (var dc = new ApplicationDbContext())
            {
                var cols = PermissionUtil.GetRestrictiveColumns<Establishment>(dc);

                var permissions = cols.SelectMany(x =>
                    Roles.RestrictiveRoles.Select(role => new EstablishmentPermission
                    {
                        PropertyName = x.Name,
                        RoleName = role,
                        AllowApproval = PermissionUtil.AllowApproval(role, x.Attribute),
                        AllowUpdate = PermissionUtil.AllowUpdate(role, x.Attribute)
                    })).ToArray();

                dc.Permissions.AddOrUpdate(permissions);
                dc.SaveChanges();
            }
        }
    }
}
