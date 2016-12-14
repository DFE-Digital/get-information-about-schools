using Edubase.Data.Entity;

namespace Edubase.Data
{
    public class ApplicationDbContextFactory : IApplicationDbContextFactory
    {
        public IApplicationDbContext Create() => new ApplicationDbContext();
    }
}
