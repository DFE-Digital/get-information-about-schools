namespace Edubase.Services.Domain
{
    public static class ApiWarningCodes
    {
        public const string GROUP_WITH_SIMILAR_NAME_FOUND = "group.with.similar.name.found";
        public const string CONFIRMATION_CC_CLOSE = "confirmation.cc.close";
        public const string CONFIRMATION_FEDERATION_BECOMES_CLOSED_LINKS_REMOVED = "confirmation.federation.becomes.closed.links.removed";
        public const string CONFIRMATION_FEDERATION_NO_LINKS_CLOSE = "confirmation.federation.no.links.close";
        public const string CONFIRMATION_CC_DEMOTE = "confirmation.cc.demote";
        public const string CONFIRMATION_CC_PROMOTE = "confirmation.cc.promote";
        public const string ESTABLISHMENT_WITH_SAME_NAME_LA_POSTCODE_FOUND = "establishment.with.same.name.la.postcode.found";

        public static readonly string[] All = new[] {
            GROUP_WITH_SIMILAR_NAME_FOUND,
            CONFIRMATION_CC_CLOSE,
            CONFIRMATION_FEDERATION_BECOMES_CLOSED_LINKS_REMOVED,
            CONFIRMATION_FEDERATION_NO_LINKS_CLOSE,
            CONFIRMATION_CC_DEMOTE,
            CONFIRMATION_CC_PROMOTE,
            ESTABLISHMENT_WITH_SAME_NAME_LA_POSTCODE_FOUND
        };
    }
}
