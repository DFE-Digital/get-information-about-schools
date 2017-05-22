using System.Security.Claims;

namespace Edubase.Services.Security
{
    public class EduClaimTypes
    {
        internal const string ClaimTypeNamespace = "http://www.edubase.gov.uk/";
        public const string SAUserId = ClaimTypeNamespace + "SAUserId";
        public const string FirstName = ClaimTypeNamespace + "FirstName"; // obsolete
        public const string LastName = ClaimTypeNamespace + "LastName"; // olsolete
        public const string UserId = ClaimTypes.NameIdentifier;
        public const string UserName = ClaimTypes.Name; // obsolete
    }
}
