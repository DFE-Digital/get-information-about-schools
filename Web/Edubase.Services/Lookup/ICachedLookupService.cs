using System.Threading.Tasks;
using System;
using System.Linq.Expressions;

namespace Edubase.Services.Lookup
{
    public interface ICachedLookupService : ILookupService
    {
        Task<string> GetNameAsync(string lookupName, int? id, string domain = null);
        Task<string> GetNameAsync(Expression<Func<int?>> expression, string domain = null);
        bool IsLookupField(string name);
    }
}