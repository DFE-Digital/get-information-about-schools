using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Edubase.Data.Entity;
using Edubase.Data.Identity;

namespace Edubase.Data.Migrations
{
    public static class UserAccountSeeder
    {
        public static void Seed(DbContext context)
        {
            var roles = SeedRoles(context);
            SeedAdminUser(context, roles);
            SeedOtherUsers(context, roles);
        }

        private static IEnumerable<IdentityRole> SeedRoles(DbContext context)
        {
            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context));

            yield return GetOrCreateRole(roleManager, "Admin");
            yield return GetOrCreateRole(roleManager, "AccessAllSchools");
        }

        private static IdentityRole GetOrCreateRole(ApplicationRoleManager roleManager, string roleName)
        {
            var role = roleManager.FindByName(roleName);
            if (role != null)
            {
                return role;
            }

            role = new IdentityRole { Name = roleName };
            roleManager.Create(role);

            return role;
        }

        private static void SeedAdminUser(DbContext context, IEnumerable<IdentityRole> roles)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var adminUser = userManager.FindByName("admin");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@edubase.user",
                    EmailConfirmed = true
                };

                userManager.Create(adminUser, "Dfe34500!");
            }

            EnsureInRole(roles, adminUser, "Admin");
            EnsureInRole(roles, adminUser, "AccessAllSchools");
            userManager.Update(adminUser);
        }

        private static void SeedOtherUsers(DbContext context, IEnumerable<IdentityRole> roles)
        {
            SeedOtherUser(context, "academy.user", null, roles, new IdentityUserClaim { ClaimType = "AccessibleSchools", ClaimValue = "137083" });
            SeedOtherUser(context, "la.maintained.sch.user", null, roles, new IdentityUserClaim { ClaimType = "AccessibleSchools", ClaimValue = "121662" });
            SeedOtherUser(context, "free.school.user", null, roles, new IdentityUserClaim { ClaimType = "AccessibleSchools", ClaimValue = "141011" });
            SeedOtherUser(context, "mat.admin.user", null, roles, new IdentityUserClaim { ClaimType = "MATAdmin", ClaimValue = "2046" });
            SeedOtherUser(context, "la.admin.user", null, roles, new IdentityUserClaim { ClaimType = "LAAdmin", ClaimValue = "816" });
            SeedOtherUser(context, "backoffice.user", new[] { "Admin" }, roles);
        }

        private static void SeedOtherUser(DbContext context, string userName, string[] userRoles, IEnumerable<IdentityRole> fullRolesList, IdentityUserClaim claim = null)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var usr = userManager.FindByName(userName);
            if (usr == null)
            {
                usr = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName + "@contentsupport.co.uk",
                    EmailConfirmed = true
                };
                userManager.Create(usr, "TestTest123!");
            }
            
            if (userRoles != null) userRoles.ToList().ForEach(x => EnsureInRole(fullRolesList, usr, x));

            if (claim != null)
            {
                var existing = usr.Claims.FirstOrDefault(x => x.ClaimType == claim.ClaimType);
                if (existing == null)
                {
                    claim.UserId = usr.Id;
                    usr.Claims.Add(claim);
                }
            }

            userManager.Update(usr);
        }

        private static void EnsureInRole(IEnumerable<IdentityRole> roles, ApplicationUser adminUser, string roleName)
        {
            var adminRoleId = roles.First(r => r.Name == roleName).Id;
            var existing = adminUser.Roles.FirstOrDefault(r => r.RoleId == adminRoleId);
            if (existing == null)
            {
                adminUser.Roles.Add(new IdentityUserRole { RoleId = adminRoleId, UserId = adminUser.Id });
            }
        }
    }
}