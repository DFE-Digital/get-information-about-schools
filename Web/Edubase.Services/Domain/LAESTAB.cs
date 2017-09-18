using Edubase.Common;

namespace Edubase.Services.Domain
{
    public struct LAESTAB
    {
        public string LocalAuthorityCode { get; set; }
        public int EstablishmentNumber { get; set; }
        public static LAESTAB? TryParse(string text)
        {
            text = text.Trim().RemoveSubstring("/");

            var retVal = new LAESTAB();
            if (text.IsInteger() && text.Length == 7)
            {
                retVal.LocalAuthorityCode = text.Substring(0, 3);
                retVal.EstablishmentNumber = int.Parse(text.Substring(3, 4));
                return retVal;
            }
            return null;
        }
    }
}
