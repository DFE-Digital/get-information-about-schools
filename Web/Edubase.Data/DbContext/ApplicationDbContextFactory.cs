namespace Edubase.Data.DbContext
{
    public class ApplicationDbContextFactory<TConcrete> : 
        IApplicationDbContextFactory,
        IInMemoryApplicationDbContextFactory
        where TConcrete : class, IApplicationDbContext, new()
    {
        private TConcrete _priorValue = null;
        public bool Retain { get; set; }
        public IApplicationDbContext Obtain() => _priorValue ?? (_priorValue = new TConcrete());
        public IApplicationDbContextFactory WhilstRetaining()
        {
            Retain = true;
            return this;
        }
    }
    
    public interface IInMemoryApplicationDbContextFactory : IApplicationDbContextFactory
    {

    }

    public interface IApplicationDbContextFactory
    {
        /// <summary>
        /// Whether to retain a created object and supply it to later invocations of Obtain()
        /// </summary>
        bool Retain { get; set; }

        IApplicationDbContext Obtain();

        /// <summary>
        /// Retains the value for future Obtain requests
        /// </summary>
        /// <returns></returns>
        IApplicationDbContextFactory WhilstRetaining();
    }
    
}
