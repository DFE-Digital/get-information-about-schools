using System.Collections.Concurrent;
using System.Globalization;
using Edubase.AcceptanceTests.Repositories;

namespace Edubase.AcceptanceTests.Users
{
    internal sealed class UsersInSqlServer : IUsers
    {
        private readonly AcceptanceTestsDbContext dbContext;
        private readonly IAcceptanceTestLogger logger;
        private static readonly ConcurrentDictionary<decimal, byte> backOfficeUsers = new();

        internal UsersInSqlServer(AcceptanceTestsDbContext dbContext, IAcceptanceTestLogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<User> GetBackOfficeUser()
        {
            foreach (var systemUser in dbContext.SystemUsers.Where(user => user.enabled == 1 && user.wsEnabled == 1))
            {
                if (backOfficeUsers.TryAdd(systemUser.saUserId, 0))
                {
                    logger.LogInformation($"Reserved back-office user {systemUser.saUserId} for testing purposes.");

                    return await CreateUserFromSystemUser(systemUser, "GIAS Backoffice - TESTER ONLY");
                }
            }

            throw new UserNotFoundException("Could not find an unused backoffice user in the database for testing purposes");
        }

        public async Task RemoveUser(User user)
        {
            var userId = decimal.Parse(
                user.AttributeStatementValue,
                CultureInfo.InvariantCulture);

            if (!backOfficeUsers.TryRemove(userId, out _))
            {
                throw new InvalidOperationException($"Back-office user {userId} was not reserved.");
            }
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
