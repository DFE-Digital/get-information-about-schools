using Edubase.Data.Entity;
using System.Collections.Generic;

namespace Edubase.Web.UI.Helpers
{
    public static class SchoolDetailsHelpers
    {
        //private static readonly IDictionary<string, string> OfstedLookup =
        //    new Dictionary<string, string>
        //    {
        //        {"", "No Ofsted assessment published"},
        //        {"1", "Outstanding"},
        //        {"2", "Good"},
        //        {"3", "Requires Improvement"},
        //        {"4", "Inadequate"}
        //    };

        //public static string GetPhasesOfEducation(dynamic schoolDetails)
        //{
        //    var phasesOfEducation = new List<string>();
        //    if (IsTrue(schoolDetails.ISPRIMARY)) phasesOfEducation.Add("Primary");
        //    if (IsTrue(schoolDetails.ISSECONDARY)) phasesOfEducation.Add("Secondary");
        //    if (IsTrue(schoolDetails.ISPOST16)) phasesOfEducation.Add("16 to 18");
        //    return phasesOfEducation.Count > 0 ? FormatHelpers.GrammarCase(phasesOfEducation.ToArray()) : null;
        //}

        public static string GetAddress(Establishment schoolDetails)
        {
            return FormatHelpers.ConcatNonEmpties(", ", schoolDetails.Address.Line1, schoolDetails.Address.CityOrTown, schoolDetails.Address.PostCode);
        }
        
        public static string GetLatLng(dynamic schoolDetails)
        {
            return schoolDetails.Location.coordinates.ToString();
        }

            return x?.ToString() == "1";
    }
}