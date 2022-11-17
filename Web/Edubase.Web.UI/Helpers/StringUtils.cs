using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Helpers
{
    public class StringUtils
    {
        public static string ElementIdFormat(string text)
        {
            return text.Replace(' ', '-').ToLowerInvariant();
        }
    }
}
