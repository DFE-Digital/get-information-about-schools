using Edubase.Data.Entity;
using Edubase.Data.Entity.Permissions;
using Edubase.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;

namespace Edubase.Data.Migrations
{
    public class DbSeeder
    {
        public void Seed(ApplicationDbContext context)
        {
            UserAccountSeeder.Seed(context);
            // Seed permissions data on the Establishment properties
            var cols = PermissionUtil.GetRestrictiveColumns<Establishment>(context);

            var permissions = cols.SelectMany(x =>
                Roles.RestrictiveRoles.Select(role => new EstablishmentPermission
                {
                    PropertyName = x.Name,
                    RoleName = role,
                    AllowApproval = PermissionUtil.AllowApproval(role, x.Attribute),
                    AllowUpdate = PermissionUtil.AllowUpdate(role, x.Attribute)
                })).ToArray();

            context.Permissions.AddOrUpdate(permissions);
            context.SaveChanges();
            
        }
    }
}
