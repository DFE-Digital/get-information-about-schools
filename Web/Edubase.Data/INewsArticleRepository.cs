using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data
{
    public interface INewsArticleRepository
    {
        Task CreateAsync(NewsArticle entity);
        Task CreateAsync(params NewsArticle[] entities);
        Task CreateAsync(IEnumerable<NewsArticle> entities);

        Task<Page<NewsArticle>> GetAllAsync(int take, bool visible = true, int? year = null,
            TableContinuationToken skip = null,
            eNewsArticlePartition partitionKey = eNewsArticlePartition.Current);
        Task<NewsArticle> GetAsync(string id, eNewsArticlePartition partitionKey = eNewsArticlePartition.Current);
        Task UpdateAsync(NewsArticle item);
        Task DeleteAsync(string id, string auditUser);
    }
}
