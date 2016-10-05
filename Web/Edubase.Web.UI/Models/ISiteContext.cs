using System.Collections.Generic;
using Web.Domain;

namespace Web.UI.Models
{
    public interface ISiteContext
    {
        Permissions Permissions { get; }
        SchoolDetails CurrentSchool { get; set; }
        IEnumerable<SchoolDetails> AllowedSchools { get; set; }
    }
}