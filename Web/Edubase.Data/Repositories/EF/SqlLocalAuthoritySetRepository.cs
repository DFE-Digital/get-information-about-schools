using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories.EF
{
    public class SqlLocalAuthoritySetRepository : ILocalAuthoritySetRepository
    {
        private readonly FrontEndDbContext _context;

        public SqlLocalAuthoritySetRepository(FrontEndDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(LocalAuthoritySet message)
        {
            _context.LocalAuthoritySets.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(params LocalAuthoritySet[] messages)
        {
            _context.LocalAuthoritySets.AddRange(messages);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(IEnumerable<LocalAuthoritySet> messages)
        {
            await CreateAsync(messages.ToArray());
        }

        public async Task<Page<LocalAuthoritySet>> GetAllAsync(
            int? take = null, TableContinuationToken skip = null)
        {
            IQueryable<LocalAuthoritySet> query = _context.LocalAuthoritySets;
            if (take.HasValue) query = query.Take(take.Value);

            var results = await query.ToListAsync();
            return new Page<LocalAuthoritySet>(results, null);
        }

        public async Task<LocalAuthoritySet> GetAsync(string id)
        {
            return await _context.LocalAuthoritySets.FindAsync(string.Empty, id);
        }

        public async Task UpdateAsync(LocalAuthoritySet item)
        {
            var existing = await _context.LocalAuthoritySets.FindAsync(item.PartitionKey, item.RowKey);

            if (existing == null)
            {
                _context.LocalAuthoritySets.Add(item);
            }
            else
            {
                existing.Title = item.Title;
                existing.IdData = item.IdData;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await _context.LocalAuthoritySets.FindAsync(string.Empty, id);

            if (existing != null)
            {
                _context.LocalAuthoritySets.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
    }
}
