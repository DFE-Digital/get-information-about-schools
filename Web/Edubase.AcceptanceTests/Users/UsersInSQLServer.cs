using Edubase.AcceptanceTests.Repositories;
using Edubase.AcceptanceTests.UserPreferences;
using Microsoft.EntityFrameworkCore;

namespace Edubase.AcceptanceTests.Users
{
    public sealed class UsersInSqlServer : IUsers
    {
        private readonly string environment;
        private readonly AcceptanceTestsDbContext dbContext;

        public UsersInSqlServer(string environment, AcceptanceTestsDbContext dbContext)
        {
            this.environment = environment;
            this.dbContext = dbContext;
        }

        public async Task<User> CreateBackOfficeUser()
        {
            try
            {
                if (environment == "test")
                {
                    var testUser = await dbContext.SystemUsers.SingleAsync(user => user.wsEnabled == 1 && user.saUserId == 3600264);

                    return await CreateUserFromSystemUser(testUser);
                }

                var devUser = await dbContext.SystemUsers.SingleAsync(user => user.wsEnabled == 1 && user.saUserId == 3600154);

                return await CreateUserFromSystemUser(devUser);
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

        private async Task<User> CreateUserFromSystemUser(DboSystemUser systemUser)
        {
            var uniqueId = DateTime.Now.Ticks;

            var newSystemUser = new DboSystemUser
            {
                username = $"Acceptance Test User {uniqueId}",
                saUserId = uniqueId,
                wsEnabled = systemUser.wsEnabled,
                UserGroupCode = systemUser.UserGroupCode
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
    }
}
