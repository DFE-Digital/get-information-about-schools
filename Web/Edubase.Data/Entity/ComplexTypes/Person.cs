using Edubase.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Data.Entity.ComplexTypes
{
    [ComplexType]
    public class Person
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        [NotMapped]
        public string FullName => ToString();

        public override string ToString() => StringUtil.ConcatNonEmpties(" ", Title, FirstName, LastName);
    }
}
