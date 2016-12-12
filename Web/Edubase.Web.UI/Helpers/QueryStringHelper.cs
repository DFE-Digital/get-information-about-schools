using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Helpers
{
    public class QueryStringHelper
    {
        public static string ToQueryString(string key, params int[] ids) => string.Join("&", ids.Select(x => $"{key}={x}"));
    }
}