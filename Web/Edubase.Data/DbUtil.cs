using Edubase.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Data
{
    class DbUtil
    {
        /// <summary>
        /// Wraps a database operation in a retry operation. Will retry the op 1000 times or until success.
        /// </summary>
        /// <param name="func"></param>
        public static void RetryableDBAction(Action func)
        {
            for (int i = 0; i < 1000; i++)
            {
                try
                {
                    func();
                    return;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (i == 999) throw;
                    System.Threading.Thread.Sleep(Random2.Next(10, 60));
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        
        public static PropertyInfo[] GetProperties<T>() => typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        



    }
}
