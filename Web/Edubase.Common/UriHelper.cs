using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
                var decodedBytes = WebEncoders.Base64UrlDecode(token);
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
            => obj == null ? null : WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));

        public static T TryDeserializeUrlToken<T>(string token)
            => !string.IsNullOrWhiteSpace(token) ? DeserializeUrlToken<T>(token) : default(T);

        public static string SchoolNameUrl(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? string.Empty : WebUtility.UrlEncode(name.Replace(" ", "-").Replace("/", "-").ToLower());
        }

    }
}
