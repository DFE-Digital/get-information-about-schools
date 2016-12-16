using Edubase.Data.Entity;
using Edubase.Data.Entity.Permissions;
using Edubase.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using MoreLinq;
using Edubase.Data.DbContext;

namespace Edubase.Data.Migrations
{
    public class DbSeeder
    {
        public void Seed(ApplicationDbContext context)
        {
            // Seed permissions data on the Establishment properties
            //var cols = PermissionUtil.GetRestrictiveColumns<Establishment>(context);

            //var permissions = cols.SelectMany(x =>
            //    Roles.RestrictiveRoles.Select(role => new EstablishmentPermission
            //    {
            //        PropertyName = x.Name,
            //        RoleName = role,
            //        AllowApproval = PermissionUtil.AllowApproval(role, x.Attribute),
            //        AllowUpdate = PermissionUtil.AllowUpdate(role, x.Attribute)
            //    })).ToArray();

            //context.Permissions.AddOrUpdate(permissions);

            if (!context.LookupEstablishmentLinkTypes.Any(x => x.Name.Equals("Successor")))
            {
                context.LookupEstablishmentLinkTypes.Add(new Entity.Lookups.LookupEstablishmentLinkType
                {
                    Name = "Successor"
                });
            }

            if (!context.LookupEstablishmentLinkTypes.Any(x => x.Name.Equals("Predecessor")))
            {
                context.LookupEstablishmentLinkTypes.Add(new Entity.Lookups.LookupEstablishmentLinkType
                {
                    Name = "Predecessor"
                });
            }

            context.SaveChanges();


            if(!context.Groups.Any(x => x.EstablishmentCount > 0))
            {
                var sql = $@"UPDATE G
                             SET G.{nameof(GroupCollection.EstablishmentCount)} = 
                                            (SELECT COUNT(1) 
                                             FROM {nameof(EstablishmentGroup)} EG 
                                             WHERE EG.{nameof(EstablishmentGroup.GroupUID)} = G.{nameof(GroupCollection.GroupUID)} 
                                             AND EG.IsDeleted = 0)
                             FROM {nameof(GroupCollection)} G
                             WHERE G.IsDeleted = 0";

                context.Database.ExecuteSqlCommand(sql);
            }

            var commands = new[] { "ALTER DATABASE CURRENT SET CHANGE_TRACKING = ON (CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON)" }
            .Concat(new[] { "Establishment", "Governor", "GroupCollection" }
                .Select(x => $@"ALTER TABLE {x} ENABLE CHANGE_TRACKING WITH(TRACK_COLUMNS_UPDATED = ON)"));
            commands.ForEach(x => Common.Invoker.IgnoringException(() => context.Database.ExecuteSqlCommand(x)));
        }
    }
}
