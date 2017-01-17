using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Services.Domain;

namespace Edubase.Services.Lookup
{
    public interface ICachedLookupService : ILookupService
    {
        string GetName(string lookupName, int id);
        Task<string> GetNameAsync(string lookupName, int id);
        bool IsLookupField(string name);
    }
}