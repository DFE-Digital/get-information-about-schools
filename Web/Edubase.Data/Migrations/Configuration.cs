namespace Edubase.Data.Migrations
{
    using Entity;
    using Entity.Permissions;
    using Identity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            UserAccountSeeder.Seed(context);
            using (var dc = new ApplicationDbContext())
            {
                // Seed permissions data on the Establishment properties
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


                //// Seed Governor roles
                //new[]
                //{
                //    "Accounting Officer","Chair of Governors",
                //    "Chair of Local Governing Body",
                //    "Chair of Trustees", "Chief Financial Officer",
                //    "Governor", "Local Governor", "Member", "Trustee"
                //}.ForEach(x =>
                //{
                //    if(!dc.LookupGovernorRoles.Any(r => r.Name == x))
                //        dc.LookupGovernorRoles.Add(new Entity.Lookups.LookupGovernorRole { Name = x });
                //});
                //dc.SaveChanges();

                //// Seed Governor Appointing Bodies
                //new[]
                //{
                //    "Any additional members appointed by the members of the academy trust",
                //    "Appointed by academy members",
                //    "Appointed by foundation/Trust",
                //    "Appointed by GB/board",
                //    "Elected by parents",
                //    "Elected by school staff",
                //    "Ex-officio by virtue of office as headteacher/principal",
                //    "Ex-officio foundation governor (appointed by foundation by virtue of the office they hold)",
                //    "Foundation/sponsor members",
                //    "N/A",
                //    "Nominated by LA and appointed by GB",
                //    "Nominated by other body and appointed by GB",
                //    "Original (signatory) members",
                //    "Parent appointed by GB/board due to no election candidates",
                //    "Persons who are appointed by the foundation body or sponsor (if applicable)"
                //}.ForEach(x =>
                //{
                //    if(!dc.LookupGovernorAppointingBodies.Any(r => r.Name == x))
                //        dc.LookupGovernorAppointingBodies.Add(new Entity.Lookups.LookupGovernorAppointingBody { Name = x });
                //});
                //dc.SaveChanges();

            }
        }
    }
}
