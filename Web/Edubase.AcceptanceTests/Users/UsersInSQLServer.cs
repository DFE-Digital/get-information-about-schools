using Edubase.AcceptanceTests.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Edubase.AcceptanceTests.Users
{
    internal sealed class UsersInSqlServer : IUsers
    {
        private readonly string environment;
        private readonly AcceptanceTestsDbContext dbContext;

        internal UsersInSqlServer(string environment, AcceptanceTestsDbContext dbContext)
        {
            this.environment = environment;
            this.dbContext = dbContext;
        }

        public async Task<User> CreateBackOfficeUser()
        {
            try
            {
                var sourceSaUserId = GetSourceSaUserId(environment);

                var sourceUser = await dbContext.SystemUsers.SingleAsync(user =>
                    user.enabled == 1 && user.wsEnabled == 1 && user.saUserId == sourceSaUserId);

                return await CreateUserFromSystemUser(sourceUser);
            }
            catch (InvalidOperationException e)
            {
                if (e.Message.Contains("Sequence contains no elements"))
                {
                    throw new UserNotFoundException("Expected user is not in the database for testing purposes");
                }

                throw;
            }
        }

        public async Task DeleteUser(User user)
        {
            var createdUser = await dbContext.SystemUsers.SingleAsync(u => u.saUserId.ToString() == user.AttributeStatementValue);

            dbContext.SystemUsers.Remove(createdUser);

            await dbContext.SaveChangesAsync();
        }

        private async Task<User> CreateUserFromSystemUser(DboSystemUser systemUser)
        {
            var uniqueId = DateTime.Now.Ticks;

            var newSystemUser = new DboSystemUser
            {
                username = $"Acceptance Test User {uniqueId}",
                saUserId = uniqueId,
                wsEnabled = systemUser.wsEnabled,
                enabled = systemUser.enabled,
                UserGroupCode = systemUser.UserGroupCode,
                URN = systemUser.URN,
                UID = systemUser.UID,
                LocalAuthority = systemUser.LocalAuthority
            };

            await dbContext.SystemUsers.AddAsync(newSystemUser);
            await dbContext.SaveChangesAsync();

            return new User
            {
                NameId = newSystemUser.username,
                AttributeStatementValue = newSystemUser.saUserId.ToString(),
                Comment = "GIAS Backoffice - TESTER ONLY"
            };
        }

        private static long GetSourceSaUserId(string environment) => environment.ToLowerInvariant() switch
        {
            "test" => 3600264L,
            "dev" or "sandbox1" or "sandbox2" => 3600154L,
            _ => throw new ArgumentException($"User cloning is not supported for environment '{environment}'.", nameof(environment))
        };
    }
}
