using Edubase.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class PersonDto
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName => ToString();
        public override string ToString() => StringUtil.ConcatNonEmpties(" ", Title, FirstName, LastName);
    }
}
