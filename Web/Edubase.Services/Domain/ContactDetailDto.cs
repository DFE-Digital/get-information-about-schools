using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class ContactDetailDto
    {
        public string TelephoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string WebsiteAddress { get; set; }
        public string FaxNumber { get; set; }
    }
}
