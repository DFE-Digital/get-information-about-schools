using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common
{
    public class Invoker
    {
        public static void IgnoringException(Action act)
        {
            try { act(); }
            catch (Exception) { }
        }
    }
}
