using Edubase.Data.Entity;

namespace Edubase.Data
{
    public interface IApplicationDbContextFactory
    {
        IApplicationDbContext Create();
    }
}
