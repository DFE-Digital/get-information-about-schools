using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Repositories.EF
{
    public class SqlFaqGroupRepository
    {
        private readonly FrontEndDbContext _context;

        public SqlFaqGroupRepository(FrontEndDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(FaqGroup entity)
        {
            _context.FaqGroups.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(params FaqGroup[] entities)
        {
            _context.FaqGroups.AddRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(IEnumerable<FaqGroup> entities)
        {
            await CreateAsync(entities.ToArray());
        }

        public async Task<Page<FaqGroup>> GetAllAsync(
            int take, TableContinuationToken skip = null)
        {
            var results = await _context.FaqGroups.Take(take).ToListAsync();
            return new Page<FaqGroup>(results, null);
        }

        public async Task<FaqGroup> GetAsync(string id)
        {
            return await _context.FaqGroups.FindAsync(string.Empty, id);
        }

        public async Task UpdateAsync(FaqGroup item)
        {
            var existing = await _context.FaqGroups.FindAsync(item.PartitionKey, item.RowKey);

            if (existing == null)
            {
                _context.FaqGroups.Add(item);
            }
            else
            {
                existing.GroupName = item.GroupName;
                existing.DisplayOrder = item.DisplayOrder;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await _context.FaqGroups.FindAsync(string.Empty, id);

            if (existing != null)
            {
                _context.FaqGroups.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
    }
}
