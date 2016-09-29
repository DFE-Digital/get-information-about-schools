using System;

namespace Edubase.Data.Stubs
{
    public class MAT
    {
        public short GroupUID { get; set; }

        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string GroupTypeCode { get; set; }
        public string GroupType { get; set; }
        public string ClosedDate { get; set; }
        public string GroupStatusCode { get; set; }
        public string GroupStatus { get; set; }


    }

    public class SchoolMAT
    {
        public int URN { get; set; }
        public short LinkedUID { get; set; }
        public DateTime JoinedDate { get; set; }

        public virtual MAT MAT { get; set; }
    }
}
