using Dynamitey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using Edubase.Common;
using Edubase.Data.Entity.Lookups;

namespace Edubase.Import.Helpers
{
    public class Helper
    {
        /// <summary>
        /// Gets a property value and automatically creates lower case versions of property names supplied.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propNames">It will return the first property that exists</param>
        /// <returns></returns>
        public static dynamic GetPropertyValue(dynamic obj, params string[] propNames)
        {
            propNames = propNames.Concat(propNames.Select(x => x.ToLower())).Distinct().ToArray();

            foreach (var item in propNames)
                if (PropertyExists(obj, item))
                    return Dynamic.InvokeGet(obj, item);
            return null;
        }

        public static bool PropertyExists(dynamic settings, string name) => settings.GetType().GetProperty(name) != null;

        public static short? ToShort(string data)
        {
            data = data.Clean();
            if (data != null)
            {
                short temp;
                if (short.TryParse(data, out temp)) return temp;
            }
            return null;
        }

        


    }
}
