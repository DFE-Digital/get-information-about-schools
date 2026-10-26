using System;
using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.EF
{
    public class SqlTokenRepository : ITokenRepository
    {
        private readonly FrontEndDbContext _context;

        public SqlTokenRepository(FrontEndDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Token message)
        {
            _context.Tokens.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<Token> GetAsync(string id)
        {
            SplitId(id, out var partitionKey, out var rowKey);
            return await _context.Tokens.FindAsync(partitionKey, rowKey);
        }

        public Token Get(string id)
        {
            SplitId(id, out var partitionKey, out var rowKey);
            return _context.Tokens.Find(partitionKey, rowKey);
        }

        public async Task UpdateAsync(Token item)
        {
            var existing = await _context.Tokens.FindAsync(item.PartitionKey, item.RowKey);

            if (existing == null)
            {
                _context.Tokens.Add(item);
            }
            else
            {
                existing.Data = item.Data;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            SplitId(id, out var partitionKey, out var rowKey);
            var existing = await _context.Tokens.FindAsync(partitionKey, rowKey);

            if (existing != null)
            {
                _context.Tokens.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        public static void SplitId(string id, out string partitionKey, out string rowKey)
        {
            if (string.IsNullOrWhiteSpace(id) || id.Length < 5)
            {
                throw new ArgumentException("Id is not valid", nameof(id));
            }

            partitionKey = id.Substring(0, 4);
            rowKey = id.Substring(4);
        }
    }
}
