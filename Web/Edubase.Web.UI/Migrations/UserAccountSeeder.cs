//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Edubase.Web.UI.Models;
//using Edubase.Data.Entity;

//namespace Edubase.Web.UI.Migrations
//{
//    public static class UserAccountSeeder
//    {
//        public static void Seed(DbContext context)
//        {
//            var roles = SeedRoles(context);
//            SeedAdminUser(context, roles);
//        }

//        private static IEnumerable<IdentityRole> SeedRoles(DbContext context)
//        {
//            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context));

//            yield return GetOrCreateRole(roleManager, "Admin");
//            yield return GetOrCreateRole(roleManager, "AccessAllSchools");
//        }

//        private static IdentityRole GetOrCreateRole(ApplicationRoleManager roleManager, string roleName)
//        {
//            var role = roleManager.FindByName(roleName);
//            if (role != null)
//            {
//                return role;
//            }

//            role = new IdentityRole { Name = roleName };
//            roleManager.Create(role);

//            return role;
//        }

//        private static void SeedAdminUser(DbContext context, IEnumerable<IdentityRole> roles)
//        {
//            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

//            var adminUser = userManager.FindByName("admin");
//            if (adminUser == null)
//            {
//                adminUser = new ApplicationUser
//                {
//                    UserName = "admin",
//                    Email = "admin@edubase.user",
//                    EmailConfirmed = true
//                };

//                userManager.Create(adminUser, "Dfe34500!");
//            }

//            EnsureInRole(roles, adminUser, "Admin");
//            EnsureInRole(roles, adminUser, "AccessAllSchools");
//            userManager.Update(adminUser);
//        }

//        private static void EnsureInRole(IEnumerable<IdentityRole> roles, ApplicationUser adminUser, string roleName)
//        {
//            var adminRoleId = roles.First(r => r.Name == roleName).Id;
//            var existing = adminUser.Roles.FirstOrDefault(r => r.RoleId == adminRoleId);
//            if (existing == null)
//            {
//                adminUser.Roles.Add(new IdentityUserRole { RoleId = adminRoleId, UserId = adminUser.Id });
//            }
//        }
//    }
//}