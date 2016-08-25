using System.Collections.Generic;

namespace Web.UI.Helpers
{
    public static class SchoolDetailsHelpers
    {
        private static readonly IDictionary<string, string> OfstedLookup =
            new Dictionary<string, string>
            {
                {"", "No Ofsted assessment published"},
                {"1", "Outstanding"},
                {"2", "Good"},
                {"3", "Requires Improvement"},
                {"4", "Inadequate"}
            };

        public static string GetPhasesOfEducation(dynamic schoolDetails)
        {
            var phasesOfEducation = new List<string>();
            if (IsTrue(schoolDetails.ISPRIMARY)) phasesOfEducation.Add("Primary");
            if (IsTrue(schoolDetails.ISSECONDARY)) phasesOfEducation.Add("Secondary");
            if (IsTrue(schoolDetails.ISPOST16)) phasesOfEducation.Add("16 to 18");
            return phasesOfEducation.Count > 0 ? FormatHelpers.GrammarCase(phasesOfEducation.ToArray()) : null;
        }

        public static string GetAddress(dynamic schoolDetails)
        {
            return FormatHelpers.ConcatNonEmpties(", ", schoolDetails.STREET, schoolDetails.TOWN, schoolDetails.POSTCODE);
        }

        private static bool IsTrue(dynamic x)
        {
            return x?.ToString() == "1";
        }

        public static string GetOfstedRating(dynamic inspectionOutcome)
        {
            return OfstedLookup[inspectionOutcome?.ToString() ?? ""];
        }
    }
}