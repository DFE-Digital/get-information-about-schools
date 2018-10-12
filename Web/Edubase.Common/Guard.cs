using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common
{
    public static class Guard
    {
        public static void IsTrue<T>(bool condition) where T : Exception, new()
        {
            if (!condition) throw new T();
        }

        public static void IsFalse<T>(bool condition) where T : Exception, new()
        {
            if (condition) throw new T();
        }

        public static void IsTrue<T>(bool condition, Func<T> fnc) where T : Exception, new()
        {
            if (!condition) throw fnc();
        }

        public static void IsFalse<T>(bool condition, Func<T> fnc) where T : Exception, new()
        {
            if (condition) throw fnc();
        }

        public static void IsNotNull<T>(object obj, Func<T> fnc) where T : Exception, new()
        {
            if (obj == null) throw fnc();
        }

        public static void Is<T>(object obj, Func<Exception> fnc)
        {
            if (!(obj is T)) throw fnc();
        }

        public static void Is<T>(object obj, string msg)
        {
            if (!(obj is T)) throw new Exception(msg);
        }
    }
}
