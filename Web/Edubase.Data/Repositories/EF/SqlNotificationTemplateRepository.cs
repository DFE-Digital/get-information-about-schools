using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories.EF
{
    public class SqlNotificationTemplateRepository : INotificationTemplateRepository
    {
        private readonly FrontEndDbContext _context;

        public SqlNotificationTemplateRepository(FrontEndDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(NotificationTemplate entity)
        {
            _context.NotificationTemplates.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(params NotificationTemplate[] entities)
        {
            _context.NotificationTemplates.AddRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(IEnumerable<NotificationTemplate> entities)
        {
            await CreateAsync(entities.ToArray());
        }

        public async Task<Page<NotificationTemplate>> GetAllAsync(int take, TableContinuationToken skip = null)
        {
            var results = await _context.NotificationTemplates.Take(take).ToListAsync();
            return new Page<NotificationTemplate>(results, null);
        }

        public async Task<NotificationTemplate> GetAsync(string id)
        {
            return await _context.NotificationTemplates.FindAsync(string.Empty, id);
        }

        public async Task UpdateAsync(NotificationTemplate item)
        {
            var existing = await _context.NotificationTemplates.FindAsync(item.PartitionKey, item.RowKey);

            if (existing == null)
            {
                _context.NotificationTemplates.Add(item);
            }
            else
            {
                existing.Content = item.Content;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await _context.NotificationTemplates.FindAsync(string.Empty, id);

            if (existing != null)
            {
                _context.NotificationTemplates.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
    }
}
