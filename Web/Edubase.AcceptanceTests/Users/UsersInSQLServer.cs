using Edubase.AcceptanceTests.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Edubase.AcceptanceTests.Users
{
    internal sealed class UsersInSqlServer : IUsers
    {
        private readonly AcceptanceTestsDbContext dbContext;

        private readonly Dictionary<decimal, DboSystemUser> backOfficeUsers = new();

        internal UsersInSqlServer(AcceptanceTestsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<User> GetBackOfficeUser()
        {
            foreach (var systemUser in dbContext.SystemUsers.Where(user => user.enabled == 1 && user.wsEnabled == 1))
            {
                if (!backOfficeUsers.ContainsKey(systemUser.saUserId))
                {
                    backOfficeUsers.Add(systemUser.saUserId, systemUser);

                    return await CreateUserFromSystemUser(systemUser,"GIAS Backoffice - TESTER ONLY");
                }
            }

            throw new UserNotFoundException("Could not find an unused backoffice user in the database for testing purposes");
        }

        public async Task RemoveUser(User user)
        {
            var backOfficeUser = backOfficeUsers.Single(u => u.Value.saUserId.ToString() == user.AttributeStatementValue);

            backOfficeUsers.Remove(backOfficeUser.Key);
        }

        private async Task<User> CreateUserFromSystemUser(DboSystemUser systemUser, string comment)
        {
            return new User
            {
                NameId = systemUser.username,
                AttributeStatementValue = systemUser.saUserId.ToString(),
                Comment = comment
            };
        }
    }
}
