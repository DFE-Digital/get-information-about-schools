using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories.EF
{
    public class SqlFaqItemRepository : IFaqItemRepository
    {
        private readonly FrontEndDbContext _context;

        public SqlFaqItemRepository(FrontEndDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(FaqItem entity)
        {
            _context.FaqItems.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(params FaqItem[] entities)
        {
            _context.FaqItems.AddRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(IEnumerable<FaqItem> entities)
        {
            await CreateAsync(entities.ToArray());
        }

        public async Task<Page<FaqItem>> GetAllAsync(int take, TableContinuationToken skip = null)
        {
            var results = await _context.FaqItems.Take(take).ToListAsync();
            return new Page<FaqItem>(results, null);
        }

        public async Task<FaqItem> GetAsync(string id)
        {
            return await _context.FaqItems.FindAsync(string.Empty, id);
        }

        public async Task UpdateAsync(FaqItem item)
        {
            var existing = await _context.FaqItems.FindAsync(item.PartitionKey, item.RowKey);

            if (existing == null)
            {
                _context.FaqItems.Add(item);
            }
            else
            {
                existing.Title = item.Title;
                existing.Content = item.Content;
                existing.DisplayOrder = item.DisplayOrder;
                existing.GroupId = item.GroupId;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await _context.FaqItems.FindAsync(string.Empty, id);

            if (existing != null)
            {
                _context.FaqItems.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
    }
}
