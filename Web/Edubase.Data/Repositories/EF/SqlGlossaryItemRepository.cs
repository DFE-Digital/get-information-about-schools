using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories.EF
{
    public class SqlGlossaryItemRepository : IGlossaryRepository
    {
        private readonly FrontEndDbContext _context;

        public SqlGlossaryItemRepository(FrontEndDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(GlossaryItem message)
        {
            _context.GlossaryItems.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(params GlossaryItem[] messages)
        {
            _context.GlossaryItems.AddRange(messages);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(IEnumerable<GlossaryItem> messages)
        {
            await CreateAsync(messages.ToArray());
        }

        public async Task<Page<GlossaryItem>> GetAllAsync(int take, TableContinuationToken skip = null)
        {
            var results = await _context.GlossaryItems.Take(take).ToListAsync();
            return new Page<GlossaryItem>(results, null);
        }

        public async Task<GlossaryItem> GetAsync(string id)
        {
            return await _context.GlossaryItems.FindAsync(string.Empty, id);
        }

        public async Task UpdateAsync(GlossaryItem item)
        {
            var existing = await _context.GlossaryItems.FindAsync(item.PartitionKey, item.RowKey);

            if (existing == null)
            {
                _context.GlossaryItems.Add(item);
            }
            else
            {
                existing.Title = item.Title;
                existing.Content = item.Content;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await _context.GlossaryItems.FindAsync(string.Empty, id);

            if (existing != null)
            {
                _context.GlossaryItems.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
    }
}
