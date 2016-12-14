using Edubase.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity.ComplexTypes
{
    [ComplexType, Serializable]
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
