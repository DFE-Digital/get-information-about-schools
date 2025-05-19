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
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("The token can not be null or whitespace");
            }

            try
            {
                var decodedBytes = HttpServerUtility.UrlTokenDecode(token);
                if (decodedBytes == null)
                {
                    throw new FormatException("The token could not be decoded.");
                }

                var decodedString = Encoding.UTF8.GetString(decodedBytes);
                return JsonConvert.DeserializeObject<T>(decodedString);
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Invalid token format: {token}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Token deserialization failed {token}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error occured while processing the token: {token}", ex);
            }
        }

        public static string SerializeToUrlToken<T>(T obj) where T : class
            => obj == null ? null : HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));

        public static T TryDeserializeUrlToken<T>(string token)
            => !string.IsNullOrWhiteSpace(token) ? DeserializeUrlToken<T>(token) : default(T);

        public static string SchoolNameUrl(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? string.Empty : HttpUtility.UrlEncode(name.Replace(" ", "-").Replace("/", "-").ToLower());
        }

    }
}
