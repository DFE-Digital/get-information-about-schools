namespace Edubase.Services.Security
{
    public class EduClaimTypes
    {

        /*
         *  These Claims types grant permission to a given user for the associated functionality.
         *  Field level permissions are NOT catered for within this model.
         *  The claim may be predicated / conditional upon applicable predicates for a claim.
         *  e.g., EditEstablishment can have predicates
         *      * = the user can edit all establishment records
         *      urn:[urn] = the user can edit establishment record where URN == [urn] (urn is supplied by SA)
         *      la:[lacode] = the user can edit all establishment types within a given LA. The LA code is supplied by SA
         *      la:[lacode];oftype:lamaintainedschools = the user can edit only LAMAINTAINEDSCHOOLS within LA of supplied LACODE
         *  
         *  Predicates:
         *      * = all
         *      id,id,... = CSV list of entity IDs
         *      
         *  
         */

        internal const string ClaimTypeNamespace = "http://www.edubase.gov.uk/";
        public const string EditEstablishment = ClaimTypeNamespace + "EditEstablishment"; 
        public const string EditGroup = ClaimTypeNamespace + "EditGroup";
        public const string CreateEstablishment = ClaimTypeNamespace + "CreateEstablishment";
        public const string CreateGroup = ClaimTypeNamespace + "CreateGroup";



    }
}
