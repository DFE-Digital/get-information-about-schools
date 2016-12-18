using System;

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

        public IApplicationDbContextFactory NoRetaining(bool dispose)
        {
            if (_priorValue != null) _priorValue.Dispose();
            _priorValue = null;
            Retain = false;
            return this;
        }

        public IApplicationDbContextFactory ClearRetained(bool dispose)
        {
            if (_priorValue != null) _priorValue.Dispose();
            _priorValue = null;
            return this;
        }

        public IApplicationDbContext ObtainNew()
        {
            ClearRetained(true);
            return Obtain();
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
        /// Obtains a new db context disposing of any previous one retained.
        /// </summary>
        /// <returns></returns>
        IApplicationDbContext ObtainNew();

        /// <summary>
        /// Retains the value for future Obtain requests
        /// </summary>
        /// <returns></returns>
        IApplicationDbContextFactory WhilstRetaining();

        /// <summary>
        /// Turns off retaining
        /// </summary>
        /// <param name="dispose"></param>
        /// <returns></returns>
        IApplicationDbContextFactory NoRetaining(bool dispose);

        /// <summary>
        /// Clears/disposes the currently retained db context without turning retaining off,
        /// use Obtain to create a new one which will be retained if Retain = true
        /// </summary>
        /// <param name="dispose"></param>
        /// <returns></returns>
        IApplicationDbContextFactory ClearRetained(bool dispose);

    }

}
