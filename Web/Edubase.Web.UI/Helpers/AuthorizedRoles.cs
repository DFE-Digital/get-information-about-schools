using Edubase.Services.Security;
namespace Edubase.Web.UI.Helpers
{
    public static class AuthorizedRoles
    {
        public const string IsAdmin = EdubaseRoles.ROLE_BACKOFFICE;
        public const string CanDefineAdditionalAddresses = EdubaseRoles.ROLE_BACKOFFICE + "," + EdubaseRoles.EDUBASE_CMT + "," + EdubaseRoles.IEBT;
        public const string CanAccessTools = EdubaseRoles.ROLE_BACKOFFICE + "," + EdubaseRoles.IEBT;
        public const string CanSearchIndependentSchools = EdubaseRoles.ROLE_BACKOFFICE + "," + EdubaseRoles.EDUBASE + "," + EdubaseRoles.EDUBASE_CMT + "," + EdubaseRoles.IEBT;
        public const string CanBulkCreateFreeSchools = EdubaseRoles.ROLE_BACKOFFICE + "," + EdubaseRoles.EDUBASE + "," + EdubaseRoles.EDUBASE_CMT + "," + EdubaseRoles.FST;
        public const string CanBulkAssociateEstabs2Groups = EdubaseRoles.ROLE_BACKOFFICE + "," + EdubaseRoles.EDUBASE + "," + EdubaseRoles.EDUBASE_CMT + "," + EdubaseRoles.AP_AOS;
        public const string CanManageAcademyTrusts = EdubaseRoles.ROLE_BACKOFFICE + "," + EdubaseRoles.EDUBASE + "," + EdubaseRoles.EDUBASE_CMT + "," + EdubaseRoles.AP_AOS;
        public const string CanBulkUpdateEstablishments = EdubaseRoles.EDUBASE + "," + EdubaseRoles.EDUBASE_CMT + "," + EdubaseRoles.AP_AOS + "," + EdubaseRoles.APT + "," + EdubaseRoles.EDUBASE_CHILDRENS_CENTRE_POLICY + "," + EdubaseRoles.EFADO + "," + EdubaseRoles.EFAHNS + "," + EdubaseRoles.FST + "," + EdubaseRoles.IEBT + "," + EdubaseRoles.SOU + "," + EdubaseRoles.EDUBASE_LACCDO + "," + EdubaseRoles.LADO + "," + EdubaseRoles.LSU + "," + EdubaseRoles.UKRLP + "," + EdubaseRoles.YCS;
        public const string CanBulkUpdateGovernors = EdubaseRoles.ROLE_BACKOFFICE + "," + EdubaseRoles.ESTABLISHMENT + "," + EdubaseRoles.EDUBASE_GROUP_MAT + "," + EdubaseRoles.EFADO;
        public const string CanBulkCreateAcademies = EdubaseRoles.ROLE_BACKOFFICE + "," + EdubaseRoles.EFADO + "," + EdubaseRoles.AP_AOS;
        public const string CanManageAcademyOpenings = EdubaseRoles.ROLE_BACKOFFICE + "," + EdubaseRoles.EFADO + "," + EdubaseRoles.AP_AOS;
        public const string CanManageSecureAcademy16To19Openings =  EdubaseRoles.YCS + "," + EdubaseRoles.ROLE_BACKOFFICE;
        public const string CanMergeEstablishments = EdubaseRoles.ROLE_BACKOFFICE + "," + EdubaseRoles.EDUBASE + "," + EdubaseRoles.EDUBASE_CMT + "," + EdubaseRoles.AP_AOS + "," + EdubaseRoles.APT + "," + EdubaseRoles.EFADO + "," + EdubaseRoles.FST + "," + EdubaseRoles.IEBT + "," + EdubaseRoles.SOU;
        public const string CanApprove = EdubaseRoles.ROLE_BACKOFFICE + "," + EdubaseRoles.EDUBASE + "," + EdubaseRoles.EDUBASE_CMT + "," + EdubaseRoles.AP_AOS + "," + EdubaseRoles.APT + "," + EdubaseRoles.EDUBASE_CHILDRENS_CENTRE_POLICY + "," + EdubaseRoles.EFADO + "," + EdubaseRoles.EFAHNS + "," + EdubaseRoles.FST + "," + EdubaseRoles.IEBT + "," + EdubaseRoles.SOU + "," + EdubaseRoles.EDUBASE_LACCDO + "," + EdubaseRoles.LADO + "," + EdubaseRoles.LSU +  "," + EdubaseRoles.YCS;
        public const string CanSeeChildrensCentreGroupManagerEmail = EdubaseRoles.ROLE_BACKOFFICE + "," + EdubaseRoles.EDUBASE_CHILDRENS_CENTRE_POLICY + "," + EdubaseRoles.EDUBASE_LACCDO;
    }
}
