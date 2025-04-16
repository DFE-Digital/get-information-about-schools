using System;
using System.ComponentModel.DataAnnotations;
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
                var hasDotsOnly = Regex.IsMatch(uriResult.AbsolutePath ?? "", @"[.]{2,}$");

                return isHttp && !hasDotsOnly;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return "The Url entered is invalid";
        }
    }
}
