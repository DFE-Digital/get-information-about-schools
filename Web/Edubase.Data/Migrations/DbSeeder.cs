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

            context.Database.ExecuteSqlCommand("DELETE FROM LookupEstablishmentType WHERE Code IN ('22', '17', '9', '98', '23')"); // Story: 7714

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

            var commands = new[] { "ALTER DATABASE CURRENT SET CHANGE_TRACKING = ON (CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON)",
                @"SET IDENTITY_INSERT dbo.LookupGovernorRole ON; 
                  IF NOT EXISTS(SELECT * FROM dbo.LookupGovernorRole WHERE Id = 10) 
                  BEGIN 
                      INSERT INTO dbo.LookupGovernorRole (Id, Name, IsDeleted, CreatedUtc, LastUpdatedUtc) VALUES (10, 'Shared Local Governor', 0, GETUTCDATE(), GETUTCDATE()) 
                  END; 
                  SET IDENTITY_INSERT dbo.LookupGovernorRole OFF;",
                @"SET IDENTITY_INSERT dbo.LookupGovernorRole ON; 
                  IF NOT EXISTS(SELECT * FROM dbo.LookupGovernorRole WHERE Id = 11) 
                  BEGIN 
                      INSERT INTO dbo.LookupGovernorRole (Id, Name, IsDeleted, CreatedUtc, LastUpdatedUtc) VALUES (11, 'Shared Chair of Local Governing Body', 0, GETUTCDATE(), GETUTCDATE()) 
                  END; 
                  SET IDENTITY_INSERT dbo.LookupGovernorRole OFF;"}
            .Concat(new[] { "Establishment", "Governor", "GroupCollection" }
                .Select(x => $@"ALTER TABLE {x} ENABLE CHANGE_TRACKING WITH(TRACK_COLUMNS_UPDATED = ON)"));
            commands.ForEach(x => Common.Invoker.IgnoringException(() => context.Database.ExecuteSqlCommand(x)));


            Common.Invoker.IgnoringException(() => context.Database.ExecuteSqlCommand(StackExchange.Profiling.Storage.SqlServerStorage.TableCreationScript));

        }
    }
}
