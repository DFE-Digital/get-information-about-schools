using Edubase.Common;
using System.Reflection;

namespace Edubase.Web.Resources
{
    public static class ApiMessagesHelper
    {
        public static string Get(string key, string defaultValue = "")
        {
            if (key.Clean() == null)
            {
                return defaultValue;
            }
            else
            {
                var rm = new System.Resources.ResourceManager("Edubase.Web.Resources.ApiMessages", Assembly.GetExecutingAssembly());
                return rm.GetString(key) ?? defaultValue;
            }
        }
    }
}
