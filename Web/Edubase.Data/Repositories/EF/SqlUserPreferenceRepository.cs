using Edubase.Data.Entity;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories.EF
{
    public class SqlUserPreferenceRepository : IUserPreferenceRepository
    {
        private readonly FrontEndDbContext _context;

        public SqlUserPreferenceRepository(FrontEndDbContext context)
        {
            _context = context;
        }

        public async Task<UserPreference> GetAsync(string userId)
        {
            return await _context.UserPreferences.FindAsync(string.Empty, userId);           
        }

        public async Task UpsertAsync(UserPreference item)
        {
            var existing = await _context.UserPreferences.FindAsync(item.PartitionKey, item.RowKey);

            if (existing == null)
            {
                _context.UserPreferences.Add(item);
            }
            else
            {
                existing.SavedSearchToken = item.SavedSearchToken;
            }

            await _context.SaveChangesAsync();
        }      
    }
}
