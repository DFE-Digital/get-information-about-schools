using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services
{
    public abstract class ServiceBase
    {
        //protected ApplicationDbContext DataContext => _dc ?? _dcLazy.Value;
        //private ApplicationDbContext _dc = null;
        //private Lazy<ApplicationDbContext> _dcLazy = new Lazy<ApplicationDbContext>(() => ApplicationDbContext.Create());

        //public ServiceBase(ApplicationDbContext dc)
        //{
        //    _dc = dc;
        //}

        //public ServiceBase()
        //{

        //}

        //protected void Using(Action<ApplicationDbContext> act)
        //{

        //    act(DataContext);
        //}

        //protected T Using<T>(Func<ApplicationDbContext, T> act)
        //{
        //    T retVal = act(DataContext);
        //    return retVal;
        //}

    }
}
