using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Edubase.Common
{
    public class UriHelper
    {
        public static T DeserializeUrlToken<T>(string token)
            => JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(token)));

        public static string SerializeToUrlToken<T>(T obj) where T : class
            => obj == null ? null : HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));

        public static T TryDeserializeUrlToken<T>(string token)
            => !string.IsNullOrWhiteSpace(token) ? DeserializeUrlToken<T>(token) : default(T);
        
    }
}
