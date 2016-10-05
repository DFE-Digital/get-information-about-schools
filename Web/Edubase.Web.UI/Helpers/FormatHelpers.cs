using System;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Helpers
{
    public static class FormatHelpers
    {

        public static double ConvertBytesToMegabytes(long bytes, int decimalPlaces = 2, double minimumValue = 0)
        {
            var mb = Math.Round((double)bytes / 1024 / 1024, decimalPlaces);
            return mb > minimumValue ? mb : minimumValue;
        }

        public static string ConcatNonEmpties(string separator, params dynamic[] items)
        {
            return string.Join(separator, items.Where(x => x != null && !string.IsNullOrWhiteSpace(x.Value)));
        }

        public static string ConcatNonEmpties(string separator, params string[] items)
        {
            return string.Join(separator, items.Where(x => x != null && !string.IsNullOrWhiteSpace(x)));
        }


        public static string CreateAnalyticsPath(params string[] items)
        {
            items = items?.Where(x => !string.IsNullOrWhiteSpace(x) && x != "/").Select(x => new string(x.ToCharArray().Select(c => char.IsLetterOrDigit(c) || c == '/' ? c : '-').ToArray())).ToArray();
            if (items != null && items.Length > 0)
            {
                items = items.Select(x => x.TrimEnd('/').TrimStart('/')).ToArray();
                var retVal = string.Join("/", items).ToLower();
                if (!retVal.StartsWith("/")) retVal = "/" + retVal;
                return retVal;
            }
            else return "/";
        }
    }
}