using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Edubase.Web.UI.Helpers.ValueProviders
{
    public class ValidateUrl : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return true;

            var strValue = value.ToString();
            if (string.IsNullOrWhiteSpace(strValue)) return true;

            if (Uri.TryCreate(strValue, UriKind.Absolute, out var uriResult))
            {
                var isHttp =  uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
                var hasTld = uriResult.Host.Contains(".") && uriResult.Host.Split('.').Last().Length >= 2;
                var hasDotsOnly = Regex.IsMatch(uriResult.AbsolutePath ?? "", @"\.+$", RegexOptions.None,
                    TimeSpan.FromMilliseconds(300));

                return isHttp && hasTld && !hasDotsOnly;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return "The Url entered is invalid";
        }
    }
}
