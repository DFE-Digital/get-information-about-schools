using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using System;
using System.Linq.Expressions;

namespace Edubase.Services.Lookup
{
    public interface ICachedLookupService : ILookupService
    {
        string GetName(string lookupName, int? id);
        Task<string> GetNameAsync(string lookupName, int? id, string domain = null);
        Task<string> GetNameAsync(Expression<Func<int?>> expression, string domain = null);
        bool IsLookupField(string name);
    }
}