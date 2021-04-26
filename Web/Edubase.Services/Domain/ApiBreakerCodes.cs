namespace Edubase.Services.Domain
{
    public static class ApiBreakerCodes
    {
        public const string ERROR_GROUP_MAT_WITH_LINKS = "error.group.edit.close.mat.with.links";
        public const string ERROR_GROUP_SAT_WITH_LINKS = "error.group.edit.close.sat.with.links";

        public static readonly string[] All = new[] {
            ERROR_GROUP_MAT_WITH_LINKS,
            ERROR_GROUP_SAT_WITH_LINKS
        };
    }
}
