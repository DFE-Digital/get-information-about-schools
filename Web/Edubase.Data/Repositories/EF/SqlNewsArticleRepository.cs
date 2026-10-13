using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Repositories.EF
{
    public class SqlNewsArticleRepository : INewsArticleRepository
    {
        private readonly FrontEndDbContext _context;

        public SqlNewsArticleRepository(FrontEndDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(NewsArticle entity)
        {
            _context.NewsArticles.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(params NewsArticle[] entities)
        {
            _context.NewsArticles.AddRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(IEnumerable<NewsArticle> entities)
        {
            await CreateAsync(entities.ToArray());
        }

        public async Task<Page<NewsArticle>> GetAllAsync(int take, bool visible = true, int? year = null, TableContinuationToken skip = null, eNewsArticlePartition partitionKey = eNewsArticlePartition.Current)
        {
            var query = _context.NewsArticles.Where(x => x.PartitionKey == partitionKey.ToString());

            if (visible)
            {
                query = query.Where(x => x.ArticleDate <= DateTime.Now);
            }

            if (year is int yearValue)
            {
                query = query.Where(x => x.ArticleDate >= new DateTime(yearValue, 1, 1, 0, 0, 0, DateTimeKind.Local) && x.ArticleDate < new DateTime(yearValue, 12, 31, 23, 59, 59, DateTimeKind.Local));
            }

            query = query.Take(take);

            var results = await query.ToListAsync();
            return new Page<NewsArticle>(results, null);
        }

        public async Task<NewsArticle> GetAsync(string id, eNewsArticlePartition partitionKey = eNewsArticlePartition.Current)
        {
            return await _context.NewsArticles.FindAsync(partitionKey.ToString(), id);
        }

        private async Task ArchiveAsync(string id, string auditUser = "")
        {
            var item = await GetAsync(id);

            var archive = Clone(item);
            archive.PartitionKey = nameof(eNewsArticlePartition.Archive);
            if (archive.Version > 1)
            {
                archive.RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
            }
            _context.NewsArticles.Add(archive);

            if (!string.IsNullOrEmpty(auditUser))
            {
                var deleteEntry = Clone(item);
                deleteEntry.PartitionKey = nameof(eNewsArticlePartition.Archive);
                deleteEntry.Version++;
                deleteEntry.AuditEvent = nameof(eNewsArticleEvent.Delete);
                deleteEntry.AuditUser = auditUser;
                deleteEntry.AuditTimestamp = DateTime.Now;
                deleteEntry.RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
                _context.NewsArticles.Add(deleteEntry);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id, string auditUser)
        {
            await ArchiveAsync(id, auditUser);

            var row = await _context.NewsArticles.FindAsync(nameof(eNewsArticlePartition.Current), id);
            if (row != null)
            {
                _context.NewsArticles.Remove(row);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(NewsArticle item)
        {
            await ArchiveAsync(item.RowKey);

            var existing = await _context.NewsArticles.FindAsync(nameof(eNewsArticlePartition.Current), item.RowKey);
            if (existing != null)
            {
                existing.Title = item.Title;
                existing.Content = item.Content;
                existing.ArticleDate = item.ArticleDate;
                existing.ShowDate = item.ShowDate;
                existing.Version++;
                existing.AuditUser = item.AuditUser;
                existing.AuditEvent = item.AuditEvent;
                existing.AuditTimestamp = item.AuditTimestamp;
            }

            await _context.SaveChangesAsync();
        }

        private static NewsArticle Clone(NewsArticle item) => new NewsArticle
        {
            PartitionKey = item.PartitionKey,
            RowKey = item.RowKey,
            Title = item.Title,
            ArticleDate = item.ArticleDate,
            ShowDate = item.ShowDate,
            Content = item.Content,
            Version = item.Version,
            Tracker = item.Tracker,
            AuditUser = item.AuditUser,
            AuditEvent = item.AuditEvent,
            AuditTimestamp = item.AuditTimestamp
        };
    }
}
