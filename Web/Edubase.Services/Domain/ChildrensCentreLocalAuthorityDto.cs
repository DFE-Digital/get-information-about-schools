using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    [Serializable]
    public class ChildrensCentreLocalAuthorityDto
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string FullName => Common.StringUtil.ConcatNonEmpties(" ", FirstName, Surname);
        public string EmailAddress { get; set; }
        public string TelephoneNumber { get; set; }

        public ChildrensCentreLocalAuthorityDto()
        {

        }

    }
}
