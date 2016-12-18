using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Security
{
    public static class EdubaseRoles
    {
        public const string Establishment = "Establishment";
        public const string MAT = "MAT";
        public const string LA = "LA";
        public const string LAChildrensCentre = "LAChildrensCentre";
        public const string EFA = "EFA"; // education funding agency
        public const string AOS = "AOS"; // Academies Operations and Strategy
        public const string FSG = "FSG"; // free schools group
        public const string IEBT = "IEBT"; // Independent Education and Boarding Schools Team
        public const string School = "School"; // School organisation
        public const string PRU = "PRU"; // pupil referral unit
        public const string ChildrensCentre = "ChildrensCentre";
        public const string Admin = "Admin";

        public static readonly string[] AllRoles = new []{
            Establishment,
            MAT,
            LA,
            LAChildrensCentre,
            EFA,
            AOS,
            FSG,
            IEBT,
            School,
            PRU,
            ChildrensCentre,
            Admin
        };
    }
}
