using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class BulkCreateFreeSchoolsResult : BulkUpdateProgressModel
    {
        public class FreeSchoolDetail
        {
            public int Urn { get; set; }
            public string Name { get; set; }
            public string PhaseOfEducationName { get; set; }
            public string EstablishmentTypeName { get; set; }
            public DateTime OpenDate { get; set; }
            public string ReasonForOpening { get; set; }
            public string EstablishmentStatusName { get; set; }
        }

        public FreeSchoolDetail[] CreatedEstablishments { get; set; }

        public bool HasCreatedEstablishments => CreatedEstablishments != null && CreatedEstablishments.Length > 0;
        
    }
}
