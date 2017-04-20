using System.Security.Claims;

namespace Edubase.Services.Security
{
    public class EduClaimTypes
    {
        #region Obsolete claim types - post texuna!
        internal const string ClaimTypeNamespace = "http://www.edubase.gov.uk/";
        public const string EditEstablishment = ClaimTypeNamespace + "EditEstablishment"; 
        public const string EditGroup = ClaimTypeNamespace + "EditGroup";
        public const string CreateEstablishment = ClaimTypeNamespace + "CreateEstablishment";
        public const string CreateGroup = ClaimTypeNamespace + "CreateGroup";
        #endregion

        public const string FirstName = ClaimTypeNamespace + "FirstName";
        public const string LastName = ClaimTypeNamespace + "LastName";
        public const string UserId = ClaimTypes.NameIdentifier;
        public const string UserName = ClaimTypes.Name;
    }
}
